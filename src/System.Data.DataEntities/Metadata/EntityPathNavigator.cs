using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// A specific entity.
    /// </summary>
    /// <typeparam name="TNavigator">The type of Navigator.</typeparam>
    /// <typeparam name="TElement">Element node type.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class EntityPathNavigator<TNavigator, TElement, TProperty> : PathNavigator<TNavigator, TElement, TProperty>
        where TNavigator : EntityPathNavigator<TNavigator, TElement, TProperty>
        where TElement : EntityElementAgent<TNavigator, TElement, TProperty>
        where TProperty : EntityPropertyAgent<TNavigator, TElement, TProperty> {
        internal const string Keyword_Parent = "Parent";

        /// <summary>
        /// Used to delegate that returns an entity type
        /// </summary>
        /// <param name="name">To obtain the name of the entity type.</param>
        /// <param name="dt">Results of the entity type.</param>
        /// <returns>Return true if found</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public delegate bool TryGetEntityTypeHandler(string name, out  IEntityType dt);

        /// <summary>
        /// Create Entity instance path Navigator 
        /// </summary>
        /// <param name="tryGetTypeHandler">Used to obtain the type of the delegate.</param>
        protected EntityPathNavigator(TryGetEntityTypeHandler tryGetTypeHandler) {
            if (tryGetTypeHandler == null) {
                OrmUtility.ThrowArgumentNullException("tryGetTypeHandler");
            }
            this._tryGetTypeHandler = tryGetTypeHandler;
        }

        readonly TryGetEntityTypeHandler _tryGetTypeHandler;

        /// <summary>
        /// By a name corresponding to the element.
        /// </summary>
        /// <param name="name">To access the element name</param>
        /// <param name="element">If you find this element, and returns its elements, otherwise it returns null</param>
        /// <param name="errorMessage">If the entity could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Returns whether this element is successfully found.</returns>
        protected override bool TryGetElementCore(string name, out TElement element, out string errorMessage) {
            if (string.IsNullOrEmpty(name)) {
                element = default(TElement);
                errorMessage = Properties.Resources.NameIsEmpty;
                return false;
            }

            IEntityType dt;
            //Either level or multi-level first tries to external queries, probably the current name is namespace name, so please try to address the external and then split.
            if (this._tryGetTypeHandler(name, out dt)) {
                element = this.CreateElement(dt, null);
                errorMessage = null;
                return true;
            }

            var names = Split(name);
            //Only one-level data
            if (names.Length == 1) {
                element = null;
                errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.NotFindElement, name);
                return false;
            }

            //Multiple Element needed to build hierarchical relationships (recursive calls). Note call is: TryGetElement rather than TryGetElementCore, so you can cache.
            //T1.R1.R2.R3   Dismantling T1.R1.R2/R3 ,Then T1.R1.R2 Split T1.R1/R2,And then split T1/R1 And then split T1
            string one = string.Join(".", names, 0, names.Length - 1);
            string two = names[names.Length - 1];

            if (this.TryGetElement(one, out element, out errorMessage) &&                   // Find top level elements
                this.TryCreateSubElement(element, two, out element, out errorMessage)) {    // A specified element, visit his relations
                return true; 
            }

            return false;
        }

        /// <summary>
        /// Created by an IEntity type corresponding to the element.
        /// </summary>
        /// <param name="dt">Entity type</param>
        /// <param name="parentProperty">Retrieved through what are the attributes of the parent element of this element.</param>
        /// <returns>Instantiate the element</returns>
        protected abstract TElement CreateElement(IEntityType dt, TProperty parentProperty);

        //Known to be an element, create a collection property corresponds to the element this element
        private bool TryCreateSubElement(TElement element, string propertyName, out TElement result,out string errorMessage) {
            result = null;
            //Attempting to access this property
            TProperty propertyAgent;
            if (!element.TryGetProperty(propertyName, out propertyAgent,out errorMessage)) {
                return false;//Does not have this property.
            }

            //Parent cannot occur, 
            //Must be a pointer to an relation.
            //Must be in their respective entities, can no longer relate to the outside.
            if (propertyAgent.Name == Keyword_Parent) {
                errorMessage = Properties.Resources.ElementExistParent;
                return false;
            }
            if (propertyAgent.RelationType == PropertyRelationType.Simple) {
                errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ElementExistSimpleProperty, propertyAgent.Name);
                return false;
            }
            if (propertyAgent.IsExternalRelation) {
                errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ElementExistExternalRelation, propertyAgent.Name);
                return false;
            }

            //Note using:propertyAgent.RelationToEntityType，Not is propertyAgent.RelationTo, Otherwise it will cause an infinite loop
            //propertyAgent.RelationTo In practice this method is called.
            result = CreateElement(propertyAgent.RelationToEntityType, propertyAgent);
            return true;
        }

        private static string[] Split(string path) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return path.Split('.');
        }
    }


    /// <summary>
    /// Non-generic instantiated Entity path Navigator
    /// </summary>
    public sealed class EntityPathNavigator : EntityPathNavigator<EntityPathNavigator, EntityElementAgent, EntityPropertyAgent> {
        /// <summary>
        /// Create Entity instance path Navigator 
        /// </summary>
        /// <param name="tryGetTypeHandler">Used to obtain the type of the delegate.</param>
        public EntityPathNavigator(TryGetEntityTypeHandler tryGetTypeHandler) : base(tryGetTypeHandler) { }

        /// <summary>
        /// By creating corresponding Entity element an IEntity type agent elements.
        /// </summary>
        /// <param name="dt">Entity type</param>
        /// <param name="parentProperty">Retrieved through what are the attributes of the parent element of this element.</param>
        /// <returns>Instantiate the element</returns>
        protected override EntityElementAgent CreateElement(IEntityType dt,EntityPropertyAgent parentProperty) {
            return new EntityElementAgent(this, dt, parentProperty);
        }
    }
   
}
