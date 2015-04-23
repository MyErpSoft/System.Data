using System.Data.Metadata.DataEntities.Dynamic;

namespace System.Data.DataEntities.Dynamic {


    public interface IDynamicEntityStorage {
        object GetValue(DynamicEntityField field);
        void SetValue(DynamicEntityField field, object value);
        void ClearValue(DynamicEntityField field);
    }
}
