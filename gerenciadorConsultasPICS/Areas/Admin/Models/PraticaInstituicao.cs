namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
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
            this.idPratica = idPratica;
            this.idInstituicao = idInstituicao;
            this.periodicidade = periodicidade;
            this.qtdSessoes = qtdSessoes;
            this.diaPermitidoParaAgendamento = diaPermitidoParaAgendamento;
        }

        public short idPratica { get; private set; }
        public int idInstituicao { get; private set; }
        public byte periodicidade { get; private set; }
        public short qtdSessoes { get; private set; }
        public byte diaPermitidoParaAgendamento { get; private set; }

        public void Atualizar(
                byte periodicidade,
                short qtdSessoes,
                byte diaPermitidoParaAgendamento)
        {
            this.periodicidade = periodicidade;
            this.qtdSessoes = qtdSessoes;
            this.diaPermitidoParaAgendamento = diaPermitidoParaAgendamento;
        }

        public static class PraticaInstituicaoFactory
        {
            public static PraticaInstituicao CriarPraticaInstituicao(
                short idPratica,
                int idInstituicao,
                byte periodicidade,
                short qtdSessoes,
                byte diaPermitidoParaAgendamento)
            {
                return new PraticaInstituicao()
                {
                    idPratica = idPratica,
                    idInstituicao = idInstituicao,
                    periodicidade = periodicidade,
                    qtdSessoes = qtdSessoes,
                    diaPermitidoParaAgendamento = diaPermitidoParaAgendamento
                };
            }
        }
    }
}
