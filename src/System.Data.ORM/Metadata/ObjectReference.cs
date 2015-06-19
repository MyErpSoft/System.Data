using System;
using System.Collections.Generic;
using System.Data.Metadata.Database;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata {

    /// <summary>
    /// 描述了一个对象的引用。
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    internal abstract class ObjectReference<T> {
        public abstract T Value { get; }
    }

    internal sealed class DictectObjectReference<T> : ObjectReference<T> {
        public DictectObjectReference(T value) {
            this._value = value;
        }

        private readonly T _value;
        public override T Value {
            get { return _value; }
        }
    }

    internal abstract class LazyGetObjectReference<T> : ObjectReference<T> where T : class {
        private T _value;

        protected abstract T GetValue();

        public sealed override T Value {
            get {
                if (_value == null) {
                    //多线程情况下，仅仅保留第一个。
                    Threading.Interlocked.CompareExchange<T>(ref _value, GetValue(), null);
                }

                return _value;
            }
        }
    }

    internal sealed class RelationshipToTableReference : LazyGetObjectReference<Table> {
        private string _tableName;
        private readonly Relationship _relationship;
        public RelationshipToTableReference(string tableName, Relationship relationship) {
            this._tableName = tableName;
            this._relationship = relationship;
        }

        protected override Table GetValue() {
            if (_relationship.From == null || this._relationship.From.Container == null) {
                OrmUtility.ThrowInvalidOperationException("not init");
            }

            try {
                return this._relationship.From.Container.GetTable(this._tableName);
            }
            catch (Exception ex) {
                throw new ApplicationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ToNameError,
                    _relationship.From.Name, _relationship.Name, this._tableName), ex);
            }
        }
    }

    internal sealed class EndMemberFieldReference : LazyGetObjectReference<Field> {
        private EndMember _endMember;
        private bool _isFrom; //true 表示从From表获取，false表示从To表获取。
        private string _fieldName;

        public EndMemberFieldReference(EndMember endMember, string fieldName, bool isFrom) {
            this._endMember = endMember;
            this._isFrom = isFrom;
            this._fieldName = fieldName;
        }

        protected override Field GetValue() {
            if (_endMember.Relationship == null || _endMember.Relationship.From == null) {
                OrmUtility.ThrowInvalidOperationException("not init");
            }

            Table table;
            if (_isFrom) {
                table = _endMember.Relationship.From;
            }
            else {
                table = _endMember.Relationship.To;
            }
            
            try {
                return table.GetField(_fieldName);
            }
            catch (Exception ex) {
                throw new ApplicationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.EndMemberFieldError,
                    _endMember.Relationship.From.Name, _endMember.Relationship.Name, this._fieldName), ex);
            }
        }
    }
}
