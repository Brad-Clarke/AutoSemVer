using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static string GetFriendlyName(this MethodInfo method)
        {
            return $"{method.ReturnType.GetFriendlyName()} {method.DeclaringType.GetFriendlyName()}.{method.Name}{GetGenericArgumentString(method)}({GetParameterString(method)})";
        }

        private static string GetParameterString(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            
            if (parameters.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", parameters.Select(p => $"{p.ParameterType.GetFriendlyName()} {p.Name}"));
        }

        private static string GetGenericArgumentString(MethodInfo method)
        {
            Type[] genericArguments = method.GetGenericArguments();

            if (genericArguments.Length == 0)
            {
                return string.Empty;
            }

            string arguments = string.Join(", ", method.GetGenericArguments().Select(a => a.GetFriendlyName()));

            return $"<{arguments}>";
        }
    }
}
