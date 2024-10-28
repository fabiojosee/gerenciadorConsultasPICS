namespace gerenciadorConsultasPICS.Areas.Admin.ViewModels.Pratica
{
    public class MinhasPraticasViewModel
    {
        public short idPratica { get; set; }
        public int idInstituicao { get; set; }
        public string nome { get; set; }
        public byte periodicidade { get; set; }
        public short qtdSessoes { get; set; }
        public byte diaPermitidoParaAgendamento { get; set; }
        public string textoPeriodicidade { get; set; }
        public string textoQtdSessoes { get; set; }
        public string textoDiaPermitidoParaAgendamento { get; set; }
    }
}
