using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Instituicao
    {
        protected Instituicao() { }

        public Instituicao(
            int idInstituicao,
            string nome,
            string? descricao,
            short idEstado,
            int idCidade,
            string cnpj,
            string cep,
            string email,
            TimeSpan horarioInicioAtendimento,
            TimeSpan horarioFimAtendimento)
        {
            this.idInstituicao = idInstituicao;
            this.nome = nome;
            this.descricao = descricao;
            this.idEstado = idEstado;
            this.idCidade = idCidade;
            this.cnpj = cnpj;
            this.cep = cep;
            this.email = email;
            this.horarioInicioAtendimento = horarioInicioAtendimento;
            this.horarioFimAtendimento = horarioFimAtendimento;
        }

        [Key]
        public int idInstituicao { get; private set; }
        public string nome { get; private set; }
        public string? descricao { get; private set; }
        public short idEstado { get; private set; }
        public int idCidade { get; private set; }
        public string cnpj { get; private set; }
        public string cep { get; private set; }
        public string email { get; private set; }
        public TimeSpan horarioInicioAtendimento { get; private set; }
        public TimeSpan horarioFimAtendimento { get; private set; }

        public void Atualizar(
            string nome,
            string? descricao,
            short idEstado,
            int idCidade,
            string cnpj,
            string cep,
            string email,
            TimeSpan horarioInicioAtendimento,
            TimeSpan horarioFimAtendimento)
        {
            this.nome = nome;
            this.descricao = descricao;
            this.idEstado = idEstado;
            this.idCidade = idCidade;
            this.cnpj = cnpj;
            this.cep = cep;
            this.email = email;
            this.horarioInicioAtendimento = horarioInicioAtendimento;
            this.horarioFimAtendimento = horarioFimAtendimento;
        }

        public static class InstituicaoFactory
        {
            public static Instituicao CriarInstituicao(
                string nome,
                string? descricao,
                short idEstado,
                int idCidade,
                string cnpj,
                string cep,
                string email,
                TimeSpan horarioInicioAtendimento,
                TimeSpan horarioFimAtendimento)
            {
                return new Instituicao()
                {
                    nome = nome,
                    descricao = descricao,
                    idEstado = idEstado,
                    idCidade = idCidade,
                    cnpj = cnpj,
                    cep = cep,
                    email = email,
                    horarioInicioAtendimento = horarioInicioAtendimento,
                    horarioFimAtendimento = horarioFimAtendimento
                };
            }
        }
    }
}
