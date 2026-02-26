namespace AgendaPics.Web.Areas.Usuario.ViewModels.Atendimento
{
    public class MeusAtendimentosInstituicaoViewModel
    {
        public MeusAtendimentosInstituicaoViewModel(int idAtendimento, string nomePratica, string cidadePaciente, string estadoPaciente, DateTime dataAtendimento, string statusAtendimento, byte status, string nomePaciente, string telefonePaciente, DateTime dataNascimentoPaciente)
        {
            this.idAtendimento = idAtendimento;
            this.nomePratica = nomePratica;
            this.cidadePaciente = cidadePaciente;
            this.estadoPaciente = estadoPaciente;
            this.dataAtendimento = dataAtendimento;
            this.statusAtendimento = statusAtendimento;
            this.status = status;
            this.nomePaciente = nomePaciente;
            this.telefonePaciente = telefonePaciente;
            this.dataNascimentoPaciente = dataNascimentoPaciente;
        }

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
