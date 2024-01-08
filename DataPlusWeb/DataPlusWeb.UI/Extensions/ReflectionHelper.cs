using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System;

public static class ReflectionHelper
{
    #region Private fields region

    private static readonly Type _nullableDefinitionType = typeof(Nullable<>);

    #endregion

    #region Public methods region

    public static IEnumerable<(TMemberInfo Member, Type? AttributeType)> GetMembers<TMemberInfo>(this Type type, BindingFlags bindingFlags, params Type[] attributeTypes)
        where TMemberInfo : MemberInfo
    {
        return GetMembers<TMemberInfo>(type, bindingFlags, MemberTypes.All, attributeTypes);
    }

    public static IEnumerable<(TMemberInfo Member, Type? AttributeType)> GetMembers<TMemberInfo>(this Type type, BindingFlags bindingFlags, MemberTypes types, params Type[] attributeTypes)
        where TMemberInfo : MemberInfo
    {
        types &= GetMemberType(typeof(TMemberInfo));

        foreach (var member in type.GetMembers(bindingFlags))
        {
            if (types.HasFlag(member.MemberType) && member.DeclaringType != typeof(object))
            {
                var validMember = member;
                if (member.MemberType == MemberTypes.Property && member is PropertyInfo propertyInfo && propertyInfo.IsSpecialName ||
                    member.MemberType == MemberTypes.Field && member is FieldInfo fieldInfo && fieldInfo.IsSpecialName ||
                    member.MemberType == MemberTypes.Method && member is MethodInfo methodInfo && methodInfo.IsSpecialName)
                {
                    validMember = null;
                }

                if (validMember != null && TryFindPreferAttribute(member, out var attributeType))
                    yield return ((TMemberInfo)member, attributeType);
            }
        }

        bool TryFindPreferAttribute(MemberInfo member, out Type? attributeType)
        {
            attributeType = null;
            return attributeTypes.Length == 0 || (attributeType = attributeTypes.FirstOrDefault(a => member.IsDefined(a))) != null;
        }

        static MemberTypes GetMemberType(Type memberInfoType)
        {
            if (memberInfoType == typeof(PropertyInfo))
                return MemberTypes.Property;
            else if (memberInfoType == typeof(FieldInfo))
                return MemberTypes.Field;
            else if (memberInfoType == typeof(MethodInfo))
                return MemberTypes.Method;
            else if (memberInfoType == typeof(EventInfo))
                return MemberTypes.Event;
            else if (memberInfoType == typeof(ConstructorInfo))
                return MemberTypes.Constructor;
            if (memberInfoType == typeof(MemberInfo))
                return MemberTypes.All;
            else
                throw new ArgumentException($"{memberInfoType} is not valid.");
        }
    }

    public static bool IsNullable(this Type proposalNullableType)
    {
        return proposalNullableType.IsGenericType == true && proposalNullableType.GetGenericTypeDefinition() == _nullableDefinitionType;
    }

    public static bool TryGetNullableUnderlyingType(this Type proposalNullableType, [MaybeNullWhen(false)] out Type underlyingType)
    {
        if (proposalNullableType.IsGenericType == true && proposalNullableType.GetGenericTypeDefinition() == _nullableDefinitionType)
        {
            underlyingType = proposalNullableType.GenericTypeArguments[0];
            return true;
        }
        underlyingType = null;
        return false;
    }

    #endregion
}
