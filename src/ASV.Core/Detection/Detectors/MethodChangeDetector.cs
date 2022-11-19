using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Helpers;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class MethodChangeDetector : IChangeDetector<MethodInfo>
    {
        private readonly IChangeTracker _changeTracker;

        private readonly IChangeDetector<ParameterInfo> _parameterChangeDetector;

        public MethodChangeDetector(IChangeTracker changeTracker, IChangeDetectorFactory changeDetectorFactory)
        {
            _changeTracker = changeTracker;

            _parameterChangeDetector = changeDetectorFactory.Build<ParameterInfo>();
        }

        public ChangeLevel DetectChanges(MethodInfo current, MethodInfo previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            if (current.ReturnType.ToFriendlyName() != previous.ReturnType.ToFriendlyName())
            {
                _changeTracker.Track($"Method {previous.ToFriendlyName()} Type has been changed from {previous.ReturnType.ToFriendlyName()} to {current.ReturnType.ToFriendlyName()}.", ChangeType.Change);

                changeLevel = changeLevel.TryChange(previous.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            if (current.IsPublic != previous.IsPublic)
            {
                if (previous.IsPublic)
                {
                    _changeTracker.Track($"Method {previous.ToFriendlyName()} is no longer publicly visible.", ChangeType.Removal);
                }
                else
                {
                    _changeTracker.Track($"Method {previous.ToFriendlyName()} is now publicly visible.", ChangeType.Addition);
                }

                changeLevel = changeLevel.TryChange(previous.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));
            changeLevel = changeLevel.TryChange(CompareParameters(current, previous));

            return changeLevel;
        }

        public bool Match(MethodInfo left, MethodInfo right)
            => DeepReflectionComparer.Compare(left, right);

        private ChangeLevel CompareAttributes(MethodInfo current, MethodInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => left.GetType().ToFriendlyName() == right.GetType().ToFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Method {original.ToFriendlyName()} had the Attribute {removed.GetType().ToFriendlyName()} Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Method {original.ToFriendlyName()} had the Attribute {added.GetType().ToFriendlyName()} Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareParameters(MethodInfo current, MethodInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetParameters(), original.GetParameters())
                .OnCompare((left, right) => _parameterChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Method {original.ToFriendlyName()} had the Parameter Removed {removed.GetType().ToFriendlyName()}.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.Member.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _parameterChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Method {current.ToFriendlyName()} had the Parameter Added {added.GetType().ToFriendlyName()}.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.Member.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
