using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Cassandra.Data.Linq
{
    internal static class ReflExt
    {
        [ThreadStatic] private static Dictionary<Type, PropertyDescriptorCollection> ReflexionCacheP;
        [ThreadStatic]
        private static Dictionary<Type, List<MemberInfo>> ReflexionCachePF;

        public static PropertyDescriptorCollection GetEntityProperties(this Type tpy)
        {
            if (ReflexionCacheP == null)
                ReflexionCacheP = new Dictionary<Type, PropertyDescriptorCollection>();

            PropertyDescriptorCollection val;
            if (ReflexionCacheP.TryGetValue(tpy, out val))
                return val;

            var props = TypeDescriptor.GetProperties(tpy);
            ReflexionCacheP.Add(tpy, props);
            return props;
        }

        public static List<MemberInfo> GetPropertiesOrFields(this Type tpy)
        {
            if (ReflexionCachePF == null)
                ReflexionCachePF = new Dictionary<Type, List<MemberInfo>>();

            List<MemberInfo> val;
            if (ReflexionCachePF.TryGetValue(tpy, out val))
                return val;

            val = new List<MemberInfo>();
            MemberInfo[] props = tpy.GetMembers();
            foreach (MemberInfo prop in props)
            {
                if (prop is PropertyInfo || prop is FieldInfo)
                    val.Add(prop);
            }
            ReflexionCachePF.Add(tpy, val);
            return val;
        }

        public static object GetValueFromPropertyDescriptor(this PropertyDescriptor prop, object x)
        {
            return prop.GetValue(x);
        }

        public static object GetValueFromPropertyOrField(this MemberInfo prop, object x)
        {
            if (prop is PropertyInfo)
                return (prop as PropertyInfo).GetValue(x, null);
            if (prop is FieldInfo)
                return (prop as FieldInfo).GetValue(x);
            return null;
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