using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Extensions;
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

            if (current.ReturnType.GetFriendlyName() != previous.ReturnType.GetFriendlyName())
            {
                _changeTracker.Track($"Method [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{previous.Name}] Type has been changed from [{previous.ReturnType.GetFriendlyName()}] to [{current.ReturnType.GetFriendlyName()}].", ChangeType.Change);

                changeLevel = changeLevel.TryChange(previous.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            if (current.IsPublic != previous.IsPublic)
            {
                if (previous.IsPublic)
                {
                    _changeTracker.Track($"Method [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{previous.Name}] is no longer publicly visible.", ChangeType.Removal);
                }
                else
                {
                    _changeTracker.Track($"Method [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{previous.Name}] is now publicly visible.", ChangeType.Addition);
                }

                changeLevel = changeLevel.TryChange(previous.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));
            changeLevel = changeLevel.TryChange(CompareParameters(current, previous));

            return changeLevel;
        }

        public bool Match(MethodInfo left, MethodInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            Type[] leftArguments = left.GetGenericArguments().ToArray();
            Type[] rightArguments = right.GetGenericArguments().ToArray();

            if (leftArguments.Length != rightArguments.Length)
            {
                return false;
            }

            if (!rightArguments.All(l => rightArguments.Any(r => r.Name == l.Name)))
            {
                return false;
            }

            ParameterInfo[] leftParameters = left.GetParameters().ToArray();
            ParameterInfo[] rightParameters = right.GetParameters().ToArray();

            if (leftParameters.Length != rightParameters.Length)
            {
                return false;
            }

            if (!leftParameters.All(l => rightParameters.Any(r => r.Name == l.Name && r.ParameterType.Name == l.ParameterType.Name)))
            {
                return false;
            }

            Attribute[] leftAttributes = left.GetCustomAttributes().ToArray();
            Attribute[] rightAttributes = right.GetCustomAttributes().ToArray();

            if (leftAttributes.Length != rightAttributes.Length)
            {
                return false;
            }

            if (!leftAttributes.All(l => rightAttributes.Any(r => r.GetType().Name == l.GetType().Name)))
            {
                return false;
            }

            return true;
        }

        private ChangeLevel CompareAttributes(MethodInfo current, MethodInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => left.GetType().GetFriendlyName() == right.GetType().GetFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Method [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] had the Attribute [{removed.GetType().GetFriendlyName()}] Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Method [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] had the Attribute [{added.GetType().GetFriendlyName()}] Added.", ChangeType.Addition);

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
                    _changeTracker.Track($"Method [{original.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] had the Parameter Removed [{removed.GetType().GetFriendlyName()}].", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.Member.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _parameterChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Method [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{current.Name}] had the Parameter Added [{added.GetType().GetFriendlyName()}].", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.Member.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
