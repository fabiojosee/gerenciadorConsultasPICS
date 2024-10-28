namespace gerenciadorConsultasPICS.Areas.Admin.ViewModels.Pratica
{
    public class VinculoPraticaViewModel
    {
        public short? idPratica { get; set; }
        public int? idInstituicao { get; set; }
        public byte? periodicidade { get; set; }
        public short? qtdSessoes { get; set; }
        public byte? diaPermitidoParaAgendamento { get; set; }
    }
}
