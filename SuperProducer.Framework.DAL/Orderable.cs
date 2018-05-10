using System;
using System.Linq;
using System.Linq.Expressions;

namespace SuperProducer.Framework.DAL
{
    /// <summary>
    /// 排序接口
    /// </summary>
    public interface IOrderable<T>
    {
        /// <summary>
        /// 正序
        /// </summary>
        IOrderable<T> Asc<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 增加正序
        /// </summary>
        IOrderable<T> ThenAsc<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 倒序
        /// </summary>
        IOrderable<T> Desc<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 增加倒序
        /// </summary>
        IOrderable<T> ThenDesc<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 结果集
        /// </summary>
        IQueryable<T> Queryable { get; }
    }

    /// <summary>
    /// Linq对集合排序实现
    /// </summary>
    public class Orderable<T> : IOrderable<T>
    {
        private IQueryable<T> _Queryable;

        /// <summary>
        /// 结果集
        /// </summary>
        public IQueryable<T> Queryable
        {
            get { return _Queryable; }
        }

        public Orderable(IQueryable<T> enumerable)
        {
            _Queryable = enumerable;
        }

        /// <summary>
        /// 正序
        /// </summary>
        public IOrderable<T> Asc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _Queryable = (_Queryable as IOrderedQueryable<T>)
                .OrderBy(keySelector);
            return this;
        }

        /// <summary>
        /// 增加正序
        /// </summary>
        public IOrderable<T> ThenAsc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _Queryable = (_Queryable as IOrderedQueryable<T>)
                .ThenBy(keySelector);
            return this;
        }

        /// <summary>
        /// 倒序
        /// </summary>
        public IOrderable<T> Desc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _Queryable = _Queryable
                .OrderByDescending(keySelector);
            return this;
        }

        /// <summary>
        /// 增加倒序
        /// </summary>
        public IOrderable<T> ThenDesc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _Queryable = (_Queryable as IOrderedQueryable<T>)
                .ThenByDescending(keySelector);
            return this;
        }
    }
}
