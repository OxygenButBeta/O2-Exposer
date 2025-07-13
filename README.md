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

## 📌 Sample Usage & 🚀 Quick Start

# Using ExposedGroup<T> (Recommended for Accessing Multiple Members)

```csharp
ExposedGroup<SomeExternalClass> exposedGroup = new(externalInstance);
```
# Configure Optional Filters to Control Which Members Are Exposed
```csharp
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
```
# Create an Exposed Group with Filters
```csharp
ExposedGroup<SomeExternalClass> filteredGroup = new ExposedGroup<SomeExternalClass>(externalInstance, exposerFilter);
```
# Retrieve a Member by Name and Expected Type
```csharp
ExposedMember<string, SomeExternalClass> fieldExposed = exposedGroup.GetMember<string>("fieldName");
```
# Convert Exposed Member into Interfaces with Restricted Access
```csharp
IGetterOnlyMember<string, SomeExternalClass> readOnly = fieldExposed.AsReadOnly();
ISetterOnlyMember<string, SomeExternalClass> writeOnly = fieldExposed.AsSetOnly();
```
# Expose a Property or Field by Name Without Filters
```csharp
ExposedMember<int, SomeExternalClass> intField = ExposedMember<int, SomeExternalClass>.Expose("GetIntField", externalInstance);
```
