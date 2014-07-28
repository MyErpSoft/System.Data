using System.ComponentModel;
using System.Data.DataEntities.Metadata;
using System.Data.DataEntities.Metadata.Dynamic;

namespace System.Data.DataEntities.Dynamic
{
    /// <summary>
    /// Dynamic entity,can build dynamic type at runtime.
    /// </summary>
    public class DynamicEntity:
        Entity,  INotifyPropertyChanged
    {
        /// <summary>
        /// Creating a DynamicEntity designated by the DynamicEntityType information.
        /// </summary>
        /// <param name="dt">Type of DynamicEntity associated</param>
        public DynamicEntity(DynamicEntityType dt)
        {
            if (null == dt)
            {
                OrmUtility.ThrowArgumentNullException("dt");
            }
            _dt = dt;
            _values = new object[dt.Properties.Count];
        }

        internal readonly DynamicEntityType _dt;
        internal readonly object[] _values;

        /// <summary>
        /// Return type of DynamicEntity associated
        /// </summary>
        /// <returns>a IEntityType instance.</returns>
        public override IEntityType GetEntityType()
        {
            return _dt;
        }

        /// <summary>
        /// Get/Set property value, This method is the best performance, because the property is acquired by itself, so you can ignore the security check.
        /// </summary>
        /// <param name="index">To retrieve the index position, starting from 0</param>
        /// <returns>get/set property value.</returns>
        public object this[int index]
        {
            get { return _dt.Properties[index].GetValueCore(this); }
            set { _dt.Properties[index].SetValueCore(this, value); }
        }
    }
}
