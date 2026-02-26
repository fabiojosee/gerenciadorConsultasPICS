using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Domain.ValueObjects;

namespace AgendaPics.Application.Features.Autenticacao.Commands;

public record SolicitarAcessoCommand(string Cpf, string Email) : IRequest<Result>;

public class SolicitarAcessoCommandHandler : IRequestHandler<SolicitarAcessoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenService _accessTokenService;

    public SolicitarAcessoCommandHandler(IUnitOfWork unitOfWork, IAccessTokenService accessTokenService)
    {
        _unitOfWork = unitOfWork;
        _accessTokenService = accessTokenService;
    }

    public async Task<Result> Handle(SolicitarAcessoCommand request, CancellationToken cancellationToken)
    {
        var cpfResult = CPF.Create(request.Cpf);
        if (cpfResult.IsFailure)
            return Result.Failure(cpfResult.Error);

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        var agendamentos = await _unitOfWork.Agendamentos.ObterPorCpfAsync(cpfResult.Value.Value, cancellationToken);

        var agendamentoComEmail = agendamentos.FirstOrDefault(a =>
            a.EmailPaciente.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (agendamentoComEmail == null)
            return Result.Failure("Não foram encontrados agendamentos com este CPF e e-mail");

        await _accessTokenService.GerarTokenAsync(cpfResult.Value.Value, emailResult.Value.Value);

        return Result.Success();
    }
}
