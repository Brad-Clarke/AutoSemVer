using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Helpers;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class ParameterChangeDetector : IChangeDetector<ParameterInfo>
    {
        private readonly IChangeTracker _changeTracker;

        public ParameterChangeDetector(IChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        public ChangeLevel DetectChanges(ParameterInfo current, ParameterInfo previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            if (current.ParameterType.ToFriendlyName() != previous.ParameterType.ToFriendlyName())
            {
                _changeTracker.Track($"Parameter {current.Member?.Name ?? "unknown"}.{previous.Name} Type has been changed from {previous.ParameterType.ToFriendlyName()} to {current.ParameterType.ToFriendlyName()}.", ChangeType.Change);

                changeLevel = changeLevel.TryChange(previous.Member.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));

            return changeLevel;
        }

        public bool Match(ParameterInfo left, ParameterInfo right)
            => DeepReflectionComparer.Compare(left, right);

        private ChangeLevel CompareAttributes(ParameterInfo current, ParameterInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes()?.ToArray() ?? Array.Empty<Attribute>(), original.GetCustomAttributes()?.ToArray() ?? Array.Empty<Attribute>())
                .OnCompare((left, right) => left.GetType().ToFriendlyName() == right.GetType().ToFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Parameter {current.Member?.Name ?? "unknown"}.{original.Name} had the Attribute {removed.GetType().ToFriendlyName()} Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.Member.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Parameter {current.Member?.Name ?? "unknown"}.{original.Name} had the Attribute {added.GetType().ToFriendlyName()} Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(original.Member.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
