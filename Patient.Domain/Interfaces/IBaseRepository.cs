using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Domain.Interfaces
{
    public interface IBaseRepository<T> : IDisposable where T : class
    {
        /// <summary>
        /// Gets All Entities
        /// </summary>
        IEnumerable<T> GetAll { get; }

        List<TOut> GetAllWithSelect<TOut>(Expression<Func<T, TOut>> @select);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(params object[] id);

        /// <summary>
        /// Get entity by identifier async
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        Task<T> GetByIdAsync(params object[] id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(T entity);

        /// <summary>
        /// Update entity
        /// </summary>        
        /// <param name="entity">Entity</param>
        void Update(T entity);


        /// <summary>
        /// Delete entity by Id
        /// </summary>        
        /// <param name="id">Id or Ids (if table has composite primary key)</param>
        void Delete(params object[] id);

        /// <summary>
        /// Delete entity 
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);
    }
}
