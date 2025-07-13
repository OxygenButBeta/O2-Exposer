namespace O2.Exposer;
/// <summary>
/// Represents a readable member of an instance, providing a getter delegate and a method to get its value.
/// </summary>
public interface IGetterOnlyMember<out T,in TInstance> {
    Func<TInstance, T> GetterDelegate { get; }
    T GetValue();
}