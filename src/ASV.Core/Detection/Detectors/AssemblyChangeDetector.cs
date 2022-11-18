using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class AssemblyChangeDetector : IChangeDetector<Assembly>
    {
        private readonly IChangeDetector<Type> _typeChangeDetector;

        private readonly IChangeTracker _changeTracker;

        public AssemblyChangeDetector(IChangeDetectorFactory changeDetectorFactory, IChangeTracker changeTracker)
        {
            _typeChangeDetector = changeDetectorFactory.Build<Type>();
            _changeTracker = changeTracker;
        }

        public ChangeLevel DetectChanges(Assembly current, Assembly previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetLoadedTypes().Where(t => !t.IsSystemGenerated()).ToArray(), previous.GetLoadedTypes().Where(t => !t.IsSystemGenerated()).ToArray())
                .OnCompare((left, right) => _typeChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"{GetTypeName(removed)} [{removed.GetFriendlyName()}] was Removed from the Assembly.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _typeChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"{GetTypeName(added)} [{added.GetFriendlyName()}] was Added to the Assembly.", ChangeType.Addition);

                    DisplayNewTypeDetails(added);

                    changeLevel = changeLevel.TryChange(added.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;

            string GetTypeName(Type type)
            {
                if (type.IsInterface)
                {
                    return "Interface";
                }

                return "Class";
            }
        }

        public bool Match(Assembly left, Assembly right)
        {
            return left.GetName().Name == right.GetName().Name;
        }

        private void DisplayNewTypeDetails(Type type)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(t => !t.IsSpecialName))
            {
                _changeTracker.Track($"Field [{type.GetFriendlyName()}.{field.Name}] was Added.", ChangeType.Addition);
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(t => !t.IsSpecialName))
            {
                _changeTracker.Track($"Property [{type.GetFriendlyName()}.{property.Name}] was Added.", ChangeType.Addition);
            }

            foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(t => !t.IsSpecialName))
            {
                _changeTracker.Track($"Field [{type.GetFriendlyName()}].{constructor.GetFriendlyName()} was Added.", ChangeType.Addition);
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(t => !t.IsSpecialName))
            {
                string name = method.Name;

                _changeTracker.Track($"Method [{type.GetFriendlyName()}].{method.GetFriendlyName()} was Added.", ChangeType.Addition);
            }
        }
    }
}
