using System.Reflection;
using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Tracking;

namespace ASV.Core.Detection.Detectors
{
    public class AssemblyChangeDetector: IChangeDetector<Assembly>
    {
        public Type ForType => typeof(Assembly);

        private readonly IChangeDetector<Type> _typeChangeDetector;

        private readonly IChangeTracker _changeTracker;

        public AssemblyChangeDetector(IChangeDetectorFactory changeDetectorFactory, IChangeTracker changeTracker)
        {
            _typeChangeDetector = changeDetectorFactory.Build<Type>();
            _changeTracker = changeTracker;
        }

        public ChangeLevel DetectChanges(Assembly current, Assembly original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            Type[] currentTypes = current.GetLoadedTypes();
            Type[] previousTypes = original.GetLoadedTypes();
            
            foreach (Type removedType in previousTypes.Where(c => currentTypes.All(p => p.Name != c.Name)))
            {
                _changeTracker.Track($"{removedType.Name} was removed from the Assembly.", ChangeType.Removal, removedType.IsPublic);

                changeLevel = changeLevel.TryChange(removedType.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
            }
            
            List<Type> existing = currentTypes.Where(c => previousTypes.Any(p => p.Name == c.Name)).ToList();
            
            foreach (Type type in existing)
            {
                ChangeLevel newLevel = _typeChangeDetector.DetectChanges(type, existing.Single(t => t.Name == type.Name));

                changeLevel = changeLevel.TryChange(newLevel);
            }

            foreach (Type removedType in previousTypes.Where(c => previousTypes.All(p => p.Name != c.Name)))
            {
                _changeTracker.Track($"{removedType.Name} was added to the Assembly.", ChangeType.Removal, removedType.IsPublic);

                changeLevel = changeLevel.TryChange(removedType.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
            }
            
            return changeLevel;
        }
    }
}
