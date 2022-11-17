using ASV.Core.Enums;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class FieldChangeDetector : IChangeDetector<FieldInfo>
    {
        public ChangeLevel DetectChanges(FieldInfo current, FieldInfo original)
        {
            return ChangeLevel.None;
        }

        public bool Match(FieldInfo left, FieldInfo right)
        {
            return left.Name == right.Name;
        }
    }
}
