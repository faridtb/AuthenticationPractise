using Country.Api.Entities;

namespace Country.Api.Interfaces.Repository
{
    public interface IGenericRepositoryAsync<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<bool> AddAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> UpdateAsync(Guid id);
    }
}
