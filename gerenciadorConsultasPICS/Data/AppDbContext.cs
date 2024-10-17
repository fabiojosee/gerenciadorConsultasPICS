using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Areas.Usuario.Models;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Instituicao> Instituicao { get; set; }
        public DbSet<Pratica> Pratica { get; set; }
        public DbSet<Agendamento> Agendamento { get; set; }
        public DbSet<Atendimento> Atendimento { get; set; }
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<PraticaInstituicao> PraticaInstituicao { get; set; }
    }
}
