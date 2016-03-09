using MStack.Core.ComponetModels;
using MStack.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NHibernate.Linq;

namespace MStack.Core.Repositories
{
    public abstract class MStackRepository<TKey>
    {
        protected MStackRepository()
        {
        }

        protected ISession Session
        {
            get { return NHSessionFactory.Session; }
        }

        protected IStatelessSession StatelessSession
        {
            get { return NHSessionFactory.StatelessSession; }
        }

        protected void Delete<TEntity>(TKey key) where TEntity : IIdEntity<TKey>
        {
            Session.Delete(Session.Load<TEntity>(key, LockMode.None));
        }

        protected void Delete<TEntity>(TEntity entity) where TEntity : class, IIdEntity<TKey>
        {
            if (entity is ISoftDeleteEntity)
            {
                (entity as ISoftDeleteEntity).IsDeleted = true;
                Update<TEntity>(entity);
            }
            else
            {
                Delete<TEntity>(entity.Id);
            }
        }

        public TEntity Get<TEntity>(TKey key) where TEntity : IIdEntity<TKey>
        {
            return Session.Get<TEntity>(key, LockMode.None);
        }

        protected TEntity Load<TEntity>(TKey key) where TEntity : IIdEntity<TKey>
        {
            return Session.Load<TEntity>(key, LockMode.None);
        }

        /// <summary>
        /// 多要操作的对象提前进行预热
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keys"></param>
        protected void Preheat<TEntity>(List<TKey> keys) where TEntity : class, IIdEntity<TKey>
        {
            Session.CreateCriteria<TEntity>().Add(Restrictions.In("Id", keys.ToArray())).SetLockMode(LockMode.None).List();
        }

        protected void Preheat<TEntity>(params TEntity[] parms) where TEntity : class, IIdEntity<TKey>
        {
            Session.CreateCriteria<TEntity>().Add(Restrictions.In("Id", parms.Select(x => x.Id).ToArray())).SetLockMode(LockMode.None).List();
        }

        #region 子对象预热
        public PreheadHelper<TEntity, TKey> CreatePreheadContext<TEntity>(TEntity entity)
            where TEntity : class, IIdEntity<TKey>
        {
            return new PreheadHelper<TEntity, TKey>(Session).Add(entity);
        }

        public PreheadHelper<TEntity, TKey> CreatePreheadContext<TEntity>(IEnumerable<TEntity> entities)
                where TEntity : class, IIdEntity<TKey>
        {
            return new PreheadHelper<TEntity, TKey>(Session).Add(entities);
        }

        public PreheadHelper<TEntity, TKey> CreatePreheadContext<TEntity>(IEnumerable<TKey> entities)
        where TEntity : class, IIdEntity<TKey>
        {
            return new PreheadHelper<TEntity, TKey>(Session).Add(entities);
        }
        #endregion

        public void Insert<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Session.Persist(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Session.Update(Session.Merge(entity));
        }

        public void SaveOrUpdate<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Session.SaveOrUpdate(entity);
        }

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class, IEntity
        {
            return Session.Query<TEntity>();
        }

        #region Dapper
        protected virtual IDbConnection DbConnection
        {
            get { return Session.Connection; }
        }

        public virtual TEntity Get<TEntity>(Expression<Func<TEntity, bool>> orderPres = null)
            where TEntity : class
        {
            return Session.QueryOver<TEntity>().Where(orderPres).SingleOrDefault();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        public virtual Page<T> GetList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> where = null, IOrderBy<T> orderBy = null)
            where T : class
        {
            var pageQuery = Session.QueryOver<T>();
            if (where != null)
                pageQuery.Where(where);

            if (orderBy != null)
                pageQuery = pageQuery.UseOrderByContext(orderBy);

            var countQuery = pageQuery.Clone().Select(Projections.RowCount()).ClearOrders();

            //var cCount = Session.CreateCriteria<T>().SetProjection(Projections.RowCount());

            var multiQuery = Session.CreateMultiCriteria().Add<int>("count", countQuery.UnderlyingCriteria).Add<T>("page", pageQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).UnderlyingCriteria);

