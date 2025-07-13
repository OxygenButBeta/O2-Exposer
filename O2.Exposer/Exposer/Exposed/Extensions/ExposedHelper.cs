using O2.Exposer;

public static class ExposedHelper {
    public static ISetterOnlyMember<TTarget,TInstance> AsSetOnly<TTarget,TInstance>(this ExposedMember<TTarget,TInstance> exposedMember) {
        if (exposedMember.HasSetter)
            return exposedMember;

        throw new InvalidOperationException("Exposed instance does not have a setter.");
    }

    public static IGetterOnlyMember<TTarget,TInstance> AsReadOnly<TTarget,TInstance>(this ExposedMember<TTarget,TInstance> exposedMember) {
        if (exposedMember.HasGetter)
            return exposedMember;

        throw new InvalidOperationException("Exposed instance does not have a getter.");
    }
}