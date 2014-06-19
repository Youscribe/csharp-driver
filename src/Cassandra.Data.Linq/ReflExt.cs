using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Cassandra.Data.Linq
{
    internal static class ReflExt
    {
        [ThreadStatic] private static Dictionary<Type, PropertyDescriptorCollection> ReflexionCachePF;

        public static PropertyDescriptorCollection GetPropertiesOrFields(this Type tpy)
        {
            if (ReflexionCachePF == null)
                ReflexionCachePF = new Dictionary<Type, PropertyDescriptorCollection>();

            PropertyDescriptorCollection val;
            if (ReflexionCachePF.TryGetValue(tpy, out val))
                return val;

            //MemberInfo[] props = tpy.GetMembers();
            //foreach (MemberInfo prop in props)
            //{
            //    if (prop is PropertyInfo || prop is FieldInfo)
            //        ret.Add(prop);
            //}
            var props = TypeDescriptor.GetProperties(tpy);
            ReflexionCachePF.Add(tpy, props);
            return props;
        }

        public static object GetValueFromPropertyOrField(this PropertyDescriptor prop, object x)
        {
            return prop.GetValue(x);
        }

        public static Type GetTypeFromPropertyOrField(this PropertyDescriptor prop)
        {
            return prop.PropertyType;
        }

        public static void SetValueFromPropertyOrField(this PropertyDescriptor prop, object x, object v)
        {
            prop.SetValue(x, v);
        }
    }
}