            var result = new Page<T> { Paging = new Paging { PageIndex = pageIndex, PageSize = pageSize } };
            result.Records = ((IList<T>)multiQuery.GetResult("page")).ToList();
            result.Paging.Total = ((IList<int>)multiQuery.GetResult("count")).Single();
            return result;
        }

        /// <summary>
        /// ICriteria 对象的分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual Page<TEntity> GetList<TEntity>(ICriteria query, int pageIndex, int pageSize)
        {
            //query.SetLockMode(LockMode.None);
            var countCriteria = (query.Clone() as ICriteria).SetProjection(Projections.Count(Projections.Id())).SetMaxResults(1);
            countCriteria.ClearOrders();

            var pageCriteria = query.SetFirstResult((pageIndex - 1) * pageSize).SetMaxResults(pageSize);

            var multiQuery = Session.CreateMultiCriteria().Add<int>("count", countCriteria).Add<TEntity>("page", pageCriteria);

            var result = new Page<TEntity> { Paging = new Paging { PageIndex = pageIndex, PageSize = pageSize } };
            result.Records = ((IList<TEntity>)multiQuery.GetResult("page")).ToList();
            result.Paging.Total = ((IList<int>)multiQuery.GetResult("count")).Single();
            //result.Records.ForEach(x => { Session.Evict(x); });
            return result;
        }

        protected virtual void ExecuteNoQuery(string sql, object param = null, IDbTransaction trans = null)
        {
            DbConnection.Execute(sql, param, trans);
        }
        #endregion
    }

    public class PreheadHelper<TEntity, TKey>
        where TEntity : class, IIdEntity<TKey>
    {
        private List<TEntity> entties;
        private List<TKey> Keys;
        private ISession _session;

        public PreheadHelper(ISession session)
        {
            this._session = session;
            this.entties = new List<TEntity>();
        }

        public PreheadHelper<TEntity, TKey> Add(TEntity entity)
        {
            this.entties.Add(entity);
            return this;
        }

        public PreheadHelper<TEntity, TKey> Add(IEnumerable<TEntity> entities)
        {
            this.entties.AddRange(entities);
            return this;
        }

        public PreheadHelper<TEntity, TKey> Add(IEnumerable<TKey> keys)
        {
            this.Keys.AddRange(keys);
            return this;
        }

        public void Prehead()
        {
            _session.CreateCriteria<TEntity>().Add(Restrictions.In("Id", entties.Select(x => x.Id).Concat(Keys).ToArray())).SetLockMode(LockMode.None).List();
        }
    }

    public static class NHibernateExtension
    {
        //public static Page<T> GetPageList<T>(this ICriteria query, ISession session, int pageIndex = 1, int pageSize = 20, string orderField = null, OrderDirection orderDir = OrderDirection.Asc)
        //{


        //    var pageCriteria = (query.Clone() as ICriteria);
        //    if (!string.IsNullOrWhiteSpace(orderField))
        //    {
        //        pageCriteria.AddOrder(new NHibernate.Criterion.Order(orderField, orderDir == OrderDirection.Asc));
        //    }
        //    pageCriteria.SetFirstResult((pageIndex - 1) * pageSize).SetMaxResults(pageSize);
        //    var countCriteria = (query.Clone() as ICriteria).SetProjection(Projections.Count(Projections.Id())).SetMaxResults(1);

        //    var multiQuery = session.CreateMultiCriteria().Add<int>("count", countCriteria).Add<T>("page", pageCriteria);

        //    var result = new Page<T> { Paging = new Paging { PageIndex = pageIndex, PageSize = pageSize } };
        //    result.Records = ((IList<T>)multiQuery.GetResult("page")).ToList();
        //    result.Paging.Total = ((IList<int>)multiQuery.GetResult("count")).Single();
        //    return result;
        //}
    }

    public interface IOrderBy<T>
    {
        IReadOnlyList<Tuple<OrderDirection, Expression<Func<T, object>>>> OrderByList { get; }

        OrderByContext<T> Asc(Expression<Func<T, object>> orderPres);
        OrderByContext<T> Desc(Expression<Func<T, object>> orderPres);
    }

    public class OrderByContext<T> : IOrderBy<T>
    {
        List<Tuple<OrderDirection, Expression<Func<T, object>>>> list = new List<Tuple<OrderDirection, Expression<Func<T, object>>>>();

        public OrderByContext<T> Asc(Expression<Func<T, object>> orderPres)
        {
            this.list.Add(new Tuple<OrderDirection, Expression<Func<T, object>>>(OrderDirection.Asc, orderPres));
            return this;
        }

        public OrderByContext<T> Desc(Expression<Func<T, object>> orderPres)
        {
            this.list.Add(new Tuple<OrderDirection, Expression<Func<T, object>>>(OrderDirection.Desc, orderPres));
            return this;
        }

        public IReadOnlyList<Tuple<OrderDirection, Expression<Func<T, object>>>> OrderByList
        {
            get { return list.AsReadOnly(); }
        }
    }

    public static class OrderByContextExtension
    {
        public static IQueryOver<T, T> UseOrderByContext<T>(this IQueryOver<T, T> obj, IOrderBy<T> orderByContext)
        {
            if (orderByContext == null)
                return obj;
            foreach (var item in orderByContext.OrderByList)
            {
                if (item.Item1 == OrderDirection.Asc)
                    obj.OrderBy(item.Item2).Asc();
                else
                    obj.OrderBy(item.Item2).Desc();
            }

            return obj;
        }
    }

    public enum OrderDirection
    {
        Asc,
        Desc
    }
}
