using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace System.Data.DataEntities.Metadata.Dynamic {
    /// <summary>
    /// Simple property descriptor.
    /// </summary>
    internal sealed class DynamicSimpleEntityProperty : DynamicEntityProperty, ISimpleEntityProperty {
        internal DynamicSimpleEntityProperty(
            string name,
            Type propertyType,
            bool isReadOnly,
            object defaultValue,
            object[] attributes)
            : base(name, propertyType, isReadOnly, ReGetDefaultValue(propertyType, defaultValue), attributes) {
        }

        private static object ReGetDefaultValue(Type propertyType, object defaultValue) {
            if (defaultValue == null) {
                if (propertyType.IsValueType) {
                    return Activator.CreateInstance(propertyType);
                }
            }
            else {
                Type valueType = defaultValue.GetType();
                if (!propertyType.IsAssignableFrom(valueType)) {
                    throw new ArgumentException("TODO:The default value type is not correct.");
                }
            }

            return defaultValue;
        }


        void ISimpleEntityProperty.ResetValue(object entity) {
            var dynamicEntity = GetDynamicEntity(entity);
            this.ResetValue(dynamicEntity);
        }

        bool ISimpleEntityProperty.ShouldSerializeValue(object entity) {
            var dynamicEntity = GetDynamicEntity(entity);
            return this.ShouldSerializeValueCore(dynamicEntity);
        }

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        public TypeConverter Converter {
            get { return TypeDescriptor.GetConverter(this.PropertyType); }
        }

    }
}
