using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Globalization;
using System.Linq;

namespace System.Data.DataEntities.Metadata.Dynamic {
    /// <summary>
    /// Dynamic entity type.
    /// </summary>
    public sealed class DynamicEntityType : DynamicMemberMetadata, IEntityType {
        /// <summary>
        /// Dynamic entity types, allows creating a type at run time and used to carry the physical structure of the data.
        /// </summary>
        /// <param name="name">The identifying name of the type, following the c# property name constraints.</param>
        /// <param name="nameSpace">The dynamic type's namespace.</param>
        /// <param name="baseType">Can be specified for this dynamic type derived relationship.</param>
        /// <param name="flag">This type of dynamic feature flags, for example is a class or an interface.</param>
        /// <param name="attributes">Custom Attributes array.</param>
        public DynamicEntityType(
            string name,
            string nameSpace = null,
            DynamicEntityType baseType = null,
            DynamicEntityTypeFlag flag = DynamicEntityTypeFlag.Class,
            params object[] attributes) {
            #region Parameter checking
            if (!OrmUtility.VerifyName(name)) {
                throw new ArgumentException("name");
            }
            if ((!string.IsNullOrEmpty(nameSpace)) &&
                !OrmUtility.VerifyNameWithNamespace(nameSpace)) {
                throw new ArgumentException("nameSpace");
            }
            #endregion

            _name = name;
            _namespace = nameSpace;
            _baseType = baseType; //TODO:Validate the base class cannot be circular references cannot be an interface. Interfaces cannot have base classes.
            _flag = flag;
            _attributes = attributes;

            //Import the properties of the base class.
            if (null == _baseType) {
                _properties = new DynamicEntityPropertyCollection(this);
                _interfaces = _emptyTypes;
            }
            else {
                //Need to clone the properties of the base class descriptor.
                _properties = new DynamicEntityPropertyCollection(this,
                    from DynamicEntityProperty p in _baseType._properties select p.Clone());
                _interfaces = _baseType._interfaces;
                _interfacePropertyMaps = _baseType._interfacePropertyMaps;
            }
            _shouldRenewMaps = true;
        }

        #region about name，ex Name,Namespcae,FullName

        private readonly string _name;
        /// <summary>
        /// The identifying name of the type
        /// </summary>
        public override string Name {
            get { return _name; }
        }

        private readonly string _namespace;
        /// <summary>
        ///The namespace of the type
        /// </summary>
        public string Namespace { get { return _namespace; } }

        /// <summary>
        /// The fullname of the type
        /// </summary>
        public string FullName {
            get { return string.IsNullOrEmpty(_namespace) ? _name : string.Concat(_namespace, ".", _name); }
        }

        #endregion

        #region Features, such as abstraction, and so on.

        /// <summary>
        /// Gets a value, this value indicates whether the entity type an abstract.
        /// </summary>
        public bool IsAbstract { get { return _flag == DynamicEntityTypeFlag.Abstract; } }

        /// <summary>
        /// Gets a value, this value indicates whether the entity type a sealed.
        /// </summary>
        public bool IsSealed { get { return _flag == DynamicEntityTypeFlag.Sealed; } }

        private readonly DynamicEntityTypeFlag _flag;
        /// <summary>
        /// Gets a value, this value indicates whether the entity type a interface.
        /// </summary>
        public bool IsInterface {
            get { return _flag == DynamicEntityTypeFlag.Interface; }
        }

        #endregion

        #region About Attributes

        private object[] _attributes;
        /// <summary>
        /// Returns all the custom attributes defined on this type of arrays, or if there are no custom attributes, return an empty array.
        /// </summary>
        /// <param name="inherit">When true, and find inherited custom attribute hierarchy chain.</param>
        /// <returns>Represents a custom property of an array of objects, or an empty array.</returns>
        public override object[] GetCustomAttributes(bool inherit) {
            if (inherit && (_baseType != null)) {
                var next = this;
                List<object> list = new List<object>();
                do {
                    if (next._attributes != null) {
                        list.AddRange(next._attributes);
                    }
                    next = next._baseType;
                } while (next != null);

                return list.ToArray();
            }
            else {
                return _attributes ?? EmptyAttributes;
            }
        }

        #endregion

        private readonly ISimpleEntityProperty _primaryKey;
        /// <summary>
        /// Returns the primary key for this EntityType.
        /// </summary>
        public ISimpleEntityProperty PrimaryKey {
            get { return _primaryKey; }
        }

