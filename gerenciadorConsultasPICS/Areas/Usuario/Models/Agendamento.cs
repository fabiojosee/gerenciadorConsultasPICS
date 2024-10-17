using gerenciadorConsultasPICS.Areas.Admin.Enums;
using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Usuario.Models
{
    public class Agendamento
    {
        protected Agendamento() { }

        public Agendamento(int idAgendamento, int idInstituicao, short idPratica, DateTime dataCriacao, byte status, string? observacao, string nomePaciente, string cpfPaciente, string telefonePaciente, DateTime dataNascimentoPaciente, byte generoPaciente, string emailPaciente, Int16 idEstadoPaciente, int idCidadePaciente, byte grauAnsiedadePaciente)
        {
            this.idAgendamento = idAgendamento;
            this.idInstituicao = idInstituicao;
            this.idPratica = idPratica;
            this.dataCriacao = dataCriacao;
            this.status = status;
            this.observacao = observacao;
            this.nomePaciente = nomePaciente;
            this.cpfPaciente = cpfPaciente;
            this.telefonePaciente = telefonePaciente;
            this.dataNascimentoPaciente = dataNascimentoPaciente;
            this.generoPaciente = generoPaciente;
            this.emailPaciente = emailPaciente;
            this.idEstadoPaciente = idEstadoPaciente;
            this.idCidadePaciente = idCidadePaciente;
            this.grauAnsiedadePaciente = grauAnsiedadePaciente;
        }

        [Key]
        public int idAgendamento { get; private set; }
        public int idInstituicao { get; private set; }
        public Int16 idPratica { get; private set; }
        public DateTime dataCriacao { get; private set; }
        public byte status { get; private set; }
        public string? observacao { get; private set; }
        public string nomePaciente { get; private set; }
        public string cpfPaciente { get; private set; }
        public string telefonePaciente { get; private set; }
        public DateTime dataNascimentoPaciente { get; private set; }
        public byte generoPaciente { get; private set; }
        public string emailPaciente { get; private set; }
        public Int16 idEstadoPaciente { get; private set; }
        public int idCidadePaciente { get; private set; }
        public byte grauAnsiedadePaciente { get; private set; }

        public void AlterarStatus(byte status) => this.status = status;

        public static class AgendamentoFactory
        {
            public static Agendamento CriarAgendamento(
                int idInstituicao,
                short idPratica,
                string nomePaciente,
                string cpfPaciente,
                string telefonePaciente,
                DateTime dataNascimentoPaciente,
                byte generoPaciente,
                string emailPaciente,
                short idEstadoPaciente,
                int idCidadePaciente,
                byte grauAnsiedadePaciente)
            {
                return new Agendamento()
                {
                    idInstituicao = idInstituicao,
                    idPratica = idPratica,
                    dataCriacao = DateTime.Now,
                    status = (byte)StatusAgendamento.EmAndamento,
                    nomePaciente = nomePaciente,
                    cpfPaciente = cpfPaciente,
                    telefonePaciente = telefonePaciente,
                    dataNascimentoPaciente = dataNascimentoPaciente,
                    generoPaciente = generoPaciente,
                    emailPaciente = emailPaciente,
                    idEstadoPaciente = idEstadoPaciente,
                    idCidadePaciente = idCidadePaciente,
                    grauAnsiedadePaciente = grauAnsiedadePaciente
                };
            }
        }
    }
}
