using Microsoft.AspNetCore.Http;
using Doctor.Domain.Interfaces;
using Doctor.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Doctor.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        public DoctorDbContext Context { get; }
        private readonly IHttpContextAccessor _contextAccessor;

        public UnitOfWork(DoctorDbContext context, IHttpContextAccessor contextAccessor)
        {
            Context = context;
            _contextAccessor = contextAccessor;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var loggedInUserId = _contextAccessor.HttpContext?.User?.Identity?.Name;
            var repo = new Repository<T, DoctorDbContext>(Context);
            return repo;
        }

        public TRepository GenericRepository<TRepository>() where TRepository : class
        {
            var loggedInUserId = _contextAccessor.HttpContext?.User?.Identity?.Name;
            var repo = (TRepository)Activator.CreateInstance(typeof(TRepository), Context, loggedInUserId);

            //repo.DataChanged += OnDataHistoryCreated;
            return repo;
        }

        public int Save()
        {
            return Context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return Context.SaveChangesAsync();
        }

        public Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!this._disposed)
                {
                    if (disposing)
                    {
                        Context?.Dispose();
                    }
                }
            }
            catch
            {
                // Ignore.
            }

            this._disposed = true;
        }

        /// <summary>
        /// Disposing
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
