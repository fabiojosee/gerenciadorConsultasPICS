using gerenciadorConsultasPICS.Areas.Admin.Enums;
using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Usuario.Models
{
    public class Atendimento
    {
        protected Atendimento() { }

        public Atendimento(int idAtendimento, int idAgendamento, DateTime dataAtendimento, byte status, string queixaPaciente, string? observacao)
        {
            this.idAtendimento = idAtendimento;
            this.idAgendamento = idAgendamento;
            this.dataAtendimento = dataAtendimento;
            this.status = status;
            this.queixaPaciente = queixaPaciente;
            this.observacao = observacao;
        }

        [Key]
        public int idAtendimento { get; private set; }
        public int idAgendamento { get; private set; }
        public DateTime dataAtendimento { get; private set; }
        public byte status { get; private set; }
        public string queixaPaciente { get; private set; }
        public string? observacao { get; private set; }

        public void AlterarStatus(byte status) => this.status = status;

        public static class AtendimentoFactory
        {
            public static Atendimento CriarAtendimento(
                int idAgendamento,
                DateTime dataAtendimento,
                string queixaPaciente)
            {
                return new Atendimento()
                {
                    idAgendamento = idAgendamento,
                    dataAtendimento = dataAtendimento,
                    status = (byte)StatusAtendimento.Agendado,
                    queixaPaciente = queixaPaciente
                };
            }
        }
    }
}
