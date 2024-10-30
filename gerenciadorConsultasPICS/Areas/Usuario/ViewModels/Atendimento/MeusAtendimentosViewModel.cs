namespace gerenciadorConsultasPICS.Areas.Usuario.ViewModels.Atendimento
{
    public class MeusAtendimentosViewModel
    {
        public int idAtendimento { get; set; }
        public string nomePratica { get; set; }
        public string cidadePaciente { get; set; }
        public string estadoPaciente { get; set; }
        public DateTime dataAtendimento { get; set; }
        public string statusAtendimento { get; set; }
        public byte status { get; set; }
        public string nomePaciente { get; set; }
        public string telefonePaciente { get; set; }
        public DateTime dataNascimentoPaciente { get; set; }
    }
}
