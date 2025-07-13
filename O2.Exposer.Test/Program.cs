using O2.Exposer;


ExposedMember<float, MyClass> exposedMember =
    ExposedMember<float, MyClass>.Expose("SetOnlyProperty", new MyClass());

exposedMember.Set(2);
exposedMember.Value = 3.5f;

ExposedGroup<MyClass> exposedGroup = new(new MyClass());

ExposedMember<float, MyClass> exposedMember2 =
    exposedGroup.GetMember<float>("GetOnlyProperty");

exposedMember2.Get();

ExposedMember<float, MyClass> exposedMember3 =
    exposedGroup.GetMember<float>("GetAndSetProperty");

Console.WriteLine("GetAndSetProperty value: " + exposedMember3.Get());
exposedMember3.Set(5.5f);



class MyClass {
    float SetOnlyProperty {
        set => Console.WriteLine("SetOnlyProperty called with value: " + value);
    }

    float GetOnlyProperty {
        get {
            Console.WriteLine("GetOnlyProperty called");
            return 42.0f;
        }
    }

    float GetAndSetProperty {
        get => 100.0f;
        set => Console.WriteLine("GetAndSetProperty called with value: " + value);
    }

    float SetAndGetProperty {
        set => Console.WriteLine("SetAndGetProperty called with value: " + value);
        get => 200.0f;
    }
}