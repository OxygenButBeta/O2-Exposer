namespace o2.Optional;

/// Represents an optional value that may or may not be valid.
/// <typeparam name="T">The type of the optional value.</typeparam>
public readonly struct Optional<T> : IComparable<Optional<T>>, IComparable<T>
{
    /// Represents an invalid Optional instance.
    public static readonly Optional<T> Invalid = new(false);

    /// Indicates whether the value is valid.
    public readonly bool IsValid;

    /// The contained value.
    public readonly T Value;

    /// Initializes a new instance of the <see cref="Optional{T}"/> struct with a specified value and validity.
    public Optional(T value, bool isValid)
    {
        IsValid = isValid;
        Value = value;
    }

    /// Initializes a new instance of the <see cref="Optional{T}"/> struct with a specified value.
    /// The validity is determined based on whether the value is non-null.
    public Optional(T value)
    {
        IsValid = value is not null;
        Value = value;
    }

    Optional(bool isValid)
    {
        IsValid = isValid;
        Value = default;
    }

    public int CompareTo(Optional<T> other)
    {
        return (IsValid, other.IsValid) switch
        {
            (false, false) => 0,
            (true, false) => 1,
            (false, true) => -1,
            _ => Comparer<T>.Default.Compare(Value, other.Value)
        };
    }

    public int CompareTo(T other)
    {
        return Comparer<T>.Default.Compare(Value, other);
    }

    public static Optional<T> Valid<T>(T instance)
    {
        return new Optional<T>(instance, true);
    }

    public static implicit operator bool(Optional<T> optional)
    {
        return optional.IsValid;
    }

    public static implicit operator Optional<T>(T value)
    {
        return new Optional<T>(value, true);
    }

    public static implicit operator Optional<T>((T value, bool valid) tuple)
    {
        return new Optional<T>(tuple.value, tuple.valid);
    }

    public static implicit operator T(Optional<T> optional)
    {
        if (!optional.IsValid)
            throw new InvalidOperationException("Cannot access the value of an invalid Optional.");
        return optional.Value;
    }

    // Convert from bool to Optional<T> is not allowed directly.
    public static implicit operator Optional<T>(bool value)
    {
        return !value
            ? Invalid
            : throw new InvalidOperationException(
                " Cannot convert bool to Optional<T>. Use Optional<T>.Valid(value) instead.");
    }
}