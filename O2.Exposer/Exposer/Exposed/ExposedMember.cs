using System.Reflection;
using System.Runtime.CompilerServices;

namespace O2.Exposer;

/// <summary>
/// Provides efficient, strongly-typed access to a member (field or property) of an instance,
/// allowing getting and setting the member's value via compiled delegates.
/// Supports both public and non-public members with optional reflection filtering.
/// </summary>
public readonly struct ExposedMember<TMemberType, TInstance> : ISetterOnlyMember<TMemberType, TInstance>,
    IGetterOnlyMember<TMemberType, TInstance> {
    Action<TInstance, TMemberType> ISetterOnlyMember<TMemberType, TInstance>.SetterDelegate => setter;
    Func<TInstance, TMemberType> IGetterOnlyMember<TMemberType, TInstance>.GetterDelegate => getter;

    public TMemberType Value {
        get => GetValue();
        set => SetValue(value);
    }

    readonly Func<TInstance, TMemberType> getter;
    readonly Action<TInstance, TMemberType> setter;
    public readonly bool HasSetter;
    public readonly bool HasGetter;
    readonly TInstance Instance;

    public ExposedMember(MemberInfo memberInfo, TInstance Instance) {
        this.Instance = Instance ?? throw new ArgumentNullException(nameof(Instance));
        ArgumentNullException.ThrowIfNull(memberInfo);

        (Func<TInstance, TMemberType> getter, Action<TInstance, TMemberType> setter) CompiledAccessors =
            CompiledDelegateCache<TMemberType, TInstance>.CreateOrGetDelegates(memberInfo);

        getter = CompiledAccessors.getter;
        setter = CompiledAccessors.setter;
        HasGetter = getter is not null;
        HasSetter = setter is not null;
    }

    // Suggestion : With this method reflection is used, consider using the ExposedBag<T> instead for better
    // init performance.if you want to access other members of the type.
    public static ExposedMember<TMemberType, TInstance> Expose(string name, TInstance Instance,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
        FieldInfo info = Instance.GetType().GetField(name, flags);
        if (info is not null)
            return new ExposedMember<TMemberType, TInstance>(info, Instance);

        PropertyInfo property = Instance.GetType().GetProperty(name, flags);
        if (property is not null)
            return new ExposedMember<TMemberType, TInstance>(property, Instance);

        throw new ArgumentException($"Member '{name}' not found in type '{typeof(TMemberType).FullName}'.",
            nameof(name));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetValue(TMemberType value) {
#if DEBUG
        if (!HasSetter)
            throw new InvalidOperationException(
                $"Exposed member '{typeof(TMemberType).Name}' does not have a setter. Cannot set value.");
#endif
        setter(Instance, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TMemberType GetValue() {
#if DEBUG
        if (!HasGetter)
            throw new InvalidOperationException(
                $"Exposed member '{typeof(TMemberType).Name}' does not have a getter. Cannot get value.");

#endif
        return getter(Instance);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TMemberType(ExposedMember<TMemberType, TInstance> exposedMember) {
        return exposedMember.Value;
    }

    public override string ToString() => Value.ToString();
}