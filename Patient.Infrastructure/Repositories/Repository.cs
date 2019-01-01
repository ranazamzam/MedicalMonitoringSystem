using FastMember;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Patient.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Infrastructure.Repositories
{
    public class Repository<T, TC> : IRepository<T> where T : class where TC : DbContext
    {
        private readonly TC _context;
        private DbSet<T> _dbSet;

        public Repository(TC context)
        {
            _context = context;
        }

        protected virtual DbSet<T> DbSet => _dbSet ?? (_dbSet = _context.Set<T>());

        public T FirstOrDefault()
        {
            // TODO: is Returning without checking isDeleted
            return DbSet.FirstOrDefault();
        }

        public Task<T> FirstOrDefaultAsync()
        {
            // TODO: is Returning without checking isDeleted
            return DbSet.FirstOrDefaultAsync();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> @where, bool noTracking = false)
        {
            IQueryable<T> set = DbSet;
            if (noTracking)
                set = DbSet.AsNoTracking();

            return set.FirstOrDefault(@where);
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> @where, bool noTracking = false)
        {
            IQueryable<T> set = DbSet;

            if (noTracking)
                set = DbSet.AsNoTracking();

            return set.FirstOrDefaultAsync(@where);
        }

        public T LastOrDefault<TKey>(Expression<Func<T, bool>> @where, Expression<Func<T, TKey>> orderBy)
        {
            return DbSet.Where(@where).OrderByDescending(orderBy).FirstOrDefault();
        }

        /// <summary>
        /// InsertAsync An Entity Into Db using EntityFramework
        /// </summary>
        /// <param name="entity">Entity to InsertAsync</param>
        public virtual void Insert(T entity)
        {
            // TODO: Test Data History
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Add(entity);
        }

        /// <summary>
        /// Inserts Range of entities 
        /// </summary>
        /// <param name="entities">Entities to insert</param>
        public void InsertRange(List<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any())
                return;

            DbSet?.AddRange(entities);
        }

        /// <summary>
        /// Deletes an Item by Id
        /// </summary>
        /// <param name="id">id or ids for tables with composite key</param>
        public virtual void Delete(params object[] id)
        {
            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Deletes an Item
        /// </summary>
        /// <param name="entity">Entity to Delete</param>
        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Remove(entity);
        }

        /// <summary>
        /// Bulk Inserts Items to DB
        /// </summary>
        /// <param name="rows">rows to be bulk inserted</param>
        /// <param name="tableName">table name to bulk insert into it</param>
        public void BulkInsert(List<T> rows, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException();

            using (var c = new SqlBulkCopy(_context.Database.GetDbConnection().ConnectionString))
            {
                c.DestinationTableName = tableName;
                c.BulkCopyTimeout = 60;
                //c.BatchSize = 50               

                string[] props;
                using (var sqlCmd = new SqlCommand())
                {
                    sqlCmd.CommandText = $"SELECT TOP(1) * FROM {tableName}";
                    sqlCmd.Connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);

                    var da = new SqlDataAdapter { SelectCommand = sqlCmd };

                    var ds = new DataSet();

                    da.FillSchema(ds, SchemaType.Source);

                    props = ds.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                }

                var table = new DataTable();

                //var row = rows.FirstOrDefault();                
                //_context.Model.FindEntityType(typeof(T)).N
                using (var reader = ObjectReader.Create(rows, props))
                {
                    table.Load(reader);
                }

                c.WriteToServer(table.CreateDataReader());
            }
        }

        /// <summary>
        /// Hard deletes an object 
        /// </summary>
        /// <param name="entity"></param>
        public void HardDelete(T entity)
        {
            // TODO: Check if Item is ISoftDeleteEnabled and if is, Check if Item is softdeleted before hard delete.
            // TODO: Check to save data history
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Remove(entity);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var result = DbSet.Where(predicate);
            return result.Count();
        }

        /// <summary>
        /// Update entity using entityframework
        /// </summary>        
        /// <param name="entity">Entity</param>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            _context.Entry(entity).State = EntityState.Modified;

        }

        /// <summary>
        /// Gets All Entities
        /// </summary>
        public virtual IEnumerable<T> GetAll
        {
            get
            {
                return DbSet;
            }
        }


        public virtual List<TOut> GetAllWithSelect<TOut>(Expression<Func<T, TOut>> @select)
        {
            return DbSet.Select(@select).ToList();
        }
        /// <summary>
        /// Gets All Entities with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> GetAllNoTracking
        {
            get
            {
                return DbSet.AsNoTracking();
            }
        }

        private IQueryable<T> PrivateFind(Expression<Func<T, bool>> @where, bool noTracking = false, bool includeSoftDelete = false)
        {
            var result = noTracking ? DbSet.AsNoTracking().Where(@where) : DbSet.Where(@where);
            return result;
        }

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="noTracking"></param>
        /// <param name="includeSoftDelete"></param>
        /// <returns></returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> @where, bool noTracking = false, bool includeSoftDelete = false)
        {
            return PrivateFind(where, noTracking, includeSoftDelete);
        }

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> @where, bool noTracking = false, bool includeSoftDelete = false)
        {
            return PrivateFind(where, noTracking, includeSoftDelete).ToListAsync();
        }

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>
        /// <param name="select">The Select Expression</param>
        /// <param name="noTracking"></param>
        /// <param name="includeSoftDelete"></param>
        /// <returns></returns>
        public IEnumerable<TOut> Find<TOut>(Expression<Func<T, bool>> @where, Expression<Func<T, TOut>> @select, bool noTracking = false, bool includeSoftDelete = false)
        {
            return PrivateFind(@where, noTracking, includeSoftDelete).Select(@select);
        }

        public Task<List<TOut>> FindAsync<TOut>(Expression<Func<T, bool>> @where, Expression<Func<T, TOut>> @select, bool noTracking = false, bool includeSoftDelete = false)
        {
            return PrivateFind(@where, noTracking, includeSoftDelete).Select(@select).ToListAsync();
        }

        /// <summary>
        /// This Function will use Expression to filter results,
        /// </summary>
        /// <param name="where">where clause (Exp: x=> x.Id == Id && x.Name.StartsWith("A"))</param>        
        /// <param name="page">(optional) page number, used to get page number x in paged results, if not determined all data will be returned</param>
        /// <param name="pageSize">(optional) pagesize, number of items to retrieve</param>
        /// <param name="noTracking"></param>
        /// <param name="includeSoftDelete"></param>
        /// <returns></returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> @where, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            return PrivateFind(@where, noTracking, includeSoftDelete).OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> @where, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            return PrivateFind(@where, noTracking, includeSoftDelete).OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> @where, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            var data = PrivateFind(@where, noTracking, includeSoftDelete);
            total = data.Count();

            return data.OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> @where, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            var data = PrivateFind(@where, noTracking, includeSoftDelete);
            total = data.Count();

            return data.OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public IEnumerable<TOut> Find<TOut>(Expression<Func<T, bool>> @where, Expression<Func<T, TOut>> @select, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            return PrivateFind(@where, noTracking, includeSoftDelete).OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize).Select(select);
        }

        public Task<List<TOut>> FindAsync<TOut>(Expression<Func<T, bool>> @where, Expression<Func<T, TOut>> @select, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            return PrivateFind(@where, noTracking, includeSoftDelete).OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize).Select(select).ToListAsync();
        }

        public IEnumerable<TOut> Find<TOut>(Expression<Func<T, bool>> @where, Expression<Func<T, TOut>> @select, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            var data = PrivateFind(@where, noTracking, includeSoftDelete);

            total = data.Count();

            return data.OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize).Select(select);
        }

        public Task<List<TOut>> FindAsync<TOut>(Expression<Func<T, bool>> @where, Expression<Func<T, TOut>> @select, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), $"{nameof(page)} should be larger than zero");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be larger than zero");

            var data = PrivateFind(@where, noTracking, includeSoftDelete);

            total = data.Count();

            return data.OrderBy(o => 1).Skip((page - 1) * pageSize).Take(pageSize).Select(select).ToListAsync();
        }

        /// <summary>
        /// Gets All Entities Including Eager Loaded Relations
        /// </summary>
        /// <param name="includedProperties">Included Relations</param>
        /// <returns>Entities with eager loaded selected relations</returns>
        public virtual IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includedProperties)
        {
            var entities = DbSet.AsQueryable();

            foreach (var includedPropery in includedProperties)
            {
                entities = entities.Include(includedPropery);
            }

            return entities;
        }

        /// <summary>
        /// Includes a nested relation in query for eager loading
        /// </summary>
        /// <typeparam name="TProperty">Nested Navigation property to include in eager loading</typeparam>
        /// <param name="includedExpression">the Expression to get EagerLoading</param>
        /// <returns></returns>
        public IIncludableQueryable<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> includedExpression)
        {
            var entities = DbSet.AsQueryable();

            return entities.Include(includedExpression);
        }

        /// <summary>
        /// Get all Entities Including Eager loaded Relations also nested relations.
        /// </summary>
        /// <param name="includedProperties">Comma seperated table (relation) names to include in result set.</param>
        /// <returns>Entities with Eager loaded selected relations</returns>
        public virtual IQueryable<T> GetAllIncluding(string includedProperties)
        {
            var entities = DbSet.AsQueryable();
            var relations = includedProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var property in relations)
            {
                entities = entities.Include(property);
            }

            return entities;
        }

        public IQueryable<T> GetAllIncludeSoftDeleted => DbSet;

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(params object[] id)
        {
            var item = DbSet.Find(id);

            return item;
        }

        /// <summary>
        /// Get entity by identifier Async
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual Task<T> GetByIdAsync(params object[] id)
        {
            var item = DbSet.FindAsync(id);

            return item;
        }

        #region Disposing        
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Disposing
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Returns primary key column and values for an entity.
        /// </summary>
        /// <param name="entity">Entity object to get its primary key columns and values</param>
        /// <returns>A dictionary of primary key name and value</returns>
        private Dictionary<string, object> GetPrimaryKeyColumns(T entity)
        {
            var keys = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name);
            var props = typeof(T).GetProperties().Where(x => keys.Contains(x.Name));

            return props.ToDictionary(x => x.Name, x => x.GetValue(entity));
        }

        /// <summary>
        /// Gets an expression for IsDeleted == true statement.
        /// </summary>
        /// <returns>Lambda expression for x=> x.IsDelted == true</returns>
        private static Expression<Func<T, bool>> GetIsDeletedExpression()
        {
            var param = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(param, "IsDeleted");
            Expression exp = Expression.Not(member);
            var exp2 = Expression.Lambda<Func<T, bool>>(exp, param);
            return exp2;
        }

        /// <summary>
        /// Gets the expression for Primary keys of a entity 
        /// </summary>
        /// <param name="primaryKeyValues">Dictionary of key name and values of the entity</param>
        /// <returns>returns lambda Expression for primary key name and values</returns>
        private Expression<Func<T, bool>> GetPrimaryKeyExpression(Dictionary<string, object> primaryKeyValues)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var pk = primaryKeyValues.First();
            var member = Expression.Property(param, pk.Key);
            ConstantExpression conExp = Expression.Constant(pk.Value);
            BinaryExpression exp = Expression.Equal(member, conExp);
            foreach (var nextPk in primaryKeyValues.Skip(1).ToList())
            {
                member = Expression.Property(param, nextPk.Key);
                conExp = Expression.Constant(nextPk.Value);
                var exp2 = Expression.Equal(member, conExp);

                exp = Expression.And(exp, exp2);
            }

            var finalExp = Expression.Lambda<Func<T, bool>>(exp, param);
            return finalExp;
        }
        #endregion
    }
}
