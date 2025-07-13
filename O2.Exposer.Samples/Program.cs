using System.Runtime.CompilerServices;
using O2.Exposer;
// Do Not Run its sample code probably, it is going to throw an exception 
// Create an instance of the class to be exposed
// You can inspect the class definition to see which members are accessible in the bottom of the file
SomeExternalClass externalInstance = new();

// ------------------------------------
// Using ExposedGroup<T> (recommended for accessing multiple members)
// ------------------------------------

// Expose all accessible members without filtering
ExposedGroup<SomeExternalClass> exposedGroup = new(externalInstance);

// Optionally, configure filters to control which members are exposed
ExposerFilter exposerFilter = new ExposerFilter {
    GetterRequired = true,
    SetterRequired = true,
    RequiredAttributes = [
        typeof(PreserveBaseOverridesAttribute),
        typeof(CompilerGeneratedAttribute)
        // Add any other required attributes here
    ],
    AccessModifiers = AccessModifierTarget.Public | AccessModifierTarget.Protected | AccessModifierTarget.Internal
};

// Create an exposed group with filters
ExposedGroup<SomeExternalClass> filteredGroup = new ExposedGroup<SomeExternalClass>(externalInstance, exposerFilter);

// Retrieve a member by name and expected type
ExposedMember<string, SomeExternalClass> fieldExposed = exposedGroup.GetMember<string>("fieldName");

// Convert exposed member into interfaces with restricted access
IGetterOnlyMember<string, SomeExternalClass> readOnly = fieldExposed.AsReadOnly();
ISetterOnlyMember<string, SomeExternalClass> writeOnly = fieldExposed.AsSetOnly();

// ------------------------------------
// Using ExposedMember<T> directly
// ------------------------------------

// Expose a property or field by name without filters
ExposedMember<int, SomeExternalClass> intField =
    ExposedMember<int, SomeExternalClass>.Expose("GetIntField", externalInstance);

// You can access and modify the value using the Value property
intField.Value++;
Console.WriteLine("Incremented field value: " + intField.Value);

// Sample class we want to expose members of
class SomeExternalClass {
    // Private field (non-public)
    string fieldName = "Initial Value";

    // Public auto-property with both getter and setter
    int GetIntField { get; set; }

    // Read-only property
    bool GetOnlyProperty { get; } = true;

    // Write-only property
    float SetOnlyProperty {
        set => Console.WriteLine("SetOnlyProperty called with value: " + value);
    }
}