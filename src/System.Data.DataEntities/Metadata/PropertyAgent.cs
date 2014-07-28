
namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// For navigation property broker, used to describe the property.
    /// </summary>
    /// <typeparam name="TNavigator">The type of Navigator.</typeparam>
    /// <typeparam name="TElement">Element node type.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class PropertyAgent<TNavigator, TElement, TProperty>
        where TNavigator : PathNavigator<TNavigator, TElement, TProperty>
        where TElement : ElementAgent<TNavigator, TElement, TProperty>
        where TProperty : PropertyAgent<TNavigator, TElement, TProperty> {

        /// <summary>
        /// Derived classes need to incoming host information
        /// </summary>
        /// <param name="reflectedElement">Property corresponds to the host</param>
        protected PropertyAgent(TElement reflectedElement) {
            if (reflectedElement == null) {
                OrmUtility.ThrowArgumentNullException("reflectedElement");
            }
            this._reflectedElement = reflectedElement;
        }

        /// <summary>
        /// Returns the property type of relationship.
        /// </summary>
        public abstract PropertyRelationType RelationType { get; }

        private readonly TElement _reflectedElement;
        /// <summary>This property corresponds to the host element is returned.</summary>
        public TElement ReflectedElement {
            get { return _reflectedElement; }
        }

        /// <summary>
        /// If this is the type of relationship property returns a pointer to the target
        /// </summary>
        public abstract TElement RelationTo { get; }

        /// <summary>
        /// Returns the accessible name of this property.
        /// </summary>
        public abstract string Name { get; }
    }
}
