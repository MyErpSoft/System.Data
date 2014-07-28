using System.Collections.Generic;

namespace System.Data.DataEntities.Metadata.Clr
{
    /// <summary>
    /// CLR type IEntityType achieve.
    /// </summary>
    internal sealed class EntityType : MemberMetadataBase<Type>,IEntityType
    {
        private static Dictionary<Type, EntityType> _entityTypeCaches;
        private static object _lockObject;
        static EntityType()
        {
            _lockObject = new object();
            _entityTypeCaches = new Dictionary<Type, EntityType>(8);
        }

        public static EntityType GetEntityType(Type clrType)
        {
            if (null == clrType)
            {
                return null;
            }

            EntityType entityType;
            if ((clrType.Assembly == null)  || (clrType.Assembly.IsDynamic))
            {
                //Dynamic class because it will too, do not cache.
                entityType = new EntityType(clrType);
                entityType.Init();
                return entityType;
            }

            if (!_entityTypeCaches.TryGetValue(clrType,out entityType))
            {
                lock (_lockObject)
                {
                    if (!_entityTypeCaches.TryGetValue(clrType, out entityType))
                    {
                        entityType = new EntityType(clrType);
                        _entityTypeCaches.Add(clrType, entityType);
                        entityType.Init();
                    }
                }
            }
            return entityType;
        }

        private EntityType(Type clrType)
            : base(clrType)
        {}

        internal SimpleEntityProperty _primaryKey;
        public ISimpleEntityProperty PrimaryKey
        {
            get { return _primaryKey; }
        }

        internal EntityType _baseType;
        public IEntityType BaseType
        {
            get { return _baseType; }
        }

        public bool IsAbstract
        {
            get { return this.ClrMember.IsAbstract; }
        }

        public bool IsSealed
        {
            get { return this.ClrMember.IsSealed; }
        }

        public bool IsInterface
        {
            get { return this.ClrMember.IsInterface; }
        }

        public string Namespace
        {
            get { return this.ClrMember.Namespace; }
        }

        public string FullName
        {
            get { return this.ClrMember.FullName; }
        }

        internal EntityPropertyCollection _properties;
        public IEntityPropertyCollection Properties
        {
            get { return _properties; }
        }

        private void Init()
        {
            EntityTypeParser.Parse(this.ClrMember, this);
        }

        private Func<object> _createInstanceFunc;
        public object CreateInstance()
        {
            if (_createInstanceFunc == null)
            {
                _createInstanceFunc = this.ClrMember.GetCreateInstanceFunc();
            }
            return _createInstanceFunc();
        }


        public IEntityType[] GetInterfaces()
        {
            var interfaces = this.ClrMember.GetInterfaces();
            IEntityType[] result = new IEntityType[interfaces.Length];
            for (int i = 0; i < interfaces.Length; i++)
            {
                result[i] = GetEntityType(interfaces[i]);
            }
            return result;
        }

        public bool IsAssignableFrom(IEntityType c)
        {
            EntityType dt = c as EntityType;
            if (dt != null)
            {
                return this.ClrMember.IsAssignableFrom(dt.ClrMember);
            }
            return false;
        }

        
    }
}
