using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class ConstructorInfoExtensions
    {
        public static string GetFriendlyName(this ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return $"[{constructor.Name}]";
            }

            string parametersString = string.Join(", ", constructor.GetParameters().Select(p => p.GetType().Name));

            return $"[{constructor.Name}]({parametersString})";
        }
    }
}
