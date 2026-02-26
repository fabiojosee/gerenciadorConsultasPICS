namespace AgendaPics.Application.Common.Interfaces;

public interface ISecureRandomGenerator
{
    string GenerateCode(int length);
    string GenerateToken(int byteLength = 32);
    byte[] GenerateBytes(int length);
}
