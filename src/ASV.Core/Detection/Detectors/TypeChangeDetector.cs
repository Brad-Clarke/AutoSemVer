using System.Reflection;
using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;

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

        private readonly IChangeDetector<Attribute> _attributeChangeDetector;


        public TypeChangeDetector(IChangeTracker changeTracker, IChangeDetectorFactory changeDetectorFactory)
        {
            _changeTracker = changeTracker;

            _constructorChangeDetector = changeDetectorFactory.Build<ConstructorInfo>();
            _propertyChangeDetector = changeDetectorFactory.Build<PropertyInfo>();
            _fieldChangeDetector = changeDetectorFactory.Build<FieldInfo>();
            _eventChangeDetector = changeDetectorFactory.Build<EventInfo>();
            _methodChangeDetector = changeDetectorFactory.Build<MethodInfo>();
            _attributeChangeDetector = changeDetectorFactory.Build<Attribute>();
        }

        public ChangeLevel DetectChanges(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            changeLevel = changeLevel.TryChange(CompareMethods(current, original));
            changeLevel = changeLevel.TryChange(CompareConstructors(current, original));
            changeLevel = changeLevel.TryChange(CompareProperties(current, original));
            changeLevel = changeLevel.TryChange(CompareFields(current, original));
            changeLevel = changeLevel.TryChange(CompareEvents(current, original));
            changeLevel = changeLevel.TryChange(CompareAttributes(current, original));
            changeLevel = changeLevel.TryChange(CompareNestedTypes(current, original));

            return changeLevel;
        }

        public bool Match(Type left, Type right)
        {
            return left.FullName == right.FullName;
        }

        private ChangeLevel CompareMethods(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetMethods(), original.GetMethods())
                .OnCompare((left, right) => _methodChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Method {removed.Name} was removed from {original.Name}", ChangeType.Removal, removed.IsPublic);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _methodChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Method {added.Name} was added to {current.Name}", ChangeType.Removal, added.IsPublic);

                    changeLevel = changeLevel.TryChange(added.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
        
        private ChangeLevel CompareConstructors(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetConstructors(), original.GetConstructors())
                .OnCompare((left, right) => _constructorChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Constructor was removed from {original.Name}", ChangeType.Removal, removed.IsPublic);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _constructorChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Constructor was added to {current.Name}", ChangeType.Removal,
                        added.IsPublic);

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
                    _changeTracker.Track($"Property {removed.Name} was removed from {original.Name}", ChangeType.Removal, original.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _propertyChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Property {added.Name} was added to {current.Name}", ChangeType.Removal, current.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
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
                    _changeTracker.Track($"Field {removed.Name} was removed from {original.Name}", ChangeType.Removal, removed.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _fieldChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Field {added.Name} was added to {current.Name}", ChangeType.Removal,
                        current.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
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
                    _changeTracker.Track($"Event {removed.Name} was removed from {original.Name}", ChangeType.Removal, original.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _eventChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Event {added.Name} was added to {current.Name}", ChangeType.Removal,
                        current.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareAttributes(Type current, Type original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;
            
            CollectionHelper.Compare(current.GetCustomAttributes().ToArray(), original.GetCustomAttributes().ToArray())
                .OnCompare((left, right) => _attributeChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Event {removed.GetType().Name} was removed from {original.Name}", ChangeType.Removal, original.IsPublic);

                    changeLevel = changeLevel.TryChange(original.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _attributeChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Event {added.GetType().Name} was added to {current.Name}", ChangeType.Removal,
                        current.IsPublic);

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
