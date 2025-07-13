using System.Runtime.CompilerServices;

namespace o2.Optional;

/// <summary>
/// Provides utility and extension methods for working with <see cref="Optional{T}"/> instances.
/// </summary>
public static class Optional
{
    /// <summary>
    /// Continues the chain if the optional is valid, and invokes the provided action.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <param name="action">The action to invoke if valid.</param>
    /// <returns>The same optional instance.</returns>
    public static Optional<T> IfTrue<T>(this Optional<T> optional, Action<T> action)
    {
        if (!optional.IsValid)
            return new Optional<T>(optional.Value, false);

        action?.Invoke(optional.Value);
        return optional;
    }

    /// <summary>
    /// Evaluates a predicate with the given value and returns a valid optional if it returns true.
    /// </summary>
    /// <typeparam name="T">The type to test.</typeparam>
    /// <param name="predicate">The predicate function.</param>
    /// <param name="value">The value to test.</param>
    /// <returns>A valid optional if predicate returns true; otherwise invalid.</returns>
    public static Optional<T> CallAsOptional<T>(this Func<T, bool> predicate, T value)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return predicate(value) ? new Optional<T>(value) : Optional<T>.Invalid;
    }

    /// <summary>
    /// Continues the chain if the optional is invalid, and invokes the provided action.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <param name="action">The action to invoke if invalid.</param>
    /// <returns>The same optional instance.</returns>
    public static Optional<T> IfFalse<T>(this Optional<T> optional, Action<T> action)
    {
        if (optional.IsValid)
            return new Optional<T>(optional.Value, true);

        action?.Invoke(optional.Value);
        return optional;
    }

    /// <summary>
    /// If the optional is invalid, calls the provided function to inject a new context.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <param name="GetNewContext">The function to get a new optional context.</param>
    /// <returns>The original or the new optional context.</returns>
    public static Optional<T> InjectContextIfInvalid<T>(this Optional<T> optional, Func<Optional<T>> GetNewContext)
    {
        return optional.IsValid ? optional : GetNewContext();
    }

    /// <summary>
    /// If the optional is valid, calls the provided function to inject a new context.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <param name="GetNewContext">The function to get a new optional context.</param>
    /// <returns>The original or the new optional context.</returns>
    public static Optional<T> InjectContextIfValid<T>(this Optional<T> optional, Func<Optional<T>> GetNewContext)
    {
        return optional.IsValid ? optional : GetNewContext();
    }

    /// <summary>
    /// Inverts the validity of the optional value.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <returns>A new optional with inverted validity.</returns>
    public static Optional<T> InvertContext<T>(this Optional<T> optional)
    {
        return new Optional<T>(optional.Value, !optional.IsValid);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the optional is invalid.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <param name="message">An optional custom error message.</param>
    /// <param name="callerName">The calling method name (automatically filled).</param>
    /// <param name="callerFilePath">The calling file path (automatically filled).</param>
    /// <returns>The valid optional instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the optional is invalid.</exception>
    public static Optional<T> ThrowIfInvalid<T>(this Optional<T> optional,
        string message = null,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string callerFilePath = null
    )
    {
        if (optional.IsValid)
            return optional;

        if (string.IsNullOrEmpty(message))
            message = $"Optional<{typeof(T).Name}> is invalid. Caller: {callerName} at {callerFilePath}";

        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Creates a new <see cref="Optional{T}"/> from a validity flag and a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="isValid">Whether the value is valid.</param>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A new optional instance.</returns>
    public static Optional<T> AsOptional<T>(this bool isValid, T value)
    {
        return new Optional<T>(value, isValid);
    }

    /// <summary>
    /// Checks if the optional is invalid.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <returns><c>true</c> if the optional is invalid; otherwise <c>false</c>.</returns>
    public static bool IsInvalid<T>(this Optional<T> optional) => !optional.IsValid;

    /// <summary>
    /// Attempts to get the value from the optional.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optional">The optional instance.</param>
    /// <param name="value">The value if valid; otherwise default.</param>
    /// <returns><c>true</c> if the optional is valid; otherwise <c>false</c>.</returns>
    public static bool TryGetValue<T>(this Optional<T> optional, out T value)
    {
        value = optional.IsValid ? optional.Value : default;
        return optional.IsValid;
    }

    public static Optional<TTarget> TryCast<TSrc, TTarget>(this TSrc value)
    {
        if (value is null)
            return Optional<TTarget>.Invalid;

        if (value is TTarget target)
        {
            return new Optional<TTarget>(target);
        }

        return Optional<TTarget>.Invalid;
    }
}