using System.Collections.Generic;
using System.Data.DataEntities.Metadata;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.DataEntities.DcxmlSerialization
{
    /// <summary>
    /// Serialization and de-serialization tools for differential treatment.
    /// </summary>
    public class DcxmlSerializer
    {
        private readonly DcxmlBinder _binder;
        private readonly IEntityType _rootType;

        /// <summary>
        /// Creating DcxmlSerializer instance, the binding of the incoming type and root entity.
        /// </summary>
        /// <param name="binder">During serialization and de-serialization, the custom binder. If this parameter is not specified, you must provide <paramref name="rootEntityType"/></param>
        /// <param name="rootEntityType">When serialization and de-serialization, the corresponding root entity type, if you do not provide this parameter, you must provide <paramref name="binder"/></param>
        public DcxmlSerializer(DcxmlBinder binder = null, IEntityType rootEntityType = null)
        {
            _rootType = rootEntityType;

            if (binder == null)
            {
                var defBinder = new DefaultDcxmlBinder();
                if (rootEntityType != null)
                {
                    defBinder.EntityTypes.Add(rootEntityType);
                }
                binder = defBinder;
            }
            _binder = binder;
        }

        /// <summary>
        /// According to deserialize the XML string to an object.
        /// </summary>
        /// <param name="xml">xml string.</param>
        /// <param name="baseObject">The reference object to deserialize operation, if it is not provided, will create a new root entity object deserialization time.</param>
        /// <returns>a entity instance after apply xml.</returns>
        public object DeserializeFromString(string xml, object baseObject = null)
        {
            DcxmlReader reader = new DcxmlReader(_binder);
            XDocument doc = XDocument.Parse(xml);
            var obj = reader.ReadObject(_rootType, baseObject, doc.Root);
            reader.EndDeserialization();
            return obj;
        }

        /// <summary>
        /// According to deserialize the XML string to an object.
        /// </summary>
        /// <param name="xmlReaders">A group of the reader, a read operation is allowed.</param>
        /// <param name="baseObject">The reference object to deserialize operation, if it is not provided, will create a new root entity object deserialization time.</param>
        /// <returns>a entity instance after apply xml.</returns>
        public object Deserialize(IEnumerable<XmlReader> xmlReaders, object baseObject = null)
        {
            if (null == xmlReaders)
            {
                throw new ArgumentNullException("xmlReaders");
            }

            DcxmlReader reader = new DcxmlReader(_binder);
            object obj = baseObject;
            foreach (var xmlReader in xmlReaders)
            {
                XDocument doc = XDocument.Load(xmlReader);
                obj = reader.ReadObject(_rootType, obj, doc.Root);
            }
            reader.EndDeserialization();
            return obj;
        }
    }

}
