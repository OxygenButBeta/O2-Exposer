using System.Linq.Expressions;
using System.Reflection;
using o2.Optional;

namespace O2.Exposer;

public static class RuntimeMemberCompiler<TMemberType> {


    public static (Optional<Func<object, TMemberType>>, Optional<Action<object, TMemberType>>) CompileDelegatesForMember(
        MemberInfo member) {
        switch (member) {
            case FieldInfo field: {
                Optional<Func<object, TMemberType>> getter = EmitFieldGetter(field);
                Optional<Action<object, TMemberType>> setter = EmitFieldSetter(field);
                return (getter, setter);
            }
            case PropertyInfo prop: {
                Optional<Func<object, TMemberType>> getter = EmitPropertyGetter(prop).Value;
                Optional<Action<object, TMemberType>> setter = EmitPropertySetter(prop).Value;
                return (getter, setter);
            }
            default:
                throw new ArgumentException("Member must be a field or property", nameof(member));
        }
    }

    static Action<object, TMemberType> EmitFieldSetter(FieldInfo field) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
        ParameterExpression valueParam = Expression.Parameter(typeof(TMemberType), "value");
        UnaryExpression instanceCast = field.IsStatic ? null : Expression.Convert(instanceParam, field.DeclaringType!);
        MemberExpression fieldAccess = Expression.Field(instanceCast, field);
        BinaryExpression assign = Expression.Assign(fieldAccess, Expression.Convert(valueParam, field.FieldType));

        Expression<Action<object, TMemberType>> lambda =
            Expression.Lambda<Action<object, TMemberType>>(assign, instanceParam, valueParam);

        return lambda.Compile();
    }

    static Optional<Func<object, TMemberType>> EmitPropertyGetter(PropertyInfo prop) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
        MethodInfo getMethod = prop.GetGetMethod(true);

        if (getMethod == null) // Property has no getter its a write-only property
            return false;

        Expression instanceCast =
            prop.GetMethod!.IsStatic ? null : Expression.Convert(instanceParam, prop.DeclaringType!);

        MethodCallExpression call = Expression.Call(instanceCast, getMethod);
        UnaryExpression convertResult = Expression.Convert(call, typeof(TMemberType));

        Expression<Func<object, TMemberType>> lambda =
            Expression.Lambda<Func<object, TMemberType>>(convertResult, instanceParam);

        return new Optional<Func<object, TMemberType>>(lambda.Compile());
    }

    static Func<object, TMemberType> EmitFieldGetter(FieldInfo field) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
        UnaryExpression instanceCast = field.IsStatic ? null : Expression.Convert(instanceParam, field.DeclaringType!);
        MemberExpression fieldAccess = Expression.Field(instanceCast, field);
        UnaryExpression convertResult = Expression.Convert(fieldAccess, typeof(TMemberType));
        Expression<Func<object, TMemberType>> lambda =
            Expression.Lambda<Func<object, TMemberType>>(convertResult, instanceParam);
        return lambda.Compile();
    }


    static Optional<Action<object, TMemberType>> EmitPropertySetter(PropertyInfo prop) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
        ParameterExpression valueParam = Expression.Parameter(typeof(TMemberType), "value");
        MethodInfo setMethod = prop.GetSetMethod(true);
        // Property must have a setter, otherwise it is a read-only property
        if (setMethod == null)
            return false;

        Expression instanceCast =
            prop.SetMethod!.IsStatic ? null : Expression.Convert(instanceParam, prop.DeclaringType!);

        MethodCallExpression call =
            Expression.Call(instanceCast, setMethod, Expression.Convert(valueParam, prop.PropertyType));

        Expression<Action<object, TMemberType>> lambda =
            Expression.Lambda<Action<object, TMemberType>>(call, instanceParam, valueParam);

        return new Optional<Action<object, TMemberType>>(lambda.Compile());
    }
}