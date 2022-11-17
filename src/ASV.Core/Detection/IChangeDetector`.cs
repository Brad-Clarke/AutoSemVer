using ASV.Core.Enums;

namespace ASV.Core.Detection
{
    public interface IChangeDetector<in T>
    {
        ChangeLevel DetectChanges(T current, T original);

        bool Match(T left, T right);
    }
}
