using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Patient.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;

        TRepository GenericRepository<TRepository>() where TRepository : class;
        
        int Save();
        Task<int> SaveAsync();
        Task<int> SaveAsync(CancellationToken cancellationToken);

        // Add Custom Repositories here
    }
}
