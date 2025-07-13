using System.Linq.Expressions;
using System.Reflection;
using O2.Optional;

namespace O2.Exposer;

/// <summary>
/// Compiles getter and setter delegates at runtime for fields and properties of a given member,
/// returning optional delegates to safely handle members that may not support get or set operations.
/// Utilizes expression trees for efficient delegate generation.
/// </summary>
public static class RuntimeDelegateCompiler<TMemberType, TInstance> {
    public static (Optional<Func<TInstance, TMemberType>> getter, Optional<Action<TInstance, TMemberType>> setter)
        CompileDelegatesForMember(
            MemberInfo member) {
        switch (member) {
            case FieldInfo field: {
                Optional<Func<TInstance, TMemberType>> getter = EmitFieldGetter(field);
                Optional<Action<TInstance, TMemberType>> setter = EmitFieldSetter(field);
                return (getter, setter);
            }
            case PropertyInfo prop: {
                Optional<Func<TInstance, TMemberType>> getter = EmitPropertyGetter(prop).Value;
                Optional<Action<TInstance, TMemberType>> setter = EmitPropertySetter(prop).Value;
                return (getter, setter);
            }
            default:
                throw new ArgumentException("Member must be a field or property", nameof(member));
        }
    }

    static Action<TInstance, TMemberType> EmitFieldSetter(FieldInfo field) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        ParameterExpression valueParam = Expression.Parameter(typeof(TMemberType), "value");
        UnaryExpression instanceCast = field.IsStatic ? null : Expression.Convert(instanceParam, field.DeclaringType!);
        MemberExpression fieldAccess = Expression.Field(instanceCast, field);
        BinaryExpression assign = Expression.Assign(fieldAccess, Expression.Convert(valueParam, field.FieldType));

        Expression<Action<TInstance, TMemberType>> lambda =
            Expression.Lambda<Action<TInstance, TMemberType>>(assign, instanceParam, valueParam);

        return lambda.Compile();
    }

    static Optional<Func<TInstance, TMemberType>> EmitPropertyGetter(PropertyInfo prop) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        MethodInfo getMethod = prop.GetGetMethod(true);

        if (getMethod == null) // Property has no getter its a write-only property
            return false;

        Expression instanceCast =
            prop.GetMethod!.IsStatic ? null : Expression.Convert(instanceParam, prop.DeclaringType!);

        MethodCallExpression call = Expression.Call(instanceCast, getMethod);
        UnaryExpression convertResult = Expression.Convert(call, typeof(TMemberType));

        Expression<Func<TInstance, TMemberType>> lambda =
            Expression.Lambda<Func<TInstance, TMemberType>>(convertResult, instanceParam);

        return new Optional<Func<TInstance, TMemberType>>(lambda.Compile());
    }

    static Func<TInstance, TMemberType> EmitFieldGetter(FieldInfo field) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        UnaryExpression instanceCast = field.IsStatic ? null : Expression.Convert(instanceParam, field.DeclaringType!);
        MemberExpression fieldAccess = Expression.Field(instanceCast, field);
        UnaryExpression convertResult = Expression.Convert(fieldAccess, typeof(TMemberType));
        Expression<Func<TInstance, TMemberType>> lambda =
            Expression.Lambda<Func<TInstance, TMemberType>>(convertResult, instanceParam);
        return lambda.Compile();
    }


    static Optional<Action<TInstance, TMemberType>> EmitPropertySetter(PropertyInfo prop) {
        ParameterExpression instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        ParameterExpression valueParam = Expression.Parameter(typeof(TMemberType), "value");
        MethodInfo setMethod = prop.GetSetMethod(true);
        // Property must have a setter, otherwise it is a read-only property
        if (setMethod == null)
            return false;

        Expression instanceCast =
            prop.SetMethod!.IsStatic ? null : Expression.Convert(instanceParam, prop.DeclaringType!);

        MethodCallExpression call =
            Expression.Call(instanceCast, setMethod, Expression.Convert(valueParam, prop.PropertyType));

        Expression<Action<TInstance, TMemberType>> lambda =
            Expression.Lambda<Action<TInstance, TMemberType>>(call, instanceParam, valueParam);

        return new Optional<Action<TInstance, TMemberType>>(lambda.Compile());
    }
}