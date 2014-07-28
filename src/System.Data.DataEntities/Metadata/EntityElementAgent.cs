using System.Data.DataEntities.Metadata.Dynamic;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// The navigation for an entity agent.
    /// </summary>
    /// <typeparam name="TNavigator">The type of Navigator.</typeparam>
    /// <typeparam name="TElement">Element node type.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class EntityElementAgent<TNavigator, TElement, TProperty> : ElementAgent<TNavigator, TElement, TProperty>
        where TNavigator : EntityPathNavigator<TNavigator, TElement, TProperty>
        where TElement : EntityElementAgent<TNavigator, TElement, TProperty>
        where TProperty : EntityPropertyAgent<TNavigator, TElement, TProperty> {

        /// <summary>
        /// By creating corresponding Entity element an IEntity type agent elements.
        /// </summary>
        /// <param name="navigator">Associated with the Navigator</param>
        /// <param name="dt">Entity type</param>
        /// <param name="parentProperty">Retrieved through what are the attributes of the parent element of this element.</param>
        protected EntityElementAgent(TNavigator navigator, IEntityType dt, TProperty parentProperty)
            : base(navigator) {
            if (dt == null) {
                OrmUtility.ThrowArgumentNullException("dt");
            }

            this._dt = dt;
            this._parentElementProperty = parentProperty;
        }

        /// <summary>Returns the immediate parent element of this element, such as retail sales under a single element, its Parent element points to the header. First single-Parent element to null.</summary>
        public TElement ParentElement {
            get { return _parentElementProperty == null ? null : _parentElementProperty.ReflectedElement; }
        }

        private readonly TProperty _parentElementProperty;
        /// <summary>Returns if this element is a child element, his element through navigation properties to this element</summary>
        public TProperty ParentElementProperty {
            get { return _parentElementProperty; }
        }

        private readonly IEntityType _dt;
        /// <summary> Returns the entity type of this element</summary>
        public IEntityType EntityType {
            get { return _dt; }
        }

        /// <summary>
        /// Returns the display name of this element.
        /// </summary>
        public override string Name {
            get { 
                //If this is the root element, and returns the entity name or add path.
                var current = this._parentElementProperty;
                var element = this;
                string result = null;
                while (current != null) {
                    result = Join(current.Name, result);
                    element = current.ReflectedElement;
                    current = element._parentElementProperty;
                }
                return Join(element._dt.FullName, result);
            }
        }

        private string _pathCache;
        /// <summary>
        /// Returns the element for root entities of a relative path.
        /// </summary>
        public string Path {
            get {
                if (this._pathCache == null) {
                    if (this._parentElementProperty == null) {
                        this._pathCache = string.Empty;
                    }
                    else {
                        this._pathCache = Join(this.ParentElement.Path, this._parentElementProperty.Name); 
                    }
                }
                return this._pathCache;
            }
        }

        private static string Join(string x, string y) {
            if (string.IsNullOrEmpty(x)) {
                return y;
            }
            if (string.IsNullOrEmpty(y)) {
                return x;
            }
            return x + "." + y;
        }

        /// <summary>
        /// Through the return corresponds to the name property.
        /// </summary>
        /// <param name="name">Name of the attribute to retrieve.</param>
        /// <param name="propertyAgent">Agent returns this property if found, otherwise null</param>
        /// <param name="errorMessage">If the property could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Find a property with this name.</returns>
        protected override bool TryGetPropertyCore(string name, out TProperty propertyAgent, out string errorMessage) {
            IEntityProperty property;
            if (!_dt.Properties.TryGetValue(name,out property)) {
                if (name == EntityPathNavigator.Keyword_Parent) {
                    if (_parentElementProperty != null) {
                        property = new ParentProperty(_parentElementProperty.ReflectedElement.EntityType);
                    }
                    else {
                        propertyAgent = default(TProperty);
                        errorMessage = Properties.Resources.ElementIsRootNotUseParent; //Object {0} is already the root entity, can no longer access the Parent property.
                        return false;
                    }
                }
                else {
                    propertyAgent = default(TProperty);
                    errorMessage = null; //Try get property will wrap: object {0} does not exist for the name property of the {1}.
                    return false;
                }
            }

            errorMessage = null;
            propertyAgent = this.CreateProperty(property);
            return true;
        }

        /// <summary>
        /// Derived classes need to override this method in order to create a new attribute.
        /// </summary>
        /// <param name="entityProperty">Corresponding entity property.</param>
        /// <returns>The property broker.</returns>
        protected abstract TProperty CreateProperty(IEntityProperty entityProperty);

        #region ParentProperty
        private sealed class ParentProperty : DynamicMemberMetadata, IComplexEntityProperty {
            public ParentProperty(IEntityType propertyType) {
                this._dt = propertyType;
            }

            public override string Name {
                get { return EntityPathNavigator.Keyword_Parent; }
            }

            public override object[] GetCustomAttributes(bool inherit) {
                return new object[0];
            }

            #region IComplexEntityProperty 
            private readonly IEntityType _dt;
            public IEntityType ComplexPropertyType {
                get { return _dt; }
            }

            #endregion

            #region IEntityProperty 

            public object GetValue(object entity) {
                return ((IObjectWithParent)entity).Parent;
            }

            public void SetValue(object entity, object newValue) {
                throw new NotSupportedException();
            }

            public bool IsReadOnly {
                get { return true; }
            }

            public Type PropertyType {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }
        #endregion
    
    }

    /// <summary>
    /// Entity present navigational elements that you can use a proxy
    /// </summary>
    public sealed class EntityElementAgent : EntityElementAgent<EntityPathNavigator, EntityElementAgent, EntityPropertyAgent> {

        /// <summary>
        /// By creating corresponding Entity element an IEntity type agent elements.
        /// </summary>
        /// <param name="navigator">Associated with the Navigator</param>
        /// <param name="dt">Entity type</param>
        /// <param name="parentProperty">Retrieved through what are the attributes of the parent element of this element.</param>
        internal EntityElementAgent(EntityPathNavigator navigator, IEntityType dt, EntityPropertyAgent parentProperty)
            : base(navigator,dt,parentProperty) {  }

        /// <summary>
        /// Creates a new attribute
        /// </summary>
        /// <param name="entityProperty">Correspond to entity properties</param>
        /// <returns>The property broker.</returns>
        protected override EntityPropertyAgent CreateProperty(IEntityProperty entityProperty) {
            return new EntityPropertyAgent(this, entityProperty);
        }
    }
}
