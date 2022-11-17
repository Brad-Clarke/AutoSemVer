using ASV.Core.Enums;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class AttributeChangeDetector : IChangeDetector<Attribute>
    {
        public ChangeLevel DetectChanges(Attribute current, Attribute original)
        {
            return ChangeLevel.None;
        }

        public bool Match(Attribute left, Attribute right)
        {
            return left.GetType().Name == right.GetType().Name;
        }
    }
}
