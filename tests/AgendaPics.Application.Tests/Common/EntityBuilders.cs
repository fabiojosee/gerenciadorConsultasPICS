using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Enums;

namespace AgendaPics.Application.Tests.Common;

public static class EntityBuilders
{
    // Dados válidos reutilizáveis
    public const string CpfValido = "52998224725";
    public const string CnpjValido = "11222333000181";
    public const string EmailValido = "test@example.com";
    public const string TelefoneValido = "11999998888";

    public static Usuario CriarUsuario(
        int idUsuario = 1,
        byte idPerfil = (byte)Perfil.Instituicao,
        int? idInstituicao = 1,
        string login = "admin",
        string senha = "hashed_password",
        bool flPrimeiroAcesso = false)
        => new(idUsuario, idPerfil, idInstituicao, login, senha, flPrimeiroAcesso);

    public static Instituicao CriarInstituicao(
        int idInstituicao = 1,
        string nome = "Instituição Teste",
        string? descricao = null,
        int idCidade = 1,
        string cnpj = CnpjValido,
        string cep = "01310100",
        string email = EmailValido,
        TimeSpan? horarioInicio = null,
        TimeSpan? horarioFim = null)
        => new(idInstituicao, nome, descricao, idCidade, cnpj, cep, email,
               horarioInicio ?? TimeSpan.FromHours(8),
               horarioFim ?? TimeSpan.FromHours(18));

    public static Agendamento CriarAgendamento(
        int idAgendamento = 1,
        int idInstituicao = 1,
        short idPratica = 1,
        DateTime? dataCriacao = null,
        byte status = (byte)StatusAgendamento.EmAndamento,
        string? observacao = null,
        string nomePaciente = "Paciente Teste",
        string cpfPaciente = CpfValido,
        string telefonePaciente = TelefoneValido,
        DateTime? dataNascimento = null,
        byte generoPaciente = (byte)Genero.Masculino,
        string emailPaciente = EmailValido,
        short idEstadoPaciente = 1,
        int idCidadePaciente = 1,
        byte grauAnsiedade = 1)
        => new(idAgendamento, idInstituicao, idPratica,
               dataCriacao ?? DateTime.Now,
               status, observacao, nomePaciente, cpfPaciente, telefonePaciente,
               dataNascimento ?? new DateTime(1990, 1, 1),
               generoPaciente, emailPaciente, idEstadoPaciente, idCidadePaciente, grauAnsiedade);

    public static Atendimento CriarAtendimento(
        int idAtendimento = 1,
        int idAgendamento = 1,
        DateTime? dataAtendimento = null,
        byte status = (byte)StatusAtendimento.Agendado,
        string queixaPaciente = "Queixa teste",
        string? observacao = null)
        => new(idAtendimento, idAgendamento,
               dataAtendimento ?? DateTime.Now,
               status, queixaPaciente, observacao);

    public static Pratica CriarPratica(
        short idPratica = 1,
        string nome = "Prática Teste",
        string? descricao = null)
        => new(idPratica, nome, descricao);

    public static PraticaInstituicao CriarPraticaInstituicao(
        short idPratica = 1,
        int idInstituicao = 1,
        byte periodicidade = (byte)Periodicidade.Semanal,
        short qtdSessoes = 4,
        byte diaPermitido = 1)
        => new(idPratica, idInstituicao, periodicidade, qtdSessoes, diaPermitido);

    public static Estado CriarEstado(
        short idEstado = 1,
        string nome = "São Paulo",
        string sigla = "SP")
        => new(idEstado, nome, sigla);

    public static Cidade CriarCidade(
        int idCidade = 1,
        short idEstado = 1,
        string nome = "São Paulo")
        => new(idCidade, idEstado, nome);

    public static Avaliacao CriarAvaliacao(
        int idAvaliacao = 1,
        int idAtendimento = 1,
        DateTime? data = null,
        string link = "https://forms.example.com/1",
        string observacao = "Boa sessão")
        => new(idAvaliacao, idAtendimento, data ?? DateTime.Now, link, observacao);
}
