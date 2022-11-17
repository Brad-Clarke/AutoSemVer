using ASV.Core.Enums;
using ASV.Core.Extensions;
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

        public ChangeLevel DetectChanges(PropertyInfo current, PropertyInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            if (current.PropertyType.GetFriendlyName() != original.PropertyType.GetFriendlyName())
            {
                _changeTracker.Track($"Property [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] Type has been changed from [{original.PropertyType.GetFriendlyName()}] to [{current.PropertyType.GetFriendlyName()}].", ChangeType.Change);

                changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            if (current.IsPublic() != original.IsPublic())
            {
                if (original.IsPublic())
                {
                    _changeTracker.Track($"Property [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] is no longer publicly visible.", ChangeType.Removal);
                }
                else
                {
                    _changeTracker.Track($"Property [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] is now publicly visible.", ChangeType.Addition);
                }

                changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
            }

            changeLevel = changeLevel.TryChange(CompareAttributes(current, original));

            return changeLevel;
        }

        public bool Match(PropertyInfo left, PropertyInfo right)
        {
            return left.Name == right.Name;
        }

        private ChangeLevel CompareAttributes(PropertyInfo current, PropertyInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => left.GetType().GetFriendlyName() == right.GetType().GetFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Property [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] had the Attribute [{removed.GetType().GetFriendlyName()}] Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Property [{current.DeclaringType?.GetFriendlyName() ?? "unknown"}.{original.Name}] had the Attribute [{added.GetType().GetFriendlyName()}] Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(original.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
