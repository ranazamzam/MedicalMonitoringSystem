using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Domain.Interfaces
{
    public interface IRepository<T> : IBaseRepository<T> where T : class
    {
        #region FirstOrDefault/LastOrDefault Operations

        /// <summary>
        /// Returns the First Or Default of DbSet
        /// </summary>
        /// <returns></returns>
        T FirstOrDefault();

        /// <summary>
        /// Returns the First Or Default of DbSet
        /// </summary>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync();

        /// <summary>
        /// Returns First Or Default based on search criteria (where)
        /// </summary>
        /// <param name="where">Where Expression to search in dbset</param>
        /// <param name="noTracking">sets if entity framework will track entry in dbset or not</param>
        /// <returns></returns>
        T FirstOrDefault(Expression<Func<T, bool>> where, bool noTracking = false);

        /// <summary>
        /// Returns First Or Default based on search criteria (where)
        /// </summary>
        /// <param name="where">Where Expression to search in dbset</param>
        /// <param name="noTracking">sets if entity framework will track entry in dbset or not</param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where, bool noTracking = false);

        /// <summary>
        /// Returns LastOrDefault based on search criteria (where) and Order Column
        /// </summary>
        /// <param name="where">where expression</param>
        /// <param name="orderBy">LastOrDefault when order by this column</param>
        /// <returns></returns>
        T LastOrDefault<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy);

        #endregion

        #region Find Operations
        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">
        /// where clause:<![CDATA[<example><code>x=> x.Id == Id && x.Name.StartsWith("A")</code></example>]]> 
        /// </param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> where, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, Aync
        /// </summary>
        /// <param name="where">
        /// where clause:<![CDATA[<example><code>x=> x.Id == Id && x.Name.StartsWith("A")</code></example>]]> 
        /// </param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> where, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">Select Expression (Exp: x=> new {Id = x.Id, Name = x.Name})</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        IEnumerable<TOut> Find<TOut>(Expression<Func<T, bool>> where, Expression<Func<T, TOut>> select, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, Aync
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">Select Expression (Exp: x=> new {Id = x.Id, Name = x.Name})</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        Task<List<TOut>> FindAsync<TOut>(Expression<Func<T, bool>> where, Expression<Func<T, TOut>> select, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, 
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> where, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, Async
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> where, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="total">Total Number of Records</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> where, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, Async
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="total">Total Number of Records</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> where, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">Select Expression (Exp: x=> new {Id = x.Id, Name = x.Name})</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        IEnumerable<TOut> Find<TOut>(Expression<Func<T, bool>> where, Expression<Func<T, TOut>> select, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, Async
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">Select Expression (Exp: x=> new {Id = x.Id, Name = x.Name})</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        Task<List<TOut>> FindAsync<TOut>(Expression<Func<T, bool>> where, Expression<Func<T, TOut>> select, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">Select Expression (Exp: x=> new {Id = x.Id, Name = x.Name})</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="total">Total Number of Records</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        IEnumerable<TOut> Find<TOut>(Expression<Func<T, bool>> where, Expression<Func<T, TOut>> select, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false);

        /// <summary>
        /// This Function will use Expression to filter results, Async
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">Select Expression (Exp: x=> new {Id = x.Id, Name = x.Name})</param>
        /// <param name="page">page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">pagesize, number of items to retrieve</param>
        /// <param name="total">Total Number of Records</param>
        /// <param name="noTracking">Set It to true if you want to get data with NoTracking Enabled</param>
        /// <param name="includeSoftDelete">If set to true, All Items on Db Included Softdeleted Items will be get</param>
        /// <returns></returns>
        Task<List<TOut>> FindAsync<TOut>(Expression<Func<T, bool>> where, Expression<Func<T, TOut>> select, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false);

        #endregion

        #region GetAll Operations
        ///<summary>
        /// Gets All Entities as No Tracking
        /// </summary> 
        /// <returns>
        /// Entites that are not tracked in memory
        /// </returns>       
        IQueryable<T> GetAllNoTracking { get; }

        //TODO: Add FindIncluding Functions
        /// <summary>
        /// Gets All Entities Including Eager Loaded Relations
        /// </summary>
        /// <param name="includedProperties">Included Relations</param>
        /// <returns>Entities with eager loaded selected relations</returns>
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includedProperties);

        /// <summary>
        /// Get all Entities Including Eager loaded Relations also nested relations.
        /// </summary>
        /// <param name="includedProperties">Comma seperated table (relation) names to include in result set.</param>
        /// <returns>Entities with Eager loaded selected relations</returns>
        IQueryable<T> GetAllIncluding(string includedProperties);

        /// <summary>
        /// This Function Returns All Items Included SoftDeleted Items if related entity is implementing ISoftDeleteEnabled
        /// </summary>
        IQueryable<T> GetAllIncludeSoftDeleted { get; }

        #endregion

        void InsertRange(List<T> entities);

        /// <summary>
        /// Bulk inserts objects to table
        /// </summary>
        /// <param name="rows">List of type [your type]</param>
        /// <param name="tableName">TableName to insert data in</param>
        void BulkInsert(List<T> rows, string tableName);

        void HardDelete(T entity);

        /// <summary>
        /// Returns Count of Items after filtering by predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate);
    }
}
