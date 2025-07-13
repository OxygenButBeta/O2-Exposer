namespace O2.Exposer;

[Flags]
public enum AccessModifierTarget : byte {
    Private = 1 << 0,
    Protected = 1 << 1,
    Internal = 1 << 2,
    Public = 1 << 3,
}