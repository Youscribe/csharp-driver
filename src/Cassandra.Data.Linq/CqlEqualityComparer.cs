using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Cassandra.Data.Linq
{
    internal class CqlEqualityComparer<TEntity> : IEqualityComparer<TEntity>
    {
        public static CqlEqualityComparer<TEntity> Default = new CqlEqualityComparer<TEntity>();

        public bool Equals(TEntity x, TEntity y)
        {
            var props = typeof(TEntity).GetEntityProperties();
            foreach (PropertyDescriptor prop in props)
            {
                var pk = prop.Attributes[typeof(PartitionKeyAttribute)] as PartitionKeyAttribute;
                if (pk != null)
                {
                    if (prop.GetValueFromPropertyDescriptor(x) == null)
                        throw new InvalidOperationException("Partition Key is not set");
                    if (prop.GetValueFromPropertyDescriptor(y) == null)
                        throw new InvalidOperationException("Partition Key is not set");
                    if (!prop.GetValueFromPropertyDescriptor(x).Equals(prop.GetValueFromPropertyDescriptor(y)))
                        return false;
                }
                else
                {
                    var rk = prop.Attributes[typeof(ClusteringKeyAttribute)] as ClusteringKeyAttribute;
                    if (rk != null)
                    {
                        if (prop.GetValueFromPropertyDescriptor(x) == null)
                            throw new InvalidOperationException("Clustering Key is not set");
                        if (prop.GetValueFromPropertyDescriptor(y) == null)
                            throw new InvalidOperationException("Clustering Key is not set");
                        if (!prop.GetValueFromPropertyDescriptor(x).Equals(prop.GetValueFromPropertyDescriptor(y)))
                            return false;
                    }
                }
            }
            return true;
        }

        public int GetHashCode(TEntity obj)
        {
            int hashCode = 0;
            var props = typeof(TEntity).GetEntityProperties();
            foreach (PropertyDescriptor prop in props)
            {
                var pk = prop.Attributes[typeof(PartitionKeyAttribute)];
                if (pk != null)
                {
                    if (prop.GetValueFromPropertyDescriptor(obj) == null)
                        throw new InvalidOperationException("Partition Key is not set");
                    hashCode ^= prop.GetValueFromPropertyDescriptor(obj).GetHashCode();
                }
                else
                {
                    var rk = prop.Attributes[typeof(ClusteringKeyAttribute)] as ClusteringKeyAttribute;
                    if (rk != null)
                    {
                        if (prop.GetValueFromPropertyDescriptor(obj) == null)
                            throw new InvalidOperationException("Clustering Key is not set");
                        hashCode ^= prop.GetValueFromPropertyDescriptor(obj).GetHashCode();
                    }
                }
            }
            return hashCode;
        }
    }
}