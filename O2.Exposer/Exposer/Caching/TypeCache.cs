using System.Reflection;

namespace O2.Exposer {
    public class TypeCache<T> {
        public static TypeCache<T> Cache => _mRttiCache.Value;
        static readonly Lazy<TypeCache<T>> _mRttiCache = new(() => new TypeCache<T>());
        public IReadOnlySet<FieldInfo> Fields => m_fields;
        public IReadOnlySet<PropertyInfo> Properties => m_properties;

        readonly HashSet<PropertyInfo> m_properties = [];
        readonly HashSet<FieldInfo> m_fields = [];

        const BindingFlags c_flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        TypeCache() {
            foreach (FieldInfo fieldInfo in typeof(T).GetFields(c_flags))
                m_fields.Add(fieldInfo);

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(c_flags))
                m_properties.Add(propertyInfo);
        }
    }
}