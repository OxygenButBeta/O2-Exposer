using System.Reflection;
using o2.Optional;

namespace O2.Exposer;

public static class AccessorCache {
    static readonly Dictionary<MemberInfo, (Delegate getter, Delegate setter)> fieldCache = new();

    public static (Action<object, T> getter, Func<object, T> setter) CreateOrGetDelegates<T>(MemberInfo memberInfo) {
        if (memberInfo is not FieldInfo and not PropertyInfo)
            throw new ArgumentException("Member must be a field or property", nameof(memberInfo)) {
                HelpLink = null,
                HResult = 0,
                Source = null
            };

        if (fieldCache.TryGetValue(memberInfo, out (Delegate getter, Delegate setter) cached)) {
            Console.WriteLine("Using cached delegates for member: " + memberInfo.Name);
            return ((Action<object, T>)cached.getter, (Func<object, T>)cached.setter);
        }

        (Optional<Func<object, T>>, Optional<Action<object, T>>) compiledDelegates =
            RuntimeMemberCompiler<T>.CompileDelegatesForMember(memberInfo);

        fieldCache.Add(memberInfo, (compiledDelegates.Item2.Value, compiledDelegates.Item1.Value));

        return (compiledDelegates.Item2, compiledDelegates.Item1);
    }
}