namespace O2.Exposer;

public struct ExposerFilter : IEquatable<ExposerFilter> {



    public Type[] RequiredAttributes = [];
    public bool GetterRequired = false;
    public bool SetterRequired = false;

    public AccessModifierTarget AccessModifiers = AccessModifierTarget.Internal | AccessModifierTarget.Public
        | AccessModifierTarget.Protected | AccessModifierTarget.Private;

    public ExposerFilter() {
    }

    public bool Equals(ExposerFilter other) =>
        GetterRequired == other.GetterRequired && SetterRequired == other.SetterRequired &&
        AccessModifiers == other.AccessModifiers;

    public override bool Equals(object obj) =>
        obj is ExposerFilter other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(GetterRequired, SetterRequired, (int)AccessModifiers);
}