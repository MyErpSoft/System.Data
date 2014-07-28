using System.Collections;
using System.Collections.Generic;
using System.Data.DataEntities.Metadata;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.DataEntities.DcxmlSerialization
{
    /// <summary>
    /// Deserialize reader tool.
    /// </summary>
    public sealed class DcxmlReader
    {
        private DcxmlBinder _binder;
        // While Dcxml not used to identify the object as long, but we can take advantage of. Net built these classes serialization and de-serialization mechanism to deal with. 
        // This will support. Net serialization and de-serialization mechanism for it.
        private ObjectIDGenerator _idGenerator;
        private ObjectManager _objectManager;

        /// <summary>
        /// Creating deserialize reader tool.
        /// </summary>
        /// <param name="binder">Type binder.</param>
        public DcxmlReader(DcxmlBinder binder)
        {
            this._binder = binder;

            Dcxml_Namespace = "http://www.myerpsoft.com/dcxml/2011";
            Dcxml_Action = Dcxml_Namespace.GetName("action");
            Dcxml_Oid = Dcxml_Namespace.GetName("oid");

            _idGenerator = new ObjectIDGenerator();
            _objectManager = new ObjectManager(null, new StreamingContext(StreamingContextStates.File));
        }

        /// <summary>
        /// Read entity data from XML stream. 
        /// </summary>
        /// <param name="baseType">Reference Type root entity object.</param>
        /// <param name="obj">Entity object can refer to if this instance will provide examples of superposition, if it is to provide examples to create a new instance.</param>
        /// <param name="xmlReader">XML stream reader</param>
        /// <returns>The newly created entity or entity after apply of XML data</returns>
        public object ReadObject(IEntityType baseType, object obj, XmlReader xmlReader) {
            #region Parameter Check
            if (null == xmlReader)
            {
                throw new ArgumentNullException("xmlReader");
            }
            #endregion

            XDocument doc = XDocument.Load(xmlReader);
            return ReadObject(baseType, obj, doc.Root);
        }

        /// <summary>
        /// From a start element, the read data to the entity.
        /// </summary>
        /// <param name="baseType">Reference Type root entity object.</param>
        /// <param name="obj">Entity object can refer to if this instance will provide examples of superposition, if it is to provide examples to create a new instance.</param>
        /// <param name="element">a start XML element </param>
        /// <returns>The newly created entity or entity after apply of XML data</returns>
        public object ReadObject(IEntityType baseType,object obj,  XElement element)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }

            //According to the node name, obtain its type
            IEnumerable<DcxmlBinder.CustomAttribute> attributes;
            var dt = BindToType(element, baseType, out attributes);

            //If there is no basis for the entity, directly create
            if (obj == null)
            {
                obj = _binder.CreateInstance(dt, attributes);
            }
            else
            {
                //If the current instance is consistent with the XML type, you can directly use, or create a new instance
                var currentObjType = _binder.GetEntityType(obj);
                if (!object.Equals(currentObjType,dt))
                {
                    obj = _binder.CreateInstance(dt, attributes);
                }
            }

            //register entity
            bool firstTime;
            var objectId = _idGenerator.GetId(obj, out firstTime);
            if (firstTime)
            {
                _objectManager.RegisterObject(obj, objectId);
                _objectManager.RaiseOnDeserializingEvent(obj);
            }

            var currentTypeNamespace = element.Name.Namespace;
            foreach (var propertyElement in element.Elements())
            {
                ReadProperty(dt, obj,currentTypeNamespace,propertyElement);
            }

            return obj;
        }

        /// <summary>
        /// According to an XML node to its name, its access to the entity type mapping.
        /// </summary>
        /// <param name="element">To read the element.</param>
        /// <param name="baseType">Alternative base type.</param>
        /// <param name="attributes">Additional attributes</param>
        /// <returns>Data type mapping.</returns>
        private IEntityType BindToType(XElement element,IEntityType baseType, out IEnumerable<DcxmlBinder.CustomAttribute> attributes)
        {
            IEntityType dt;
            if (TryBindFromBinder(element,out dt,out attributes) ||
                TryBindFromBase(element,baseType,out dt) ||
                TryBindFromEvent(element,baseType,out dt))
            {
                //Check the derived relationships are correct, that dt must be baseDt derived type.
                if ((baseType != null) && (!baseType.IsAssignableFrom(dt)))
                {
                    throw new ApplicationException("Design errors, the data type of the return is not derived from baseType");
                }
            }
            
            return dt;
        }

        private bool TryBindFromBinder(XElement element, out IEntityType dt, out IEnumerable<DcxmlBinder.CustomAttribute> attributes)
        {
            //The first-level policy
            var name = element.Name;
            attributes = from p in element.Attributes()
                         select new DcxmlBinder.CustomAttribute() { Namespace = p.Name.NamespaceName, Name = p.Name.LocalName, Value = p.Value };
            dt = _binder.BindToType(name.NamespaceName, name.LocalName,attributes);

            return (dt != null);
        }

        private bool TryBindFromBase(XElement element, IEntityType baseType, out IEntityType dt)
        {
            //The second-level policy: If not, see if there is a default type.
            if (baseType != null)
            {
                var name = element.Name;
                //This type of inspection is really just the name
                string n1, n2;
                _binder.BindToName(baseType, out n1, out n2);
                if (string.Equals(name.NamespaceName, n1, _binder.StringComparison) &&
                    string.Equals(name.LocalName, n2, _binder.StringComparison))
                {
                    //Not detect relationships derived directly returned.
                    dt = baseType;
                    return true;
                }
            }

            dt = null;
            return false;
        }

        private bool TryBindFromEvent(XElement element, IEntityType baseType, out IEntityType dt)
        {
            //Third-level policy
            //TODO: should first broadcast event System.Xml.Serialization.XmlElementEventHandler UnknownElement events. Gets the type of the outside world.
            throw new ApplicationException();
        }

        /// <summary>
        ///End deserialization process will trigger the end of the deserialization process.
        /// </summary>
        public void EndDeserialization()
        {
            _objectManager.RaiseDeserializationEvent();
        }

        #region Reading attributes

        private void ReadProperty(IEntityType dt, object obj, XNamespace currentTypeNamespace, XElement propertyElement)
        {
            //Only built and the current entity with a name attribute space.
            if (propertyElement.Name.Namespace == currentTypeNamespace)
            {
                string name = propertyElement.Name.LocalName;
                IEntityProperty property;
                if (dt.Properties.TryGetValue(name,out property))
                {
                    ReadSimpleProperty(property, obj, propertyElement);
                }
                else
                {
                    throw new SerializationException(string.Format("Could not find an XML description of the desired properties in the entity {0} {1}.", dt.FullName, propertyElement.Name.NamespaceName));
                }
            }
            else
            {
                //Let bindings bind to the attached object.
            }
        }

        private void ReadSimpleProperty(IEntityProperty property, object obj, XElement propertyElement)
        {
            ISimpleEntityProperty sp = property as ISimpleEntityProperty;
            if (sp != null)
            {
                var action = GetPropertyAction(propertyElement);
                switch (action)
                {
                    case PropertyActions.SetValue:
                        sp.SetValue(obj, sp.Converter.ConvertFromString(propertyElement.Value));
                        break;
                    case PropertyActions.Reset:
                        sp.ResetValue(obj);
                        break;
                    case PropertyActions.SetNull:
                        sp.SetValue(obj, null);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                ReadComplexProperty(property, obj, propertyElement);
            }
        }

        private void ReadComplexProperty(IEntityProperty property, object obj, XElement propertyElement)
        {
            IComplexEntityProperty cpx = property as IComplexEntityProperty;
            if (cpx != null)
            {
                var action = GetPropertyAction(propertyElement);
                switch (action)
                {
                    case PropertyActions.SetValue:
                        {
                            if (propertyElement.HasElements)
                            {
                                //Should have, and only one child node.
                                var child = propertyElement.Elements().First();
                                var oldValue = cpx.GetValue(obj);
                                var newValue = ReadObject(cpx.ComplexPropertyType, oldValue, child);
                                if (object.ReferenceEquals(oldValue,newValue))
                                {
                                    cpx.SetValue(obj, newValue);
                                }
                            }
                            break;
                        }
                    case PropertyActions.Reset:
                    case PropertyActions.SetNull:
                        cpx.SetValue(obj, null);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                ReadCollectionProperty(property, obj, propertyElement);
            }
        }

        private void ReadCollectionProperty(IEntityProperty property, object obj, XElement propertyElement)
        {
            ICollectionEntityProperty colp = property as ICollectionEntityProperty;
            if (colp != null)
            {
                IList list = colp.GetValue(obj) as IList;
                if (null == list)
                {
                    throw new ArgumentNullException("list");
                }
                foreach (var child in propertyElement.Elements())
                {
                    string oid;
                    var action = GetCollectionAction(child, out oid);
                    switch (action)
                    {
                        case CollectionActions.Add:
                            list.Add(ReadObject(colp.ItemPropertyType, null, child));
                            break;
                        case CollectionActions.Edit:
                            {
                                var findIndex = FindByPrimaryKey(colp.ItemPropertyType, list, oid);
                                var oldValue = list[findIndex];
                                var newValue = ReadObject(colp.ItemPropertyType, oldValue, child);
                                if (!object.ReferenceEquals(oldValue,newValue))
                                {
                                    list[findIndex] = newValue;
                                }
                                break;
                            }
                        case CollectionActions.Remove:
                            list.RemoveAt(FindByPrimaryKey(colp.ItemPropertyType, list, oid));
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private int FindByPrimaryKey(IEntityType itemType, IList list, string oid)
        {
            var pk = itemType.PrimaryKey;
            //TODO:Check whether the definition of a primary key
            var pkValue = pk.Converter.ConvertFromString(oid);
            object itemPkValue;

            for (int i = 0; i < list.Count; i++)
            {
                itemPkValue = pk.GetValue(list[0]);
                if (object.Equals(itemPkValue,pkValue))
                {
                    return i;
                }
            }

            throw new ApplicationException("The primary key is not found");
        }

        #endregion

        #region Get action in XML

        private XNamespace Dcxml_Namespace;
        private XName Dcxml_Action;
        private XName Dcxml_Oid;

        private PropertyActions GetPropertyAction(XElement propertyElement)
        {
            var att = propertyElement.Attribute(Dcxml_Action);
            if (att != null)
            {
                string action = att.Value;
                if (!string.IsNullOrEmpty(action))
                {
                    action = action.ToUpperInvariant();
                    if (action == "setvalue")
                    {
                        return PropertyActions.SetValue;
                    }
                    else if (action == "reset")
                    {
                        return PropertyActions.Reset;
                    }
                    else if (action == "setnull")
                    {
                        return PropertyActions.SetNull;
                    }
                }
            }
            return PropertyActions.SetValue;
        }

        private CollectionActions GetCollectionAction(XElement itemElement,out string oid)
        {
            CollectionActions result = CollectionActions.Add;
            var att = itemElement.Attribute(Dcxml_Action);
            if (att != null)
            {
                string action = att.Value;
                if (!string.IsNullOrEmpty(action))
                {
                    action = action.ToUpperInvariant();
                    if (action == "add")
                    {
                        result = CollectionActions.Add;
                    }
                    else if (action == "edit")
                    {
                        result = CollectionActions.Edit;
                    }
                    else if (action == "remove")
                    {
                        result = CollectionActions.Remove;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            if ((result == CollectionActions.Edit) || (result == CollectionActions.Remove))
            {
                att = itemElement.Attribute(Dcxml_Oid);
                if ((att == null) || (string.IsNullOrEmpty(att.Value)))
                {
                    throw new ApplicationException("To edit or delete the primary key is not specified.");
                }
                oid = att.Value;
            }
            else
            {
                oid = null;
            }

            return result;
        }

        #endregion
    }
}
