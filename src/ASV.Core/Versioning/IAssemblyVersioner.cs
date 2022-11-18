using System.Reflection;

namespace ASV.Core.Versioning
{
    public interface IAssemblyVersioner
    {
        public Version GetNewVersion(Assembly current, Assembly previous);
    }
}
