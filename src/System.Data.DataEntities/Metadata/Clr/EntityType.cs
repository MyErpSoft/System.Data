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
            get { return this.ClrMember.IsAbstract; }
        }

        public bool IsSealed {
            get { return this.ClrMember.IsSealed; }
        }

        public string Namespace {
            get { return this.ClrMember.Namespace; }
        }

        public string FullName {
            get { return this.ClrMember.FullName; }
        }

        public Type UnderlyingSystemType {
            get { return this.ClrMember.UnderlyingSystemType; }
        }
        
        #endregion
        
        private Func<object> _createInstanceFunc;
        public object CreateInstance() {
            if (_createInstanceFunc == null) {
                Interlocked.CompareExchange<Func<object>>(ref _createInstanceFunc, this.ClrMember.GetCreateInstanceFunc(), null);
            }

            return _createInstanceFunc();
        }

        #region Property & Field

        public IEntityProperty GetProperty(string name) {
            IMemberMetadata member;
            if (TryGetMember(name,out member)) {
                IEntityProperty property = member as IEntityProperty;
                if (property != null) {
                    return property;
                }
            }

            OrmUtility.ThrowKeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                Properties.Resources.KeyNotFoundException, this.Name, name));
            return null;
        }

        public IEntityField GetField(string name) {
            IMemberMetadata member;
            if (TryGetMember(name, out member)) {
                IEntityField field = member as IEntityField;
                if (field != null) {
                    return field;
                }
            }

            OrmUtility.ThrowKeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                Properties.Resources.KeyNotFoundException, this.Name, name));
            return null;
        }

        private readonly MetadataReadOnlyCollection<IMemberMetadata> _members = new MemberCollection();
        /// <summary>
        /// 尝试获取指定名称的成员
        /// </summary>
        /// <param name="name">要检索的成员名称</param>
        /// <param name="member">如果找到将返回他，否则返回null</param>
        /// <returns>如果找到将返回true，否则返回false.</returns>
        public bool TryGetMember(string name, out IMemberMetadata member) {
            if (string.IsNullOrEmpty(name)) {
                member = null;
                return false;
            }

            //从已加载的集合中获取         
            if (_members.TryGetValue(name,out member)) {
                return true;
            }

            //反射获取成员信息。
            var clrMemberInfo = GetClrMember(name);
            if (clrMemberInfo == null) {
                return false;
            }

            //加入缓存
            return this.TryGetMemberCore(clrMemberInfo, out member);
        }

        /// <summary>
        /// 派生类可以重载此方法，过滤不希望公开的属性或字段
        /// </summary>
        /// <param name="name">要获取的字段或属性的名称</param>
        /// <returns>如果找到并确认公开，返回其实例，否则返回null.</returns>
        protected virtual MemberInfo GetClrMember(string name) {
            var members = this.ClrMember.GetMember(name, 
                Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic);
            //字段和属性只能有一个
            if (members != null && members.Length > 0) {
                return members[0];
            }

            return null;
        }

        private bool TryGetMemberCore(MemberInfo clrMember, out IMemberMetadata member) {
            //如果成员来自基类，那么应该使用基类的对象，这样可以节约Member对象的数量。
            lock (this) {
                if (_members.TryGetValue(clrMember.Name, out member)) {
                    return true;
                }

                //如果成员定义在基类，那么应该从基类获取，这样可以大大减少Property的描述对象
                var declaringType = clrMember.DeclaringType;
                if (declaringType != this.ClrMember) {
                    //这里调用TryGetMemberCore，目的是减少一次反射，由于TryGetMemberCore第一句话还会检查缓存，所以还是会使用缓存的。
                    if (!EntityType.GetEntityType(declaringType).TryGetMemberCore(ClrMember, out member)) {
                        return false; //基类可能认为此属性不适合公开
                    }
                }
                else {
                    member = this.CreateMemberMetadata(clrMember);
                }

                _members.Add(member);
                return true;
            }
        }

        protected IMemberMetadata CreateMemberMetadata(MemberInfo clrMember) {
            PropertyInfo p = clrMember as PropertyInfo;
            if (p != null) {
                return new EntityProperty(p);
            }
            else {
                throw new NotImplementedException();
            }
        }

        private sealed class MemberCollection : MetadataReadOnlyCollection<IMemberMetadata> {
            public MemberCollection() :base(null){
            }

            protected override string GetName(IMemberMetadata item) {
                return item.Name;
            }
        }
        #endregion
    }
}
