using System.Reflection;

namespace ASV.Core.Helpers
{
    public static class DeepReflectionComparer
    {
        public static bool Compare(Type left, Type right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Namespace != right.Namespace)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            if (left == typeof(object))
            {
                return right == typeof(object);
            }

            Type[] leftArguments = left.GetGenericArguments().ToArray();
            Type[] rightArguments = right.GetGenericArguments().ToArray();

            if (leftArguments.Length != rightArguments.Length)
            {
                return false;
            }

            if (!leftArguments.All(l => rightArguments.Any(r => Compare(r, l))))
            {
                return false;
            }

            return true;
        }

        public static bool Compare(MethodInfo left, MethodInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            Type[] leftArguments = left.GetGenericArguments().ToArray();
            Type[] rightArguments = right.GetGenericArguments().ToArray();

            if (leftArguments.Length != rightArguments.Length)
            {
                return false;
            }

            if (!leftArguments.All(l => rightArguments.Any(r => Compare(r, l))))
            {
                return false;
            }

            ParameterInfo[] leftParameters = left.GetParameters().ToArray();
            ParameterInfo[] rightParameters = right.GetParameters().ToArray();

            if (leftParameters.Length != rightParameters.Length)
            {
                return false;
            }

            if (!leftParameters.All(l => rightParameters.Any(r => Compare(r.ParameterType, l.ParameterType))))
            {
                return false;
            }

            return true;
        }

        public static bool Compare(ConstructorInfo left, ConstructorInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            ParameterInfo[] leftParameters = left.GetParameters().ToArray();
            ParameterInfo[] rightParameters = right.GetParameters().ToArray();

            if (leftParameters.Length != rightParameters.Length)
            {
                return false;
            }

            if (!leftParameters.All(l => rightParameters.Any(r => Compare(r, l))))
            {
                return false;
            }

            return true;
        }

        public static bool Compare(ParameterInfo left, ParameterInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            return true;
        }

        public static bool Compare(PropertyInfo left, PropertyInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            return true;
        }

        public static bool Compare(FieldInfo left, FieldInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            return true;
        }

        public static bool Compare(EventInfo left, EventInfo right)
        {
            if (left.Name != right.Name)
            {
                return false;
            }

            if (left.Attributes != right.Attributes)
            {
                return false;
            }

            return true;
        }
    }
}
