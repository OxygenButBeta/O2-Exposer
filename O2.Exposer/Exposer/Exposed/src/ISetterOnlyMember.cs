namespace O2.Exposer;
/// <summary>
/// Represents a writable member of an instance, providing a setter delegate and a method to set its value.
/// </summary>
public interface ISetterOnlyMember<in TMember,in TInstance> {
    Action<TInstance, TMember> SetterDelegate { get; }
    void SetValue(TMember value);
}