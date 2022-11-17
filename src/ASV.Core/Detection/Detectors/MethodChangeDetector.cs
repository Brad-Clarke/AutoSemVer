using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Tracking;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class MethodChangeDetector : IChangeDetector<MethodInfo>
    {
        private readonly IChangeTracker _changeTracker;

        private readonly IChangeDetector<Attribute> _attributeChangeDetector;

        private readonly IChangeDetector<ParameterInfo> _parameterChangeDetector;

        public MethodChangeDetector(IChangeTracker changeTracker, IChangeDetectorFactory changeDetectorFactory)
        {
            _changeTracker = changeTracker;

            _attributeChangeDetector = changeDetectorFactory.Build<Attribute>();
            _parameterChangeDetector = changeDetectorFactory.Build<ParameterInfo>();
        }

        public ChangeLevel DetectChanges(MethodInfo current, MethodInfo original)
        {
            // Check return type
            // Check attributes
            // Check parameters


            return ChangeLevel.None;
        }

        public bool Match(MethodInfo left, MethodInfo right)
        {
            return left.Name == right.Name;
        }
    }
}
