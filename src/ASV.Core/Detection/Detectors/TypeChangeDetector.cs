using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Helpers;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class TypeChangeDetector : IChangeDetector<Type>
    {
        private readonly IChangeTracker _changeTracker;

        private readonly IChangeDetector<ConstructorInfo> _constructorChangeDetector;

        private readonly IChangeDetector<PropertyInfo> _propertyChangeDetector;

        private readonly IChangeDetector<FieldInfo> _fieldChangeDetector;

        private readonly IChangeDetector<EventInfo> _eventChangeDetector;

        private readonly IChangeDetector<MethodInfo> _methodChangeDetector;


        public TypeChangeDetector(IChangeTracker changeTracker, IChangeDetectorFactory changeDetectorFactory)
        {
            _changeTracker = changeTracker;

            _constructorChangeDetector = changeDetectorFactory.Build<ConstructorInfo>();
            _propertyChangeDetector = changeDetectorFactory.Build<PropertyInfo>();
            _fieldChangeDetector = changeDetectorFactory.Build<FieldInfo>();
            _eventChangeDetector = changeDetectorFactory.Build<EventInfo>();
            _methodChangeDetector = changeDetectorFactory.Build<MethodInfo>();
        }

        public ChangeLevel DetectChanges(Type current, Type previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            changeLevel = changeLevel.TryChange(CompareMethods(current, previous));
            changeLevel = changeLevel.TryChange(CompareConstructors(current, previous));
            changeLevel = changeLevel.TryChange(CompareProperties(current, previous));
            changeLevel = changeLevel.TryChange(CompareFields(current, previous));
            changeLevel = changeLevel.TryChange(CompareEvents(current, previous));
            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));
            changeLevel = changeLevel.TryChange(CompareNestedTypes(current, previous));

            return changeLevel;
        }

        public bool Match(Type left, Type right)
            => DeepReflectionComparer.Compare(left, right);

        private ChangeLevel CompareMethods(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetValidMethods(), original.GetValidMethods())
                .OnCompare((left, right) => _methodChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Method {removed.ToFriendlyName()} was Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _methodChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Method {added.ToFriendlyName()} was Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareConstructors(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetConstructors().Where(m => !m.IsSpecialName).ToArray(), original.GetConstructors().Where(m => !m.IsSpecialName).ToArray())
                .OnCompare((left, right) => _constructorChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Constructor {original.ToFriendlyName()}.{removed.ToFriendlyName()} was Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _constructorChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Constructor {original.ToFriendlyName()}.{added.ToFriendlyName()} was Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareProperties(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetProperties(), original.GetProperties())
                .OnCompare((left, right) => _propertyChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Property {original.ToFriendlyName()}.{removed.Name} was Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _propertyChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Property {original.ToFriendlyName()}.{added.Name} was Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareFields(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetFields(), original.GetFields())
                .OnCompare((left, right) => _fieldChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Field {original.ToFriendlyName()}.{removed.Name} was Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _fieldChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Field {original.ToFriendlyName()}.{added.Name} was Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareEvents(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetEvents(), original.GetEvents())
                .OnCompare((left, right) => _eventChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Event {original.ToFriendlyName()}.{removed.Name} was Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.IsPublic() ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _eventChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Event {original.ToFriendlyName()}.{added.Name} was Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(added.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareAttributes(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => left.GetType().ToFriendlyName() == right.GetType().ToFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Attribute {original.ToFriendlyName()}.{removed.GetType().ToFriendlyName()} was Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Attribute {original.ToFriendlyName()}.{added.GetType().ToFriendlyName()} was Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareNestedTypes(Type current, Type original)
        {
            return ChangeLevel.None;
        }
    }
}
