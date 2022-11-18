using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static string GetFriendlyName(this MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length == 0)
            {
                return $"{method.Name}()";
            }
            
            string parametersString = string.Join(", ", parameters.Select(p => $"{p.ParameterType.GetFriendlyName()} {p.Name}"));

            return $"{method.Name}({parametersString})";
        }
    }
}
