namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> ObterPorIdAsync(int id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task AdicionarAsync(T entidade);
        Task AtualizarAsync(T entidade);
        Task RemoverAsync(int id);
    }
}
