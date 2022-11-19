using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static string ToFriendlyName(this MethodInfo method)
        {
            return $"{method.ReturnType.ToFriendlyName()} {method.DeclaringType?.ToFriendlyName() ?? "N/A"}.{method.Name}{GetGenericArgumentString(method)}({GetParameterString(method)})";
        }

        private static string GetParameterString(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            
            if (parameters.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", parameters.Select(p => p.ToFriendlyName()));
        }

        private static string GetGenericArgumentString(MethodInfo method)
        {
            Type[] genericArguments = method.GetGenericArguments();

            if (genericArguments.Length == 0)
            {
                return string.Empty;
            }

            string arguments = string.Join(", ", method.GetGenericArguments().Select(a => a.ToFriendlyName()));

            return $"<{arguments}>";
        }
    }
}
