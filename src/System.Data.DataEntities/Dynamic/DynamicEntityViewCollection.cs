using System.Linq.Expressions;
using System.Collections.Generic;

namespace System.Data.DataEntities.Dynamic {

    /// <summary>
    /// Strongly typed view the collection
    /// </summary>
    /// <typeparam name="TView">View class type.</typeparam>
    public class DynamicEntityViewCollection<TView> 
        : ViewCollection<TView,DynamicEntity> 
        where TView : DynamicEntityView {

        /// <summary>
        /// Create DynamicEntityViewCollection instance.
        /// </summary>
        /// <param name="items">Entity list.</param>
        public DynamicEntityViewCollection(IList<DynamicEntity> items):base(items) {
        }

        internal static Func<DynamicEntity, TView> _createFunc;
        /// <summary>
        /// Create a view instance from entity instance.
        /// </summary>
        /// <param name="item">DynamicEntity to be packaged</param>
        /// <returns>A new view instance.</returns>
        protected override TView CreateView(DynamicEntity item) {
            return DynamicEntityView.CreateView<TView>(item);
        }

        /// <summary>
        /// Get a DynamicEntity instance from view.
        /// </summary>
        /// <param name="view">a view instance.</param>
        /// <returns>a view instance.</returns>
        protected override DynamicEntity GetItem(TView view) {
            if (view == null) {
                OrmUtility.ThrowArgumentNullException("view");
            }
            return view.Entity;
        }
    }
}
