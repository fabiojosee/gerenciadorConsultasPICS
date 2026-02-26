using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using AgendaPics.Infrastructure.Repositories;
using AgendaPics.Infrastructure.Security;
using AgendaPics.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgendaPics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAtendimentoRepository, AtendimentoRepository>();
        services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
        services.AddScoped<ICidadeRepository, CidadeRepository>();
        services.AddScoped<IEstadoRepository, EstadoRepository>();
        services.AddScoped<IInstituicaoRepository, InstituicaoRepository>();
        services.AddScoped<IPraticaRepository, PraticaRepository>();
        services.AddScoped<IPraticaInstituicaoRepository, PraticaInstituicaoRepository>();
        services.AddScoped<ITermoConsentimentoRepository, TermoConsentimentoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITokenAcessoRepository, TokenAcessoRepository>();

        // Security
        services.AddSingleton<IPasswordHasher, PasswordHasherBcrypt>();
        services.AddSingleton<ISecureRandomGenerator, SecureRandomGenerator>();

        // Services
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCustomMediator(
            typeof(AgendaPics.Application.Features.Autenticacao.Commands.LoginCommand).Assembly,
            typeof(AgendaPics.Application.Features.Localidades.Queries.ObterEstadosQuery).Assembly
        );

        return services;
    }
}
