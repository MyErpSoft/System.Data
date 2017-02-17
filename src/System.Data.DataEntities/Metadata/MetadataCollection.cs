using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Data.Metadata {

    /// <summary>
    /// 默认的集合处理类。
    /// </summary>
    /// <typeparam name="TKey">元素键的类型</typeparam>
    /// <typeparam name="TItem">元素对象的类型</typeparam>
    /// <remarks>
    /// <para>此集合类实现以下功能：</para>
    /// <para> * 禁止添加null对象；</para>
    /// <para> * 延迟的按照名称检索的能力；</para>
    /// </remarks>
    public abstract class MetadataCollection<TKey, TItem> : Collection<TItem>  {

        /// <summary>
        /// 初始化使用默认相等比较器的 MetadataCollection 类的新实例。
        /// </summary>
        protected MetadataCollection() : this(null,null){ }
        
        /// <summary>
        /// 初始化使用指定相等比较器的 MetadataCollection 类的新实例。
        /// </summary>
        /// <param name="list">内部使用的数据集合。</param>
        /// <param name="comparer">
        /// 比较键时要使用的 System.Collections.Generic.IEqualityComparer`1 泛型接口的实现，如果为 null，则使用从
        /// System.Collections.Generic.EqualityComparer`1.Default 获取的该类型的键的默认相等比较器。
        /// </param>
        protected MetadataCollection(IList<TItem> list, IEqualityComparer<TKey> comparer)
            :base(list){
            if (comparer == null) {
                comparer = EqualityComparer<TKey>.Default;
            }
            this._comparer = comparer;
        }

        private readonly IEqualityComparer<TKey> _comparer;
        /// <summary>
        /// 获取用于确定集合中的键是否相等的泛型相等比较器。
        /// </summary>
        public IEqualityComparer<TKey> Comparer {
            get { return this._comparer; }
        }

        protected override void InsertItem(int index, TItem item) {
            VerifyItem(item);

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, TItem item) {
            VerifyItem(item);

            base.SetItem(index, item);
        }
        
        private void VerifyItem(TItem item) {
            if (item == null) {
                OrmUtility.ThrowArgumentNullException("item");
            }
        }

        /// <summary>
        /// 从元素中检索对象的键。
        /// </summary>
        /// <param name="item">要检索的对象</param>
        /// <returns>对象的键。</returns>
        protected abstract TKey GetKeyForItem(TItem item);

        /// <summary>
        /// 尝试获取指定键的元素。
        /// </summary>
        /// <param name="key">要检索的元素键。</param>
        /// <param name="item">检索到的元素</param>
        /// <returns>如果检索到此键的元素，返回true</returns>
        public bool TryGet(TKey key, out TItem item) {
            if (key != null) {
                foreach (var item2 in this.Items) {
                    if (this.Comparer.Equals(GetKeyForItem(item2), key)) {
                        item = item2;
                        return true;
                    }
                }
            }

            item = default(TItem);
            return false;
        }
    }
}
