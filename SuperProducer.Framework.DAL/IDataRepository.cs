using SuperProducer.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SuperProducer.Framework.DAL
{
    /// <summary>
    /// 数据仓库接口
    /// </summary>
    public interface IDataRepository
    {
        T Insert<T>(T entity) where T : ModelBase;

        bool Delete<T>(T entity) where T : ModelBase;

        T Update<T>(T entity) where T : ModelBase;

        T Find<T>(params object[] keyValues) where T : ModelBase;

        List<T> FindAll<T>(Expression<Func<T, bool>> conditions = null) where T : ModelBase;

        PagedList<T> FindAllByPage<T, S>(Expression<Func<T, bool>> conditions, Expression<Func<T, S>> orderBy, int pageIndex, int pageSize) where T : ModelBase;
    }
}
