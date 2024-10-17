using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Usuario.Models
{
    public class PraticaInstituicao
    {
        public PraticaInstituicao(short idPratica, int idInstituicao, byte periodicidade, short qtdSessoes, byte diaPermitidoParaAgendamento)
        {
            this.idPratica = idPratica;
            this.idInstituicao = idInstituicao;
            this.periodicidade = periodicidade;
            this.qtdSessoes = qtdSessoes;
            this.diaPermitidoParaAgendamento = diaPermitidoParaAgendamento;
        }

        [Key]
        public Int16 idPratica { get; private set; }
        public int idInstituicao { get; private set; }
        public byte periodicidade { get; private set; }
        public Int16 qtdSessoes { get; private set; }
        public byte diaPermitidoParaAgendamento { get; private set; }
    }
}
