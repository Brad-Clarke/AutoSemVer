using ASV.Core.Enums;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class PropertyChangeDetector : IChangeDetector<PropertyInfo>
    {
        public ChangeLevel DetectChanges(PropertyInfo current, PropertyInfo original)
        {
            return ChangeLevel.None;
        }

        public bool Match(PropertyInfo left, PropertyInfo right)
        {
            return left.Name == right.Name;
        }
    }
}
