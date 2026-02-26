namespace AgendaPics.Application.Common.Interfaces;

public interface IAccessTokenService
{
    Task<string> GerarTokenAsync(string cpf, string email);
    Task<bool> ValidarTokenAsync(string cpf, string token);
}
