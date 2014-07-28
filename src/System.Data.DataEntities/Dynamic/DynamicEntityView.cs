using System.Collections.Generic;
using System.Data.DataEntities.Metadata.Dynamic;
using System.Linq.Expressions;

namespace System.Data.DataEntities.Dynamic {

    /// <summary>
    /// DynamicEntityView base class.
    /// </summary>
    public abstract class DynamicEntityView {

        /// <summary>
        /// Create DynamicEntityView instance.
        /// </summary>
        /// <param name="entity">DynamicEntity to be packaged</param>
        protected DynamicEntityView(DynamicEntity entity) {
            _entity = entity;
        }

        private DynamicEntity _entity;
        /// <summary>
        /// DynamicEntity to be packaged.
        /// </summary>
        public DynamicEntity Entity {
            get { return _entity; }
        }

        /// <summary>
        /// Creating a strongly typed view the collection.
        /// </summary>
        /// <typeparam name="TView">View class type.</typeparam>
        /// <param name="property">Collection property.</param>
        /// <returns>A strongly typed view the collection.</returns>
        protected DynamicEntityViewCollection<TView> CreateViewCollection<TView>(DynamicEntityProperty property) where TView : DynamicEntityView {
            var list = property.GetValue(this._entity) as IList<DynamicEntity>;
            return new DynamicEntityViewCollection<TView>(list);
        }

        /// <summary>
        /// Create a view instance.
        /// </summary>
        /// <typeparam name="TView">view class type.</typeparam>
        /// <param name="item">DynamicEntity to be packaged.</param>
        /// <returns>A new view instance.</returns>
        public static TView CreateView<TView>(DynamicEntity item) where TView : DynamicEntityView {
            Func<DynamicEntity, TView> createFunc = DynamicEntityViewCollection<TView>._createFunc;
            if (createFunc == null) {
                createFunc = DynamicCreateFunc<TView>(createFunc);
            }
            return createFunc(item);
        }

        private static Func<DynamicEntity, TView> DynamicCreateFunc<TView>(Func<DynamicEntity, TView> createFunc) where TView : DynamicEntityView {
            // = new View(item);
            var itemPar = Expression.Parameter(typeof(DynamicEntity), "item");
            var ctor = typeof(TView).GetConstructor(new Type[] { typeof(DynamicEntity) });
            var c1 = Expression.New(ctor, itemPar);
            createFunc = Expression.Lambda<Func<DynamicEntity, TView>>(c1, itemPar).Compile();
            DynamicEntityViewCollection<TView>._createFunc = createFunc;
            return createFunc;
        }
    }
}