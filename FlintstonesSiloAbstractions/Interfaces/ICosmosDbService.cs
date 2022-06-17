namespace FlintstonesSiloAbstractions.Interfaces
{
    public interface ICosmosDbService<T>
    {
        Task<IEnumerable<T>> GetAllAsync(string queryString);
        Task<T> GetAsync(string id);
        Task<T> AddAsync(T item);
        Task UpdateAsync(string id, T item);
    }
}
