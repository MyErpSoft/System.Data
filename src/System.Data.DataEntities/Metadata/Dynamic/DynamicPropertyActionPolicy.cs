using System.Data.DataEntities.Dynamic;

namespace System.Data.DataEntities.Metadata.Dynamic {

    public abstract class DynamicPropertyActionPolicy<T> where T : DynamicPropertyActionPolicy<T> {

        internal static T Override(T firstPolicy, T newPolicy, T basePolicy) {
            if (firstPolicy == null) {
                OrmUtility.ThrowArgumentNullException("firstPolicy");
            }

            T prePolicy = null;
            T current = firstPolicy;

            do {
                if (object.ReferenceEquals(current, basePolicy)) {
                    newPolicy._basePolicy = basePolicy;

                    if (prePolicy == null) {
                        return newPolicy;
                    }
                    else {
                        prePolicy._basePolicy = newPolicy;
                        return firstPolicy;
                    }                    
                }

                if (current == null) {
                    OrmUtility.ThrowArgumentNullException("basePolicy");
                }

                prePolicy = current;
                current = current._basePolicy;
            } while (true);
        }

        private T _basePolicy;
        protected T BasePolicy {
            get { return this._basePolicy; }
        }
    }

    public abstract class DynamicPropertyGetPolicy<TObject, TValue> : DynamicPropertyActionPolicy<DynamicPropertyGetPolicy<TObject, TValue>> {

        public abstract TValue GetValue(TObject obj);
    }

    internal sealed class DynamicDefaultValueGetPolicy<TObject, TValue> : DynamicPropertyGetPolicy<TObject, TValue> {
        public DynamicDefaultValueGetPolicy()
            : this(default(TValue)) {
        }

        public DynamicDefaultValueGetPolicy(TValue defaultValue) {
            this._defaultValue = defaultValue;
        }

        private TValue _defaultValue;

        public override TValue GetValue(TObject obj) {
            return this._defaultValue;
        }
    }

    internal sealed class DynamicLocalValueGetPolicy<TValue> : DynamicPropertyGetPolicy<DynamicEntity, TValue> {
        private static readonly object NullObject = new object();

        private DynamicEntityProperty _property;
        protected DynamicEntityProperty Property {
            get { return this._property; }
        }

        public override TValue GetValue(DynamicEntity obj) {
            object value = obj._values[this._property.Ordinal];
            if (value == null) {
                return this.BasePolicy.GetValue(obj);
            }

            if (object.ReferenceEquals(NullObject, value)) {
                return default(TValue);
            }

            return (TValue)value;
        }
    }

}