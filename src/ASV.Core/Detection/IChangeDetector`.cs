using ASV.Core.Enums;

namespace ASV.Core.Detection
{
    public interface IChangeDetector<in T>: IChangeDetector
    {
        ChangeLevel DetectChanges(T current, T original);
    }
}
