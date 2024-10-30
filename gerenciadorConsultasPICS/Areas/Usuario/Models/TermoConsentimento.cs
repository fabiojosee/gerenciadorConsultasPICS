using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Usuario.Models
{
    public class TermoConsentimento
    {
        protected TermoConsentimento() { }

        public TermoConsentimento(int idAgendamento, DateTime dataConsentimento)
        {
            this.idAgendamento = idAgendamento;
            this.dataConsentimento = dataConsentimento;
        }

        [Key]
        public int idAgendamento { get; private set; }
        public DateTime dataConsentimento { get; private set; }

        public static class TermoConsentimentoFactory
        {
            public static TermoConsentimento CriarTermoConsentimento(int idAgendamento, DateTime dataConsentimento)
            {
                return new TermoConsentimento()
                {
                    idAgendamento = idAgendamento,
                    dataConsentimento = dataConsentimento
                };
            }
        }
    }
}
