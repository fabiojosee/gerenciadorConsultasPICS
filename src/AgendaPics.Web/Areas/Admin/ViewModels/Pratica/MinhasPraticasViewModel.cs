namespace AgendaPics.Web.Areas.Admin.ViewModels.Pratica
{
    public class MinhasPraticasViewModel
    {
        public short idPratica { get; set; }
        public int idInstituicao { get; set; }
        public string nome { get; set; } = string.Empty;
        public byte periodicidade { get; set; }
        public short qtdSessoes { get; set; }
        public byte diaPermitidoParaAgendamento { get; set; }
        public string textoPeriodicidade { get; set; } = string.Empty;
        public string textoQtdSessoes { get; set; } = string.Empty;
        public string textoDiaPermitidoParaAgendamento { get; set; } = string.Empty;
    }
}
