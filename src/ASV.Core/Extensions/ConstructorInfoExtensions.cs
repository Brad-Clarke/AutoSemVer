using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class ConstructorInfoExtensions
    {
        public static string ToFriendlyName(this ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return $"{constructor.Name}";
            }

            string parametersString = string.Join(", ", constructor.GetParameters().Select(p => p.ToFriendlyName()));

            return $"{constructor.Name}({parametersString})";
        }
    }
}
