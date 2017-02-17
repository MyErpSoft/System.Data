using System.Collections.ObjectModel;

namespace System.Data.Metadata.Edm {

    public abstract class EntityTypeBase : StructuralType {
        private Collection<EdmProperty> _keyProperties;

        public Collection<EdmProperty> KeyProperties {
            get { return _keyProperties; }
        }

    }
}
