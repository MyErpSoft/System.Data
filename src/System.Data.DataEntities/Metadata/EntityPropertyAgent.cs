using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// Navigator about the properties of the agent.
    /// </summary>
    /// <typeparam name="TNavigator">The type of Navigator.</typeparam>
    /// <typeparam name="TElement">Element node type.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class EntityPropertyAgent<TNavigator, TElement, TProperty> : PropertyAgent<TNavigator, TElement, TProperty>
        where TNavigator : EntityPathNavigator<TNavigator, TElement, TProperty>
        where TElement : EntityElementAgent<TNavigator, TElement, TProperty>
        where TProperty : EntityPropertyAgent<TNavigator, TElement, TProperty> {

        /// <summary>
        /// Derived classes need to incoming host information
        /// </summary>
        /// <param name="reflectedElement">Property corresponds to the host</param>
        /// <param name="property">Entity attributes.</param>
        protected EntityPropertyAgent(TElement reflectedElement, IEntityProperty property)
            : base(reflectedElement) {
            this._entityProperty = property;
        }

        /// <summary>Returns the relationship corresponds to the entity type</summary>
        public IEntityType RelationToEntityType {
            get { return this.GetEntityRelationTo(); }
        }

        private readonly IEntityProperty _entityProperty;
        /// <summary>Returns a property of an entity</summary>
        public IEntityProperty EntityProperty {
            get { return _entityProperty; }
        }

        /// <summary>
        /// Returns the name of the entity property.
        /// </summary>
        public override string Name {
            get { return _entityProperty.Name; }
        }

        /// <summary>
        /// If it is a relational property, returns a pointer to the target
        /// </summary>
        public override TElement RelationTo {
            get {
                if (this.RelationType != PropertyRelationType.Simple) {
                    if (this.Name == EntityPathNavigator.Keyword_Parent) {
                        return this.ReflectedElement.ParentElement;
                    }
                    else {
                        string elementName = this.ReflectedElement.Name + "." + this.Name;
                        return this.ReflectedElement.Navigator.GetElement(elementName);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the property type of relationship.
        /// </summary>
        public override PropertyRelationType RelationType {
            get {
                if (_entityProperty is ISimpleEntityProperty) {
                    return PropertyRelationType.Simple;
                }
                if (_entityProperty is ICollectionEntityProperty) {
                    return PropertyRelationType.Collection;
                }
                if (_entityProperty is IComplexEntityProperty) {
                    return PropertyRelationType.Complex;
                }

                return PropertyRelationType.Simple;
            }
        }

        /// <summary>
        /// If it's a relationship, returns whether this relationship is an external relations.
        /// </summary>
        public virtual bool IsExternalRelation {
            get { return false; }
        }

        private IEntityType GetEntityRelationTo() {
            ISimpleEntityProperty sp = _entityProperty as ISimpleEntityProperty;
            if (sp != null) {
                return null;
            }

            ICollectionEntityProperty colp = _entityProperty as ICollectionEntityProperty;
            if (colp != null) {
                return colp.ItemPropertyType;
            }

            IComplexEntityProperty cpx = _entityProperty as IComplexEntityProperty;
            if (cpx != null) {
                return cpx.ComplexPropertyType;
            }

            return null;
        }
    }

    /// <summary>
    /// Properties for entity navigation
    /// </summary>
    public sealed class EntityPropertyAgent : EntityPropertyAgent<EntityPathNavigator, EntityElementAgent, EntityPropertyAgent> {
        /// <summary>
        /// Create EntityPropertyAgent instance.
        /// </summary>
        /// <param name="reflectedElement">Property corresponds to the host</param>
        /// <param name="property">Entity attributes.</param>
        internal EntityPropertyAgent(EntityElementAgent reflectedElement, IEntityProperty property)
            : base(reflectedElement, property) { }
    }
}
