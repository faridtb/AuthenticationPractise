using Country.Api.Dtos;
using Country.Api.Entities;

namespace Country.Api.Interfaces.Repository
{
    public interface IGenericRepositoryAsync<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<GeneralResponse> AddAsync(T entity);
        Task<GeneralResponse> RemoveAsync(Guid id);
        Task<GeneralResponse> UpdateAsync(T entity);
    }
}
