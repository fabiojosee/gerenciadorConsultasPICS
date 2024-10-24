using System.Security.Cryptography;
using System.Text;

namespace gerenciadorConsultasPICS.Helpers
{
    public class HashHelper
    {
        public static string Criptografar(string entrada)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(entrada));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
