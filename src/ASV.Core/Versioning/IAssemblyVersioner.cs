namespace ASV.Core.Versioning
{
    public interface IAssemblyVersioner
    {
        public Version GetNewVersion();

        public Version? GetCurrentVersion();
    }
}