        private readonly DynamicEntityPropertyCollection _properties;
        /// <summary>
        /// Return all properties for this EntityType.
        /// </summary>
        public DynamicEntityPropertyCollection Properties { get { return _properties; } }

        IEntityPropertyCollection IEntityType.Properties { get { return _properties; } }

        /// <summary>
        /// Create an instance of this EntityType
        /// </summary>
        /// <returns></returns>
        public object CreateInstance() {
            return new DynamicEntity(this);
        }

        #region Derived base classes and interfaces

        private bool _isFrozen;
        /// <summary>
        /// Gets a value that indicates whether this dynamic entity types can be modified.
        /// </summary>
        public bool IsFrozen {
            get { return _isFrozen; }
        }

        /// <summary>
        /// Current dynamic entity type unmodifiable and sets its IsFrozen property to true.
        /// </summary>
        public void Freeze() {
            _isFrozen = true;
        }

        private readonly DynamicEntityType _baseType;
        /// <summary>
        /// Gets the type from which the current type directly inherits.
        /// </summary>
        public DynamicEntityType BaseType {
            get { return _baseType; }
        }

        IEntityType IEntityType.BaseType { get { return _baseType; } }

        private static readonly DynamicEntityType[] _emptyTypes = new DynamicEntityType[0];
        //This type supports an array of all interfaces
        //Result after these interfaces are already underway, such as:interface IA ： IB， class A : IA，its class a consisted of these two interfaces.
        private DynamicEntityType[] _interfaces;
        /// <summary>
        /// Returns the entity type supported interfaces
        /// </summary>
        /// <returns>the entity type supported interfaces</returns>
        public IEntityType[] GetInterfaces() {
            IEntityType[] array = new IEntityType[_interfaces.Length];
            _interfaces.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Determines whether the current instance of an DynamicEntityType can be assigned from the specified DynamicEntityType instance.
        /// </summary>
        /// <param name="c">Compare with the current DynamicEntityType an DynamicEntityType.</param>
        /// <returns>
        /// If any of the following conditions are true:
        ///     if c and the current Type represent the same type, 
        ///     or if the current Type is in the inheritance hierarchy of c,
        ///     or if the current Type is an interface that c implements,
        ///  false if none of these conditions are true, or if c is null.
        /// </returns>
        public bool IsAssignableFrom(DynamicEntityType c) {
            if (null == c) {
                return false;
            }

            var current = c;
            int thisToken = this.MetadataToken;
            do {
                if (current.MetadataToken == thisToken) {
                    return true;
                }
                current = current._baseType;
            } while (null != current);

            if ((this.IsInterface) && (c._interfaces != null)) {
                foreach (var i in c._interfaces) {
                    if (i.MetadataToken == thisToken) {
                        return true;
                    }
                }
            }

            return false;
        }

        bool IEntityType.IsAssignableFrom(IEntityType c) {
            return this.IsAssignableFrom(c as DynamicEntityType);
        }

        /// <summary>
        /// Determines whether the DynamicEntityType represented by the current DynamicEntityType derives from the DynamicEntityType represented by the specified DynamicEntityType.
        /// </summary>
        /// <param name="c">The DynamicEntityType to compare with the current DynamicEntityType.</param>
        /// <returns>
        /// true if the Type represented by the c parameter and the current Type represent classes, and the class represented by the current Type derives from the class represented by c; otherwise, false. This method also returns false if c and the current Type represent the same class.
        /// </returns>
        public bool IsSubclassOf(DynamicEntityType c) {
            var current = this._baseType;
            int cToken = c.MetadataToken;

            while (null == current) {
                if (current.MetadataToken == cToken) {
                    return true;
                }
                current = current._baseType;
            }

            return false;
        }

        /// <summary>
        /// Detect whether the current type supports the specified interface.
        /// </summary>
        /// <param name="interfaceType">To check for interface</param>
        /// <returns>If the current type that supports this interface returns true, otherwise returns false.</returns>
        public bool IsSupportedInterface(DynamicEntityType interfaceType) {
            if (interfaceType != null) {
                DynamicEntityType item;
                var token = interfaceType.MetadataToken;
                for (int i = 0; i < _interfaces.Length; i++) {
                    item = _interfaces[i];
                    if (item.MetadataToken == token) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>A data structure that stores all the interface mapping locations on the property in the current type. Key of this type is the MetadataToken,Value interface property is indexed.</summary>
        private Dictionary<int, int> _interfacePropertyMaps;
        //Contains the base classes, will reuse the mapping tables for the base class, unless the derived class supports the new interface.
        private bool _shouldRenewMaps;

        /// <summary>
        /// Returns an interface property corresponds to the implementations of this type of property.
        /// </summary>
        internal DynamicEntityProperty GetInterfacePropertyMap(DynamicEntityProperty property) {
            System.Diagnostics.Debug.Assert((property.ReflectedType != null) && property.ReflectedType.IsInterface);

            int index;
            if ((_interfacePropertyMaps != null) &&
                (_interfacePropertyMaps.TryGetValue(property.MetadataToken, out index))) {
                return this._properties[index];
            }

            throw new NotSupportedException(string.Format("Type {0} does not support interface {1} {2} and its members.",
                this.FullName, property.ReflectedType.FullName, property.Name));
        }
        #endregion

        #region Properties Interface and register
        private static void CheckIsFrozen(DynamicEntityType dt) {
            if (!dt.IsFrozen) {
                throw new ArgumentException(
                    string.Format("To inherit a base class or interface that supports must be frozen, please call the Freeze method {0}.", dt.FullName));
            }
        }

        private void CheckNotIsFrozen() {
            if (this.IsFrozen) {
                throw new ArgumentException(
                    string.Format("DynamicEntityType {0} is frozen cannot change again.", this.FullName));
            }
        }

        /// <summary>
        /// Registered with the current dynamic entity type is a simple property.
        /// </summary>
        /// <param name="name">Unique name for this simple property, duplicate names cannot be combined with other properties.</param>
        /// <param name="propertyType">The return type of the property.</param>
        /// <param name="isReadonly">set this property is a read-only property if value is true.</param>
        /// <param name="defaultValue">set this property default value when not set value.</param>
        /// <param name="attributes">Custom attributes</param>
        /// <returns>A new property</returns>
        public DynamicEntityProperty RegisterSimpleProperty(
            string name, Type propertyType,
            bool isReadonly = false,
            object defaultValue = null,
            params object[] attributes) {
            
            this.CheckNotIsFrozen();
            if (!OrmUtility.VerifyName(name)) {
                throw new ArgumentException(string.Format("Register a simple property name {0} is not correct, can only be a combination of letters, numbers, and underscores.", name));
            }

            //TODO:Check parameters
            DynamicSimpleEntityProperty property = new DynamicSimpleEntityProperty(
                name,
                propertyType,
                isReadonly,
                defaultValue,
                attributes);

            _properties.Add(property);

            return property;
        }

        /// <summary>
        /// Registered with the current dynamic entity type is a collection property.
        /// </summary>
        /// <param name="name">Unique name for this collection property, duplicate names cannot be combined with other properties.</param>
        /// <param name="itemType">The item return type of the property.</param>
        /// <param name="attributes">Custom attributes</param>
        /// <returns>A new property</returns>
        public DynamicEntityProperty RegisterCollectionProperty(
            string name, DynamicEntityType itemType,
            params object[] attributes) {

            this.CheckNotIsFrozen();
            if (!OrmUtility.VerifyName(name)) {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Register a property name {0} is not correct, can only be a combination of letters, numbers, and underscores.", name));
            }

            //TODO:Check parameters
            DynamicCollectionEntityProperty property = new DynamicCollectionEntityProperty(
                name, itemType, attributes);

            _properties.Add(property);

            return property;
        }

        /// <summary>
        /// Add support for an interface to an entity type.
        /// </summary>
        /// <param name="dt">The interface definition to support.</param>
        public void RegisterInterface(
            DynamicEntityType dt) {

            if (null == dt) {
                throw new ArgumentNullException("dt");
            }

            if (!dt.IsInterface) {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Registered type {0} must be an interface.", dt.FullName));
            }

            CheckIsFrozen(dt);

            //TODO:Loop interface support check.

            if (IsSupportedInterface(dt)) {
                return;
            }

            if (dt._properties.Count > 0) {
                //Gets all the property to check for already defined on this type and return type.
                var propertyMaps = (from DynamicEntityProperty property in dt._properties
                                    select new {
                                        //Interface property
                                        Source = property,
                                        //This type implements the property.
                                        Target = GetOrCreatePropertyMap(dt, property)
                                    }).ToArray();

                var newProperties = (from p in propertyMaps where p.Target.Item2 == 2 select p.Target.Item1).ToArray();
                _properties.AddRange(newProperties);

                //Interface mapping tables when this type without extra support other interfaces (that is, no increase of the base interfaces supported by the class), are not going to clone a copy of _interfacePropertyMaps
                //In order to save performance and memory when starting support for new interface, you need a copy, in order to isolate.
                if (_shouldRenewMaps) {
                    if (_interfacePropertyMaps == null) {
                        _interfacePropertyMaps = new Dictionary<int, int>(newProperties.Length);
                    }
                    else {
                        _interfacePropertyMaps = new Dictionary<int, int>(_interfacePropertyMaps);
                    }

                    _shouldRenewMaps = false;
                }

                foreach (var map in propertyMaps) {
                    //Members of this interface has already been mapped once.
                    if (map.Target.Item2 != 0) {
                        _interfacePropertyMaps.Add(map.Source.MetadataToken, map.Target.Item1.Ordinal);
                    }
                }
            }

            //The registration interface is added to the list of the current interface.
            var count = dt._interfaces.Length + 1;
            var tmp = new KeyValuePair<DynamicEntityType, bool>[count];
            var shouldAddCount = 0;

            for (int i = 0; i < dt._interfaces.Length; i++) {
                if (!IsSupportedInterface(dt._interfaces[i])) {
                    tmp[i] = new KeyValuePair<DynamicEntityType, bool>(dt._interfaces[i], true);
                    shouldAddCount++;
                }
            }

            tmp[count - 1] = new KeyValuePair<DynamicEntityType, bool>(dt, true);
            shouldAddCount++;

            var newInterfaces = new DynamicEntityType[_interfaces.Length + shouldAddCount];
            _interfaces.CopyTo(newInterfaces, 0);

            int index = _interfaces.Length;
            for (int i = 0; i < count; i++) {
                if (tmp[i].Value) {
                    newInterfaces[index] = tmp[i].Key;
                    index++;
                }
            }

            _interfaces = newInterfaces;
        }

        /// <summary>
        /// Check the map on an interface member in the current type. If there is no mapping will create a new (but not formally added).
        /// </summary>
        /// <param name="interfaceType">interface</param>
        /// <param name="property">interface property</param>
        /// <returns>Item1 is mapped to the property of this type, there may be new. Item2=0 says it has mapped, =1 no mapping can support, =2 said it would take the new property.</returns>
        private Tuple<DynamicEntityProperty, int> GetOrCreatePropertyMap(DynamicEntityType interfaceType, DynamicEntityProperty property) {
            DynamicEntityProperty find;
            int index;

            //If this property has been achieved in the current type, no need to find maps.
            //When Register interface registers a current class does not support the interface, but also contains a b interface and the current class b interfaces are supported, scan b interface members, this situation will occur.
            if ((_interfacePropertyMaps != null) &&
                (_interfacePropertyMaps.TryGetValue(property.MetadataToken, out index))) {
                return new Tuple<DynamicEntityProperty, int>(_properties[index], 0);
            }

            //Member of the interface in the currently registered types.
            if (_properties.TryGetValue(property.Name, out find)) {
                if (find.PropertyType != property.PropertyType) {
                    throw new InvalidOperationException(
                        string.Format("To register interface {0} at {1}, property on an interface '{2}:{3}' is inconsistent with the type on an existing EntityType {4} .",
                        this.FullName, interfaceType.FullName, property.Name, property.PropertyType.FullName, find.PropertyType.FullName));

                }

                if (!property.IsReadOnly && find.IsReadOnly) {
                    throw new InvalidOperationException(
                        string.Format("To register interface {0} at {1}, found its properties on an interface {2}:{3} is read/write, but on an existing EntityType is read-only.",
                        this.FullName, interfaceType.FullName, property.Name, property.PropertyType.FullName));
                }

                //Find maps
                return new Tuple<DynamicEntityProperty, int>(find, 1);
            }
            else {
                //Not found, will be created automatically, the current Token has not changed.
                var newProperty = property.Clone();
                return new Tuple<DynamicEntityProperty, int>(newProperty, 2);
            }
        }
        #endregion
    }
}
