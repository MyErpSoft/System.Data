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
            _dt = dt;
            if (dt.Fields.Count < 30) {
                this._storage = new DynamicEntityArrayStorage(dt);
            }
            else {
                this._storage = new DynamicEntityDictStorage();
            }
        }

        private readonly DynamicEntityType _dt;
        internal IDynamicEntityStorage _storage;

        /// <summary>
        /// Return type of DynamicEntity associated
        /// </summary>
        /// <returns>a DynamicEntityType instance.</returns>
        public DynamicEntityType DynamicEntityType {
            get { return _dt; }
        }
    }
}