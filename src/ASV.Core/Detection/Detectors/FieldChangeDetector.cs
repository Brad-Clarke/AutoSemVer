using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Helpers;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class FieldChangeDetector : IChangeDetector<FieldInfo>
    {
        private readonly IChangeTracker _changeTracker;

        public FieldChangeDetector(IChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        public ChangeLevel DetectChanges(FieldInfo current, FieldInfo previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            if (current.FieldType.GetFriendlyName() != previous.FieldType.GetFriendlyName())
            {
                _changeTracker.Track($"Field {current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{previous.Name} Type has been changed from {previous.FieldType.GetFriendlyName()} to {current.FieldType.GetFriendlyName()}.", ChangeType.Change);

                changeLevel = changeLevel.TryChange(previous.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            if (current.IsPublic() != previous.IsPublic())
            {
                if (previous.IsPublic())
                {
                    _changeTracker.Track($"Field {current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{previous.Name} is no longer publicly visible.", ChangeType.Removal);
                }
                else
                {
                    _changeTracker.Track($"Field {current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{previous.Name} is now publicly visible.", ChangeType.Addition);
                }

                changeLevel = changeLevel.TryChange(previous.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));

            return changeLevel;
        }

        public bool Match(FieldInfo left, FieldInfo right)
            => DeepReflectionComparer.Compare(left, right);

        private ChangeLevel CompareAttributes(FieldInfo current, FieldInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => left.GetType().GetFriendlyName() == right.GetType().GetFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Field {current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name} had the Attribute {removed.GetType().GetFriendlyName()} Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Field {current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name} had the Attribute {added.GetType().GetFriendlyName()} Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
