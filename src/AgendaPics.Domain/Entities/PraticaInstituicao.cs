using AgendaPics.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class PraticaInstituicao
{
    protected PraticaInstituicao() { }

    public PraticaInstituicao(
        short idPratica,
        int idInstituicao,
        byte periodicidade,
        short qtdSessoes,
        byte diaPermitidoParaAgendamento)
    {
        IdPratica = idPratica;
        IdInstituicao = idInstituicao;
        Periodicidade = periodicidade;
        QtdSessoes = qtdSessoes;
        DiaPermitidoParaAgendamento = diaPermitidoParaAgendamento;
    }

    [ForeignKey(nameof(Pratica))]
    public short IdPratica { get; private set; }
    [ForeignKey(nameof(Instituicao))]
    public int IdInstituicao { get; private set; }
    public byte Periodicidade { get; private set; }
    public short QtdSessoes { get; private set; }
    public byte DiaPermitidoParaAgendamento { get; private set; }

    public virtual Pratica? Pratica { get; private set; }
    public virtual Instituicao? Instituicao { get; private set; }

    public void Atualizar(
        byte periodicidade,
        short qtdSessoes,
        byte diaPermitidoParaAgendamento)
    {
        Periodicidade = periodicidade;
        QtdSessoes = qtdSessoes;
        DiaPermitidoParaAgendamento = diaPermitidoParaAgendamento;
    }

    public static PraticaInstituicao Criar(
        short idPratica,
        int idInstituicao,
        byte periodicidade,
        short qtdSessoes,
        byte diaPermitidoParaAgendamento)
    {
        return new PraticaInstituicao
        {
            IdPratica = idPratica,
            IdInstituicao = idInstituicao,
            Periodicidade = periodicidade,
            QtdSessoes = qtdSessoes,
            DiaPermitidoParaAgendamento = diaPermitidoParaAgendamento
        };
    }

    public Enums.Periodicidade GetPeriodicidadeEnum() => (Enums.Periodicidade)Periodicidade;
}
