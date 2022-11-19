using System.Reflection;

namespace ASV.Core.Extensions
{
    public static class ParameterInfoExtensions
    {
        public static bool IsNullable(this ParameterInfo parameterInfo)
            => Nullable.GetUnderlyingType(parameterInfo.ParameterType) != null;

        public static string ToFriendlyName(this ParameterInfo parameter)
        {
            string parameterName = string.Empty;

            if (parameter.IsOut)
            {
                parameterName += "out ";
            }
            else if (parameter.ParameterType.IsByRef)
            {
                parameterName += "ref ";
            }

            parameterName += parameter.ParameterType.ToFriendlyName();

            if (parameter.IsNullable())
            {
                parameterName += '?';
            }

            parameterName += $" {parameter.Name}";

            if (parameter.HasDefaultValue)
            {
                parameterName += $" = {parameter.DefaultValue ?? "null"}";
            }

            return parameterName;
        }
    }
}
