# O2.Exposer

`O2.Exposer` is a lightweight, high-performance member access library that enables fast runtime access to fields and properties of any object — without the overhead of slow reflection calls.

Traditional reflection in .NET can be slow and impact application performance, especially when accessing members repeatedly. `O2.Exposer` solves this by generating and caching delegate-based getters and setters for each member, ensuring:

- Reflection is performed only once per type (type caching)
- Each member’s getter/setter delegate is compiled once and reused indefinitely (delegate caching)

It also offers advanced filtering capabilities, allowing you to expose members based on access level (public/private), attributes, and accessibility rules. This makes it a perfect choice for creating serialization systems, debugging tools, runtime inspectors, and other utilities where dynamic and efficient member access is critical.

---

## Key Features

- Access private and public fields and properties by name
- Create and cache fast delegates for getters and setters
- Filter exposed members by access level, attributes, and accessibility
- Supports implicit value access via `exposed.Value`
- Interface-based restricted access (get-only, set-only)
- Type and delegate caching for maximum performance

## 🔢 Benchmark Comparison

| Operation    | Faster Method              | Slower Method     | Faster Mean (ns) | Slower Mean (ns) |    × Faster |   Performance Gain |
| ------------ | -------------------------- | ----------------- | ---------------: | ---------------: | ----------: | -----------------: |
| Field Get    | `ExposedMemberGet`         | `FieldInfoGet`    |        0.2045 ns |        2.3544 ns | **\~11.5×** | **\~1051% faster** |
| Field Set    | `ExposedMemberSet`         | `FieldInfoSet`    |        1.0329 ns |        5.2493 ns |  **\~5.1×** |  **\~408% faster** |
| Property Get | `ExposedMemberPropertyGet` | `PropertyInfoGet` |        0.8029 ns |        6.7075 ns |  **\~8.4×** |  **\~736% faster** |
| Property Set | `ExposedMemberPropertySet` | `PropertyInfoSet` |        1.0093 ns |        9.9785 ns |  **\~9.9×** |  **\~889% faster** |


## 📦 Installation

Download the latest release from the [Releases](https://github.com/OxygenButBeta/O2.Exposer/releases) page.

Extract the package and add the DLL file (`O2.Exposer.dll`) to your project references manually

## 📌 Sample Usage

```csharp
using System.Runtime.CompilerServices;
using O2.Exposer;

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
    RequiredAttributes = new[] {
        typeof(PreserveBaseOverridesAttribute),
        typeof(CompilerGeneratedAttribute)
        // Add any other required attributes here
    },
    AccessModifiers = AccessModifierTarget.Public | AccessModifierTarget.Protected | AccessModifierTarget.Internal
};

// Create an exposed group with filters
ExposedGroup<SomeExternalClass> filteredGroup = new ExposedGroup<SomeExternalClass>(externalInstance, exposerFilter);

// Retrieve a member by name and expected type
ExposedMember<string> fieldExposed = exposedGroup.GetMember<string>("fieldName");

// Convert exposed member into interfaces with restricted access
IGetterOnlyMember<string> readOnly = fieldExposed.AsReadOnly();
ISetterOnlyMember<string> writeOnly = fieldExposed.AsSetOnly();

// ------------------------------------
// Using ExposedMember<T> directly
// ------------------------------------

// Expose a property or field by name without filters
ExposedMember<int> intField = ExposedMember<int>.Expose("GetIntField", externalInstance);

// You can access and modify the value using the Value property
intField.Value++;
Console.WriteLine("Incremented field value: " + intField.Value);

// Sample class we want to expose members of
class SomeExternalClass {
    // Private field (non-public)
    string fieldName = "Initial Value";

    // Public auto-property with both getter and setter
    public int GetIntField { get; set; }

    // Read-only property
    public bool GetOnlyProperty { get; } = true;

    // Write-only property
    public float SetOnlyProperty {
        set => Console.WriteLine("SetOnlyProperty called with value: " + value);
    }
}
