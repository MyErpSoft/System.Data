using System;
using System.Data.DataEntities.Metadata.Dynamic;

namespace System.Data.DataEntities.Dynamic {

    /// <summary>
    /// Dynamic entity,can build dynamic type at runtime.
    /// </summary>
    public class DynamicEntity {
        public DynamicEntity(DynamicEntityType dt) {
            if (null == dt) {
                OrmUtility.ThrowArgumentNullException("dt");
            }

            this._storage = dt.InitializeEntity(this);
            this._dt = dt;
        }

        private readonly DynamicEntityType _dt;
        internal readonly IDynamicEntityStorage _storage;

        /// <summary>
        /// Return type of DynamicEntity associated
        /// </summary>
        /// <returns>a DynamicEntityType instance.</returns>
        public DynamicEntityType DynamicEntityType {
            get { return _dt; }
        }
    }
}