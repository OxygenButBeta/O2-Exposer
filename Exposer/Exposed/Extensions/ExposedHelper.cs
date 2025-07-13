using O2.Exposer;

public static class ExposedHelper {
    public static ISetterOnlyMember<TTarget> AsSetOnly<TTarget>(this ExposedMember<TTarget> exposedMember) {
        if (exposedMember.HasSetter)
            return exposedMember;

        throw new InvalidOperationException("Exposed instance does not have a setter.");
    }

    public static IGetterOnlyMember<TTarget> AsReadOnly<TTarget>(this ExposedMember<TTarget> exposedMember) {
        if (exposedMember.HasGetter)
            return exposedMember;

        throw new InvalidOperationException("Exposed instance does not have a getter.");
    }
}