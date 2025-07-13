using System.Reflection;
using BenchmarkDotNet.Attributes;
using O2.Exposer;

[MemoryDiagnoser]
public class ExposedMemberBenchmarks {
    Reflect reflectInstance;

    ExposedMember<string,Reflect> exposedMember;
    ExposedMember<string,Reflect> exposedProperty;

    FieldInfo fieldInfo;
    PropertyInfo propertyInfo;

    [GlobalSetup]
    public void Setup() {
        reflectInstance = new Reflect();

        exposedMember = ExposedMember<string,Reflect>.Expose("name", reflectInstance);
        exposedProperty = ExposedMember<string,Reflect>.Expose("PublicName", reflectInstance);

        fieldInfo = typeof(Reflect).GetField("name", BindingFlags.Instance | BindingFlags.NonPublic);
        propertyInfo = typeof(Reflect).GetProperty("PublicName", BindingFlags.Instance | BindingFlags.Public);
    }

    [Benchmark]
    public string ExposedMemberGet() => exposedMember.GetValue();

    [Benchmark]
    public string FieldInfoGet() => (string)fieldInfo.GetValue(reflectInstance);

    [Benchmark]
    public void ExposedMemberSet() => exposedMember.SetValue("abc");

    [Benchmark]
    public void FieldInfoSet() => fieldInfo.SetValue(reflectInstance, "abc");

    [Benchmark]
    public void ExposedMemberPropertySet() => exposedProperty.SetValue("abc");

    [Benchmark]
    public void PropertyInfoSet() => propertyInfo.SetValue(reflectInstance, "abc");

    [Benchmark]
    public void ExposedMemberPropertyGet() => exposedProperty.GetValue();

    [Benchmark]
    public string PropertyInfoGet() => (string)propertyInfo.GetValue(reflectInstance);
}

public class Reflect {
    string name = "12312";
    public string GetName() => name;
    public void SetName(string value) => name = value;

    public string PublicName {
        get => name;
        set => name = value;
    }
}