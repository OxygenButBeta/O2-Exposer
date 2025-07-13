using System.Collections.Concurrent;
using System.Reflection;
namespace O2.Exposer;

/// <summary>"
/// Caches compiled getter and setter delegates for fields and properties of a given type and instance,
/// optimizing repeated access by avoiding redundant delegate compilation.
/// Thread-safe for concurrent use.
/// </summary>
public static class CompiledDelegateCache<TMember, TInstance> {
    static readonly ConcurrentDictionary<MemberInfo, (Func<TInstance, TMember> getter,Action<TInstance, TMember> setter)>
        delegateCache = new();

    public static (Func<TInstance, TMember> getter, Action<TInstance, TMember> setter) CreateOrGetDelegates(MemberInfo memberInfo) {
        if (memberInfo is not FieldInfo and not PropertyInfo)
            throw new ArgumentException("Member must be a field or property", nameof(memberInfo));

        return delegateCache.GetOrAdd(memberInfo, info =>
            RuntimeDelegateCompiler<TMember, TInstance>.CompileDelegatesForMember(info));
    }
}