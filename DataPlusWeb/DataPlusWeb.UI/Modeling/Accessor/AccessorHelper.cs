using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace DataPlus.Web.UI;

public static class AccessorHelper
{
    #region Private fields region

    private static readonly ConcurrentDictionary<Type, TypeAccessor> _typeAccessorsCache = new();

    #endregion

    #region Private methods region

    private static MemberInfo? GetMember(Type type, string fieldName)
    {
        var memberInfo = (MemberInfo?)type.GetProperty(fieldName) ?? type.GetField(fieldName);

        if (memberInfo == null)
        {
            var baseTypesAndInterfaces = new List<Type>();

            if (type.BaseType != null)
                baseTypesAndInterfaces.Add(type.BaseType);

            baseTypesAndInterfaces.AddRange(type.GetInterfaces());

            foreach (var baseType in baseTypesAndInterfaces)
            {
                memberInfo = GetMember(baseType, fieldName);

                if (memberInfo != null) break;
            }
        }

        return memberInfo;
    }

    /// <summary>
    /// Checks if requested type can bu nullable.
    /// </summary>
    /// <param name="type">Object type.</param>
    /// <returns></returns>
    private static bool IsNullable(Type type)
    {
        if (type.IsClass) return true;
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    #endregion

    #region Internal methods region

    /// <summary>
    /// Bulds an access expression for nested properties while checking for null values.
    /// </summary>
    /// <param name="item">Item that has the requested field name.</param>
    /// <param name="propertyOrFieldName">Item field name.</param>
    /// <returns>Returns the requested field if it exists.</returns>
    internal static Expression GetPropertyOrField(Expression item, string propertyOrFieldName)
    {
        if (string.IsNullOrEmpty(propertyOrFieldName))
            throw new ArgumentException($"{nameof(propertyOrFieldName)} is not specified.");

        var parts = propertyOrFieldName.Split(new char[] { '.' }, 2);

        Expression? field = null;

        var memberInfo = GetMember(item.Type, parts[0]);

        if (memberInfo is PropertyInfo propertyInfo)
            field = Expression.Property(item, propertyInfo);
        else if (memberInfo is FieldInfo fieldInfo)
            field = Expression.Field(item, fieldInfo);

        if (field == null)
            throw new ArgumentException($"Cannot detect the member of {item.Type}", propertyOrFieldName);

        if (parts.Length > 1)
            field = GetPropertyOrField(field, parts[1]);

        // if the value type cannot be null there's no reason to check it for null
        if (!IsNullable(field.Type))
            return field;

        // check if field is null
        return Expression.Condition(Expression.Equal(item, Expression.Constant(null)), Expression.Constant(null, field.Type), field);
    }

    #endregion

    #region Public methods region

    public static TypeAccessor GetTypeAccessor<TInstance>(TInstance instance) => GetTypeAccessor((instance ?? throw new ArgumentNullException(nameof(instance))).GetType());

    public static TypeAccessor GetTypeAccessor<TInstance>() => GetTypeAccessor(typeof(TInstance));

    public static TypeAccessor GetTypeAccessor(Type instanceType)
    {
        return GetTypeAccessor(instanceType, MemberWhere, MemberSelect, ActionWhere, ActionSelect);

        bool MemberWhere(MemberInfo memberInfo) => true;
        MemberAccessor MemberSelect(TypeAccessor type, MemberInfo memberInfo, Func<object?, object?>? getter, Action<object?, object?>? setter) => new(type, memberInfo, getter, setter);

        bool ActionWhere(MethodInfo methodInfo) => true;
        ActionAccessor ActionSelect(TypeAccessor type, MethodInfo methodInfo) => new(type, methodInfo);
    }

    public static TypeAccessor GetTypeAccessor(Type instanceType,
        Func<MemberInfo, bool> memberWhere, Func<TypeAccessor, MemberInfo, Func<object?, object?>?, Action<object?, object?>?, MemberAccessor> memberSelect,
        Func<MethodInfo, bool> actionWhere, Func<TypeAccessor, MethodInfo, ActionAccessor> actionSelect)
    {
        return _typeAccessorsCache.GetOrAdd(instanceType,
            t => t.TryGetNullableUnderlyingType(out var ut) ? _typeAccessorsCache.GetOrAdd(ut, CreateTypeAccessor) : CreateTypeAccessor(t));

        TypeAccessor CreateTypeAccessor(Type t)
        {
            var typeAccessor = new TypeAccessor(t);
            var members = new List<MemberAccessor>();
            var actions = new List<ActionAccessor>();
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var instanceTyped = Expression.Convert(instance, t);

            foreach (var (memberInfo, _) in t.GetMembers<MemberInfo>(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, MemberTypes.Property | MemberTypes.Field | MemberTypes.Method))
            {
                if (memberInfo is MethodInfo methodInfo)
                {
                    var isAction = methodInfo.IsDefined(typeof(ActionAttribute));
                    if (actionWhere(methodInfo))
                    {
                        actions.Add(actionSelect(typeAccessor, methodInfo));
                    }
                }
                else
                {
                    Func<object?, object?>? getter = null;
                    Action<object?, object?>? setter = null;

                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        if (memberWhere(memberInfo))
                        {
                            getter = Expression.Lambda<Func<object?, object?>>(Expression.Convert(Expression.Field(fieldInfo.IsStatic ? null : instanceTyped, fieldInfo), typeof(object)), instance).Compile();
                            if (!fieldInfo.IsLiteral)
                                setter = Expression.Lambda<Action<object?, object?>>(Expression.Assign(Expression.Field(fieldInfo.IsStatic ? null : instanceTyped, fieldInfo), Expression.Convert(value, fieldInfo.FieldType)), instance, value).Compile();
                        }
                    }
                    else if (memberInfo is PropertyInfo propertyInfo)
                    {
                        var indexedParameters = propertyInfo.GetIndexParameters();
                        if ((indexedParameters is null || indexedParameters.Length == 0) && memberWhere(memberInfo))
                        {
                            var getMethod = propertyInfo.CanRead && propertyInfo.GetMethod?.IsPublic == true ? propertyInfo.GetMethod : null;
                            var setMethod = propertyInfo.CanWrite && propertyInfo.SetMethod?.IsPublic == true ? propertyInfo.SetMethod : null;

                            if (getMethod != null)
                                getter = Expression.Lambda<Func<object?, object?>>(Expression.Convert(Expression.Property(getMethod.IsStatic ? null : instanceTyped, propertyInfo), typeof(object)), instance).Compile();

                            if (setMethod != null)
                                setter = Expression.Lambda<Action<object?, object?>>(Expression.Assign(Expression.Property(setMethod.IsStatic ? null : instanceTyped, propertyInfo), Expression.Convert(value, propertyInfo.PropertyType)), instance, value).Compile();
                        }
                    }

                    if (getter != null || setter != null)
                        members.Add(memberSelect(typeAccessor, memberInfo, getter, setter));
                }
            }

            typeAccessor.AddMembers(Sort(members));
            typeAccessor.AddActions(Sort(actions));
            return typeAccessor;

            static IEnumerable<TAccessor> Sort<TAccessor>(IList<TAccessor> accessors)
                where TAccessor : AccessorBase
            {
                List<(TAccessor Accessor, int, bool)> sortList = new(accessors.Count);
                for (int i = 0; i < accessors.Count; i++)
                {
                    var item = accessors[i];
                    if (item.Order is null)
                        sortList.Add((item, i + 1, true));
                    else
                        sortList.Add((item, item.Order.Value, false));
                }
                sortList.Sort(Comparer<(TAccessor, int Order, bool IsAuto)>.Create((a, b) =>
                {
                    var ret = Comparer<int>.Default.Compare(a.Order, b.Order);
                    if (ret == 0)
                        ret = Comparer<bool>.Default.Compare(a.IsAuto, b.IsAuto);
                    return ret;
                }));
                return sortList.Select(a => a.Accessor);
            }
        }
    }

    public static TValue? GetValue<TValue>(this MemberAccessor member) => (TValue?)member.GetValue();

    public static TValue? GetValue<TValue>(this MemberAccessor member, object instance) => (TValue?)member.GetValue(instance);

    public static void SetValue<TValue>(this MemberAccessor member, TValue? value) => member.SetValue(value);

    public static void SetValue<TValue>(this MemberAccessor member, object instance, TValue? value) => member.SetValue(instance, value);

    #endregion
}
