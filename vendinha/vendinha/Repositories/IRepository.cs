namespace vendinha.Repositories
{
    public interface IRepository<T>
    {
        Task Add(T item);
        Task Remove(long id);
        Task Update(T item);
        Task<T> FindByID(int id);
        IEnumerable<T> FindAll();
    }
}
