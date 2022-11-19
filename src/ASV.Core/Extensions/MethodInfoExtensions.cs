using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace ASV.Core.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static bool IsExtensionMethod(this MethodInfo method)
            => method.IsDefined(typeof(ExtensionAttribute), true);

        public static string ToFriendlyName(this MethodInfo method)
        {
            string name = method.Name;

            return $"{method.ReturnType.ToFriendlyName()} {method.DeclaringType?.ToFriendlyName() ?? "N/A"}.{method.Name}{GetGenericArgumentString(method)}({GetParameterString(method)})";
        }

        private static string GetParameterString(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            
            if (parameters.Length == 0)
            {
                return string.Empty;
            }

            string parameterString = string.Empty;

            if (method.IsExtensionMethod())
            {
                parameterString += "this ";
            }

            parameterString += string.Join(", ", parameters.Select(p => p.ToFriendlyName()));

            return parameterString;
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
