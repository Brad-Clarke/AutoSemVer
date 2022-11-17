using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
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

        public ChangeLevel DetectChanges(Assembly current, Assembly original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;
            
            CollectionHelper.Compare(current.GetLoadedTypes(), original.GetLoadedTypes())
                .OnCompare((left, right) => _typeChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"{removed.Name} was removed from the Assembly.", ChangeType.Removal, removed.IsPublic);

                    changeLevel = changeLevel.TryChange(removed.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _typeChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"{added.Name} was added to the Assembly.", ChangeType.Removal, added.IsPublic);

                    changeLevel = changeLevel.TryChange(added.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        public bool Match(Assembly left, Assembly right)
        {
            return left.GetName().Name == right.GetName().Name;
        }
    }
}
