using System;
using System.Data.DataEntities.Metadata.Dynamic;

namespace System.Data.DataEntities.Dynamic {

    internal interface IDynamicEntityStorage {
        object GetValue(DynamicEntityField field);
        void SetValue(DynamicEntityField field, object value);
        void ClearValue(DynamicEntityField field);
    }
}
