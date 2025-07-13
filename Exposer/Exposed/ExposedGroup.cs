using System.Reflection;

namespace O2.Exposer;

public class ExposedGroup<T> {
    readonly Dictionary<string, object> members = new();

    public ExposedGroup(T objectInstance, in ExposerFilter exposerFilter = default) {
        foreach (FieldInfo field in TypeCache<T>.Cache
                     .Fields) {
            if (!IsValidField(field, exposerFilter))
                continue;

            members.Add(field.Name,
                Activator.CreateInstance(typeof(ExposedMember<>).MakeGenericType(field.FieldType), field, objectInstance));
        }

        foreach (PropertyInfo property in TypeCache<T>.Cache
                     .Properties) {
            if (!IsValidProperty(property, exposerFilter))
                continue;

            members.Add(property.Name,
                Activator.CreateInstance(typeof(ExposedMember<>).MakeGenericType(property.PropertyType), property,
                    objectInstance));
        }
    }

    bool IsValidField(FieldInfo field, in ExposerFilter exposerFilter) {
        ExposerFilter defaultFilter = default;
        if (defaultFilter.Equals(exposerFilter))
            return true;

        if (exposerFilter.AccessModifiers == defaultFilter.AccessModifiers)
            return true;

        // Exposer filter is modified, check if the field is allowed
        if (field.IsPublic)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Public))
                return false;

        if (field.IsPrivate)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Private))
                return false;

        if (field.IsFamily)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Protected))
                return false;

        if (field.IsAssembly)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Internal))
                return false;

        if (field.IsInitOnly && exposerFilter.SetterRequired)
            return false;

        if (field.IsLiteral && exposerFilter.SetterRequired)
            return false;

        if (exposerFilter.RequiredAttributes.Length == 0) {
            // No required attributes, so we can expose the field
            return true;
        }

        return exposerFilter.RequiredAttributes.All(requiredType =>
            field.GetCustomAttributes().Any(requiredType.IsInstanceOfType)
        );
    }

    bool IsValidProperty(PropertyInfo property, ExposerFilter exposerFilter) {
        ExposerFilter defaultFilter = default;
        if (defaultFilter.Equals(exposerFilter))
            return true;

        if (exposerFilter.AccessModifiers == defaultFilter.AccessModifiers)
            return true;

        if (property.SetMethod is null && exposerFilter.SetterRequired)
            return false;

        if (property.GetMethod is null && exposerFilter.GetterRequired)
            return false;

        if (property.GetMethod != null && property.GetMethod.IsPublic)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Public))
                return false;

        if (property.GetMethod != null && property.GetMethod.IsPrivate)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Private))
                return false;

        if (property.GetMethod != null && property.GetMethod.IsFamily)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Protected))
                return false;

        if (property.GetMethod != null && property.GetMethod.IsAssembly)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Internal))
                return false;


        if (property.SetMethod != null && property.SetMethod.IsPublic)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Public))
                return false;

        if (property.SetMethod != null && property.SetMethod.IsPrivate)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Private))
                return false;

        if (property.SetMethod != null && property.SetMethod.IsFamily)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Protected))
                return false;

        if (property.SetMethod != null && property.SetMethod.IsAssembly)
            if (!exposerFilter.AccessModifiers.HasFlag(AccessModifierTarget.Internal))
                return false;


        if (exposerFilter.RequiredAttributes.Length == 0) {
            // No required attributes, so we can expose the property
            return true;
        }

        return exposerFilter.RequiredAttributes.All(requiredType =>
            property.GetCustomAttributes().Any(requiredType.IsInstanceOfType));
    }

    public ExposedMember<TTarget> GetMember<TTarget>(string name) {
        if (!members.TryGetValue(name, out var member))
            throw new KeyNotFoundException($"Member '{name}' not found in the expose group.");

        if (member is ExposedMember<TTarget> exposedMember)
            return exposedMember;

        throw new InvalidOperationException(
            $"Member '{name}' is not of type ExposedMember<{typeof(TTarget).Name}>.");
    }
}