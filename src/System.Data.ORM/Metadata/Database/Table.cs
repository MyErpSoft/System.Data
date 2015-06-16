using System.Collections.Generic;
using System.Data.Metadata.DataEntities;
using System.Globalization;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 定义了数据库的一个表或一个查询结果的Schema结构。
    /// </summary>
    public class Table : MemberMetadata {

        public Table(string name,Field[] fields, Relationship[] relationships)
            : base(name) {
            if (fields == null) {
                OrmUtility.ThrowArgumentNullException("fields");
            }

            _fields = fields;
            _fieldsDict = new Dictionary<string, Field>(_fields.Length, DatabaseMetadataContainer.DefaultNameComparer);
            foreach (var field in fields) {
                _fieldsDict.Add(field.Name, field);
            }

            if (relationships == null) {
                relationships = EmptyRelationships;
            }
            else {
                _relationships = relationships;
                _relationshipsDict = new Dictionary<string, Relationship>(relationships.Length, DatabaseMetadataContainer.DefaultNameComparer);
                foreach (var relationship in relationships) {
                    _relationshipsDict.Add(relationship.Name, relationship);
                }
            }
        }

        private readonly Field[] _fields;
        private readonly Dictionary<string, Field> _fieldsDict;
        /// <summary>
        /// 返回指定名称的字段
        /// </summary>
        public Field GetField(string name) {
            Field value;
            if (!_fieldsDict.TryGetValue(name,out value)) {
                OrmUtility.ThrowKeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.KeyNotFoundException, this.Name, name));
            }
            
            return value;
        }

        /// <summary>
        /// 返回所有的字段。
        /// </summary>
        public IEnumerable<Field> Fields {
            get { return this._fields; }
        }

        private static readonly Relationship[] EmptyRelationships = new Relationship[0];
        private readonly Relationship[] _relationships;
        private readonly Dictionary<string, Relationship> _relationshipsDict;
        /// <summary>
        /// 返回指定名称的关系对象
        /// </summary>
        /// <param name="name">要检索的关系名称</param>
        /// <returns>如果找到此名称的关系，将返回他，否则抛出异常。</returns>
        public Relationship GetRelationship(string name) {
            Relationship value;
            if (_relationshipsDict == null || !_relationshipsDict.TryGetValue(name, out value)) {
                OrmUtility.ThrowKeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.KeyNotFoundException, this.Name, name));
                return null;
            }

            return value;
        }

        /// <summary>
        /// 返回所有的子关系。
        /// </summary>
        public IEnumerable<Relationship> Relationships {
            get { return this._relationships; }
        }
    }
}
