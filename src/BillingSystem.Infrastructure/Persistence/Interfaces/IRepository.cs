using BillingSystem.Domain.Contracts;

namespace BillingSystem.Infrastructure.Persistence.Interfaces
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetPaged(int pageNumber, int pageSize);
        Task<IEnumerable<T>> Query(Func<T, bool> query);
        Task<T> Get(int id);
        Task<long> Insert(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
    }
}
