namespace gerenciadorConsultasPICS.Areas.Usuario.ViewModels.Atendimento
{
    public class MeusAtendimentosViewModel
    {
        public MeusAtendimentosViewModel(
            int idAtendimento,
            string nomePratica, 
            string cidadePaciente,
            string estadoPaciente, 
            DateTime dataAtendimento,
            string statusAtendimento, 
            byte status,
            string? nomePaciente,
            string? telefonePaciente, 
            DateTime? dataNascimentoPaciente,
            TimeSpan? horarioInicioAtendimento,
            TimeSpan? horarioFimAtendimento,
            string? nomeInstituicao, 
            string? cepInstituicao)
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
            this.horarioInicioAtendimento = horarioInicioAtendimento;
            this.horarioFimAtendimento = horarioFimAtendimento;
            this.nomeInstituicao = nomeInstituicao;
            this.cepInstituicao = cepInstituicao is null ? string.Empty : FormatarCep(cepInstituicao);
        }

        public int idAtendimento { get; set; }
        public string nomePratica { get; set; }
        public string cidadePaciente { get; set; }
        public string estadoPaciente { get; set; }
        public DateTime dataAtendimento { get; set; }
        public string statusAtendimento { get; set; }
        public byte status { get; set; }
        public string? nomePaciente { get; set; }
        public string? telefonePaciente { get; set; }
        public DateTime? dataNascimentoPaciente { get; set; }
        public TimeSpan? horarioInicioAtendimento { get; set; }
        public TimeSpan? horarioFimAtendimento { get; set; }
        public string? nomeInstituicao { get; set; }
        public string? cepInstituicao { get; set; }

        private string FormatarCep(string cep)
        {
            cep = new string(cep.Where(char.IsDigit).ToArray());
            if (cep.Length == 8)
                return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";

            return cep;
        }
    }
}
