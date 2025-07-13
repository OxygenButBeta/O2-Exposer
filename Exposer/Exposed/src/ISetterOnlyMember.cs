namespace O2.Exposer;

public interface ISetterOnlyMember<in T> {
    Action<object, T> Setter { get; }
    void Set(T value);
}