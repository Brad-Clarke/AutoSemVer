namespace ASV.Core.Detection.Factory
{
    public interface IChangeDetectorFactory
    {
        IChangeDetector<T> Build<T>() where T : class;
    }
}
