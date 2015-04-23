using System.Collections.Generic;

namespace System.Data.Metadata.DataEntities.Dynamic {

    /// <summary>
    /// DynamicEntityField collection.
    /// </summary>
    public sealed class DynamicEntityFieldCollection :
        MetadataReadOnlyCollection<DynamicEntityField> {
        private readonly DynamicEntityType _reflectedType;

        internal DynamicEntityFieldCollection(DynamicEntityType reflectedType)
            : base(null) {
            _reflectedType = reflectedType;
        }

        internal DynamicEntityFieldCollection(DynamicEntityType reflectedType, IEnumerable<DynamicEntityField> fields)
            : base(fields) {
            _reflectedType = reflectedType;
            ResetOrdinal(0);
        }

        /// <summary>
        ///  extracts the name from the DynamicEntityField.
        /// </summary>
        /// <param name="item">DynamicEntityField instance</param>
        /// <returns>The DynamicEntityField's name</returns>
        protected override string GetName(DynamicEntityField item) {
            return item.Name;
        }

        /// <summary>
        /// When internal add element, update its index.
        /// </summary>
        /// <param name="item">add a element.</param>
        internal override void Add(DynamicEntityField item) {
            base.Add(item);
            item.Ordinal = this.Items.Length - 1;
            item.ReflectedType = _reflectedType;
        }

        /// <summary>
        /// When internal add element, update its index.
        /// </summary>
        /// <param name="items">add a elements.</param>
        internal override void AddRange(IEnumerable<DynamicEntityField> items) {
            int startIndex = this.Items.Length;
            base.AddRange(items);
            ResetOrdinal(startIndex);
        }

        private void ResetOrdinal(int startIndex) {
            var baseItems = this.Items;
            DynamicEntityField item;
            for (int i = startIndex; i < baseItems.Length; i++) {
                item = baseItems[i];
                item.Ordinal = i;
                item.ReflectedType = _reflectedType;
            }
        }
    }
}
