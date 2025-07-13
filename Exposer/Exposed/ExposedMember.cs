using System.Reflection;
using System.Runtime.CompilerServices;

namespace O2.Exposer;

public readonly struct ExposedMember<T> : ISetterOnlyMember<T>, IGetterOnlyMember<T> {
    public Action<object, T> Setter { get; }
    public Func<object, T> Getter { get; }
    public bool HasSetter => Setter != null;
    public bool HasGetter => Getter != null;

    public T Value {
        get => Get();
        set => Set(value);
    }

    readonly object Instance;

    public ExposedMember(MemberInfo memberInfo, object Instance) {
        this.Instance = Instance ?? throw new ArgumentNullException(nameof(Instance));

        (Action<object, T> setter, Func<object, T> getter) CompiledAccessors =
            AccessorCache.CreateOrGetDelegates<T>(memberInfo);

        Getter = CompiledAccessors.getter;
        Setter = CompiledAccessors.setter;

        this.Instance = Instance;
    }

    // Suggestion : With this method reflection is used, consider using the ExposedBag<T> instead for better
    // init performance.if you want to access other members of the type.
    public static ExposedMember<T> Expose(string name, object Instance,
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
        FieldInfo info = Instance.GetType().GetField(name, flags);
        if (info is not null)
            return new ExposedMember<T>(info, Instance);

        PropertyInfo property = Instance.GetType().GetProperty(name, flags);
        if (property is not null)
            return new ExposedMember<T>(property, Instance);

        throw new ArgumentException($"Member '{name}' not found in type '{typeof(T).FullName}'.", nameof(name));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(T value) => Setter(Instance, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get() => Getter(Instance);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(ExposedMember<T> exposedMember) {
        return exposedMember.Value;
    }
}