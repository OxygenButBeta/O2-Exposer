# O2.Exposer

`O2.Exposer` is a lightweight, high-performance member access library that enables fast runtime access to fields and properties of any object — without the overhead of slow reflection calls.

Traditional reflection in .NET can be slow and impact application performance, especially when accessing members repeatedly. `O2.Exposer` solves this by generating and caching delegate-based getters and setters for each member, ensuring:

- Reflection is performed only once per type (type caching)
- Each member’s getter/setter delegate is compiled once and reused indefinitely (delegate caching)

It also offers advanced filtering capabilities, allowing you to expose members based on access level (public/private), attributes, and accessibility rules. This makes it a perfect choice for creating serialization systems, debugging tools, runtime inspectors, and other utilities where dynamic and efficient member access is critical.

---

## Key Features
-🔍 Access by Name: Access private and public fields and properties via string names.
-⚡ High Performance: Reflection is only used once per member type. Afterwards, access is done through compiled delegates, completely reflection-free and with zero GC allocations.
-🧠 Smart Caching: Caches types, members, and delegates for maximum runtime performance.
-🧹 GC-Free Access: No boxing or allocations during get/set operations — ideal for performance-critical applications.
-🎯 Flexible Filtering: Filter exposed members by:
Access level (private, public, etc.)
Attributes (e.g. [SerializeField])
Accessibility (read/write)
🧩 Clean Value Access: Get/set directly via .Value, with full type safety.
🛡 Interface-Based Control: Restrict access via IExposedGetOnly<T> or IExposedSetOnly<T> interfaces.

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

### Using ExposedGroup<T> (Recommended for Accessing Multiple Members)

```csharp
ExposedGroup<TargetType> exposedGroup = new(TargetInstance);
```
### Configure Optional Filters to Control Which Members Are Exposed
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
### Create an Exposed Group with Filters
```csharp
ExposedGroup<TargetType> filteredGroup = new ExposedGroup<TargetType>(TargetInstance, exposerFilter);
```
### Retrieve a Member by Name and Expected Type
```csharp
ExposedMember<string, TargetType> fieldExposed = exposedGroup.GetMember<string>("fieldName");
```
### Convert Exposed Member into Interfaces with Restricted Access
```csharp
IGetterOnlyMember<string, TargetType> readOnly = fieldExposed.AsReadOnly();
ISetterOnlyMember<string, TargetType> writeOnly = fieldExposed.AsSetOnly();
```
### Expose a Property or Field by Name Without Filters
```csharp
ExposedMember<int, TargetType> intField = ExposedMember<int, TargetType>.Expose("GetIntField", TargetInstance);
```
