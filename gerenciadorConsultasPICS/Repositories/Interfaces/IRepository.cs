namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<T> ObterPorIdAsync(object id);
        public Task<T> ObterPorIdAsync(object?[] ids);
        public Task<IEnumerable<T>> ObterTodosAsync();
        public Task AdicionarAsync(T entidade);
        public Task AtualizarAsync(T entidade);
        public Task RemoverAsync(object id);
        public Task RemoverAsync(object?[] ids);
    }
}
