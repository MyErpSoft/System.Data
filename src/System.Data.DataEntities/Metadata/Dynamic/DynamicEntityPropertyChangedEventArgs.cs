using System;
using System.ComponentModel;

namespace System.Data.DataEntities.Metadata.Dynamic {
    internal sealed class DynamicEntityPropertyChangedEventArgs : PropertyChangedEventArgs {
        private DynamicEntityProperty dynamicEntityProperty;

        public DynamicEntityPropertyChangedEventArgs(DynamicEntityProperty dynamicEntityProperty)
            : base(null) {
            this.dynamicEntityProperty = dynamicEntityProperty;
        }

        public override string PropertyName {
            get {
                return dynamicEntityProperty.Name;
            }
        }
    }
}
