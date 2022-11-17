using System.Reflection;

namespace ASV.Core.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool IsPublic(this MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Constructor => ((ConstructorInfo)memberInfo).IsPublic,
                MemberTypes.Event => ((EventInfo)memberInfo).DeclaringType?.IsPublic ?? false,
                MemberTypes.Field => ((FieldInfo)memberInfo).IsPublic,
                MemberTypes.Method => ((MethodInfo)memberInfo).IsPublic,
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetAccessors().Any(member => member.IsPublic),
                MemberTypes.TypeInfo => ((TypeInfo)memberInfo).IsPublic,
                _ => false
            };
        }
    }
}
