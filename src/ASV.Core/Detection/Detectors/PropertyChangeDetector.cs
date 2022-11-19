using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Helpers;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class PropertyChangeDetector : IChangeDetector<PropertyInfo>
    {
        private readonly IChangeTracker _changeTracker;

        public PropertyChangeDetector(IChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        public ChangeLevel DetectChanges(PropertyInfo current, PropertyInfo previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            if (current.PropertyType.ToFriendlyName() != previous.PropertyType.ToFriendlyName())
            {
                _changeTracker.Track($"Property {previous.ToFriendlyName()} Type has been changed from {previous.PropertyType.ToFriendlyName()} to {current.PropertyType.ToFriendlyName()}.", ChangeType.Change);

                changeLevel = changeLevel.TryChange(previous.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            if (current.IsPublic() != previous.IsPublic())
            {
                if (previous.IsPublic())
                {
                    _changeTracker.Track($"Property {previous.ToFriendlyName()} is no longer publicly visible.", ChangeType.Removal);
                }
                else
                {
                    _changeTracker.Track($"Property {current.ToFriendlyName()} is now publicly visible.", ChangeType.Addition);
                }

                changeLevel = changeLevel.TryChange(previous.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));

            return changeLevel;
        }

        public bool Match(PropertyInfo left, PropertyInfo right)
            => DeepReflectionComparer.Compare(left, right);

        private ChangeLevel CompareAttributes(PropertyInfo current, PropertyInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => left.GetType().ToFriendlyName() == right.GetType().ToFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Property {current.DeclaringType?.ToFriendlyName() ?? "unknown"}.{original.Name} had the Attribute {removed.GetType().ToFriendlyName()} Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Property {current.DeclaringType?.ToFriendlyName() ?? "unknown"}.{original.Name} had the Attribute {added.GetType().ToFriendlyName()} Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
