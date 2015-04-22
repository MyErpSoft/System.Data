using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace System.Data.DataEntities.Metadata.Clr
{
    /// <summary>
    /// CLR type IEntityType achieve.
    /// </summary>
    internal class EntityType : MemberMetadataBase<Type>, IEntityType {
        #region 从Type转换为EntityType

        private readonly static ConcurrentDictionary<Type, EntityType> _entityTypeCaches =
            new ConcurrentDictionary<Type, EntityType>();

        public static EntityType GetEntityType(Type clrType) {
            if (null == clrType) {
                return null;
            }

            EntityType entityType;
            if ((clrType.Assembly == null) || (clrType.Assembly.IsDynamic)) {
                //Dynamic class because it will too, do not cache.
                entityType = new EntityType(clrType);
                return entityType;
            }

            //在并发环境中，同一个clrType可能造成多次调用CreateEntityType，但仅仅采纳一个值，并没有副作用。
            return _entityTypeCaches.GetOrAdd(clrType, EntityTypeParser.Parse);
        }

        #endregion

        internal EntityType(Type clrType)
            : base(clrType) { }

        #region Properties
        public bool IsAbstract {
            get { return this.ClrMapping.IsAbstract; }
        }

        public bool IsSealed {
            get { return this.ClrMapping.IsSealed; }
        }

        public string Namespace {
            get { return this.ClrMapping.Namespace; }
        }

        public string FullName {
            get { return this.ClrMapping.FullName; }
        }

        public Type UnderlyingSystemType {
            get { return this.ClrMapping.UnderlyingSystemType; }
        }
        
        #endregion
        
        private Func<object> _createInstanceFunc;
        public object CreateInstance() {
            if (_createInstanceFunc == null) {
                Interlocked.CompareExchange<Func<object>>(ref _createInstanceFunc, this.ClrMapping.GetCreateInstanceFunc(), null);
            }

            return _createInstanceFunc();
        }

        #region Property & Field

        public IEntityProperty GetProperty(string name) {
            IEntityProperty property;
            if (TryGetProperty(name, out property)) {
                return property;
            }

            OrmUtility.ThrowKeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                Properties.Resources.KeyNotFoundException, this.Name, name));
            return null;
        }
        
        private PropertyCollection _properties;
        /// <summary>
        /// 尝试获取指定名称的成员
        /// </summary>
        /// <param name="name">要检索的成员名称</param>
        /// <param name="property">如果找到将返回他，否则返回null</param>
        /// <returns>如果找到将返回true，否则返回false.</returns>
        public bool TryGetProperty(string name, out IEntityProperty property) {
            if (string.IsNullOrEmpty(name)) {
                property = null;
                return false;
            }

            //从已加载的集合中获取         
            if (_properties != null && _properties.TryGetValue(name,out property)) {
                return true;
            }

            //反射获取成员信息。
            var clrMemberInfo = GetClrMember(name);
            if (clrMemberInfo == null) {
                property = null;
                return false;
            }

            //加入缓存
            return this.TryGetPropertyCore(clrMemberInfo, out property);
        }

        /// <summary>
        /// 派生类可以重载此方法，过滤不希望公开的属性或字段
        /// </summary>
        /// <param name="name">要获取的字段或属性的名称</param>
        /// <returns>如果找到并确认公开，返回其实例，否则返回null.</returns>
        protected virtual MemberInfo GetClrMember(string name) {
            //虽然IEntityType对外公开的是Property,但实际上我们也可以包装字段
            var members = this.ClrMapping.GetMember(name, 
                Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic);
            //字段和属性只能有一个
            if (members != null && members.Length > 0) {
                return members[0];
            }

            return null;
        }

        private bool TryGetPropertyCore(MemberInfo clrMember, out IEntityProperty property) {
            //如果成员来自基类，那么应该使用基类的对象，这样可以节约Member对象的数量。
            lock (this) {
                if (_properties != null && _properties.TryGetValue(clrMember.Name, out property)) {
                    return true;
                }

                //如果成员定义在基类，那么应该从基类获取，这样可以大大减少Property的描述对象
                var declaringType = clrMember.DeclaringType;
                if (declaringType != this.ClrMapping) {
                    //这里调用TryGetPropertyCore，目的是减少一次反射，由于TryGetPropertyCore第一句话还会检查缓存，所以还是会使用缓存的。
                    if (!EntityType.GetEntityType(declaringType).TryGetPropertyCore(clrMember, out property)) {
                        return false; //基类可能认为此属性不适合公开
                    }
                }
                else {
                    property = this.CreatePropertyMetadata(clrMember);
                }

                if (_properties == null) {
                    _properties = new PropertyCollection();
                }
                _properties.Add(property);
                return true;
            }
        }

        /// <summary>
        /// 根据Clr成员信息创建属性对象。
        /// </summary>
        /// <param name="clrMember">一个clr成员</param>
        /// <returns>创建成功的成员</returns>
        protected IEntityProperty CreatePropertyMetadata(MemberInfo clrMember) {
            PropertyInfo p = clrMember as PropertyInfo;
            if (p != null) {
                return new EntityProperty(p);
            }
            else {
                throw new NotImplementedException();
            }
        }

        private sealed class PropertyCollection : MetadataReadOnlyCollection<IEntityProperty> {
            public PropertyCollection() :base(null){
            }

            protected override string GetName(IEntityProperty item) {
                return item.Name;
            }
        }
        #endregion
    }
}
