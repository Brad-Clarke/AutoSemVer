using ASV.Core.Enums;

namespace ASV.Core.Detection.Detectors
{
    public class TypeChangeDetector : IChangeDetector<Type>
    {
        public Type ForType => typeof(Type);

        public ChangeLevel DetectChanges(Type current, Type original)
        {
            return ChangeLevel.None;
        }
    }
}
