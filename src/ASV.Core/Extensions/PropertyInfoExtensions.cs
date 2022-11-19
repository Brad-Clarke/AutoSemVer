using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string ToFriendlyName(this PropertyInfo property)
        {
            string propertyName = $"{property.PropertyType.ToFriendlyName()} ";

            propertyName += property.DeclaringType.ToFriendlyName();
            propertyName += $".{property.Name}";

            MethodInfo? getter = property.GetGetMethod(true);

            if (getter == null)
            {
                return propertyName;
            }

            if (getter.IsPublic)
            {
                propertyName += " { get; ";
            }
            else if (getter.IsPrivate)
            {
                propertyName += " { private get; ";
            }
            else if (getter.IsFamily)
            {
                propertyName += " { protected get; ";
            }
            else
            {
                throw new ArgumentException();
            }

            MethodInfo? setter = property.GetSetMethod(true);

            if (setter == null)
            {
                propertyName += "}";

                return propertyName;
            }

            if (setter.IsPublic)
            {
                propertyName += "set; }";
            }
            else if (setter.IsPrivate)
            {
                propertyName += "private set; }";
            }
            else if (setter.IsFamily)
            {
                propertyName += "protected set; }";
            }
            else
            {
                throw new ArgumentException();
            }

            return propertyName;
        }
    }
}
