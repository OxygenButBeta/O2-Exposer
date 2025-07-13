namespace O2.Exposer;
public interface IGetterOnlyMember<out T> {
    Func<object, T> Getter { get; }
    T Get();
}