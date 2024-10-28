using gerenciadorConsultasPICS.Repositories;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using gerenciadorConsultasPICS.Services;
using gerenciadorConsultasPICS.Services.Interfaces;

namespace gerenciadorConsultasPICS.Configurations
{
    public static class DependencyInjection
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            #region Repositories

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
            services.AddScoped<IAtendimentoRepository, AtendimentoRepository>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();
            services.AddScoped<IEstadoRepository, EstadoRepository>();
            services.AddScoped<IInstituicaoRepository, InstituicaoRepository>();
            services.AddScoped<IPraticaRepository, PraticaRepository>();
            services.AddScoped<IPraticaInstituicaoRepository, PraticaInstituicaoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            #endregion

            #region Services

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ITokenService, TokenService>();

            #endregion
        }
    }
}
