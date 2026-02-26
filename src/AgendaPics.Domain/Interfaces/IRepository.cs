namespace AgendaPics.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> ObterPorIdAsync(object id, CancellationToken cancellationToken = default);
    Task<T?> ObterPorIdAsync(object?[] ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default);
    void Atualizar(T entidade);
    void Remover(T entidade);
}
