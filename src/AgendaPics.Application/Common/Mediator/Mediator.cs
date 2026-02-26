namespace AgendaPics.Application.Common.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler não encontrado para {requestType.Name}");

        var method = handlerType.GetMethod("Handle");
        if (method == null)
            throw new InvalidOperationException($"Método Handle não encontrado no handler para {requestType.Name}");

        var result = method.Invoke(handler, [request, cancellationToken]);
        if (result is not Task<TResponse> task)
            throw new InvalidOperationException($"O handler para {requestType.Name} não retornou o tipo esperado");

        return await task;
    }
}
