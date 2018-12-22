using Patient.Domain.Interfaces;
using Patient.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PatientEntity = Patient.Domain.Models.PatientEntity;

namespace Patient.Infrastructure.Repositories
{
    public class InMemoryRepository : IRepository<PatientEntity>
    {
        private List<PatientEntity> _patients;

        public InMemoryRepository()
        {
            _patients = new List<PatientEntity>();
            _patients.AddRange(new[]
            {
                new PatientEntity
                {
                    Id="19860813-XXXX",
                    Name="Henrik Karlsson"
                },
                new PatientEntity
                {
                    Id="19750612-XXXX",
                    Name="Erik Henriksson"
                },
                new PatientEntity
                {
                    Id="19600519-XXXX",
                    Name="Cecilia Eliasson"
                },
            });
        }

        public IQueryable<PatientEntity> GetAllNoTracking => throw new NotImplementedException();

        public IQueryable<PatientEntity> GetAllIncludeSoftDeleted => throw new NotImplementedException();

        public IEnumerable<PatientEntity> GetAll
        {
            get
            {
                return _patients;
            }
        }


        public void BulkInsert(List<PatientEntity> rows, string tableName)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<PatientEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(params object[] id)
        {
            throw new NotImplementedException();
        }

        public void Delete(PatientEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PatientEntity> Find(Expression<Func<PatientEntity, bool>> where, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TOut> Find<TOut>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TOut>> select, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PatientEntity> Find(Expression<Func<PatientEntity, bool>> where, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PatientEntity> Find(Expression<Func<PatientEntity, bool>> where, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TOut> Find<TOut>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TOut>> select, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TOut> Find<TOut>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TOut>> select, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<PatientEntity>> FindAsync(Expression<Func<PatientEntity, bool>> where, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<TOut>> FindAsync<TOut>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TOut>> select, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<PatientEntity>> FindAsync(Expression<Func<PatientEntity, bool>> where, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<PatientEntity>> FindAsync(Expression<Func<PatientEntity, bool>> where, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<TOut>> FindAsync<TOut>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TOut>> select, int page, int pageSize, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<TOut>> FindAsync<TOut>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TOut>> select, int page, int pageSize, out int total, bool noTracking = false, bool includeSoftDelete = false)
        {
            throw new NotImplementedException();
        }

        public PatientEntity FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public PatientEntity FirstOrDefault(Expression<Func<PatientEntity, bool>> where, bool noTracking = false)
        {
            throw new NotImplementedException();
        }

        public Task<PatientEntity> FirstOrDefaultAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PatientEntity> FirstOrDefaultAsync(Expression<Func<PatientEntity, bool>> where, bool noTracking = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<PatientEntity> GetAllIncluding(params Expression<Func<PatientEntity, object>>[] includedProperties)
        {
            throw new NotImplementedException();
        }

        public IQueryable<PatientEntity> GetAllIncluding(string includedProperties)
        {
            throw new NotImplementedException();
        }

        public PatientEntity GetById(params object[] id)
        {
            throw new NotImplementedException();
        }

        public Task<PatientEntity> GetByIdAsync(params object[] id)
        {
            return Task.FromResult(_patients.FirstOrDefault(x => x.Id == id[0].ToString()));
        }

        public void HardDelete(PatientEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(PatientEntity entity)
        {
            throw new NotImplementedException();
        }

        public void InsertRange(List<PatientEntity> entities)
        {
            throw new NotImplementedException();
        }

        public PatientEntity LastOrDefault<TKey>(Expression<Func<PatientEntity, bool>> where, Expression<Func<PatientEntity, TKey>> orderBy)
        {
            throw new NotImplementedException();
        }

        public void Update(PatientEntity entity)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InMemoryRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
