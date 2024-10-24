using System.Text;

namespace gerenciadorConsultasPICS.Helpers
{
    public class EmailHelper
    {
        public static string GerarCodigoAleatorio(int tamanho)
        {
            string codigo = string.Empty;
            for (int i = 0; i < tamanho; i++)
            {
                Random random = new Random();
                int numeroAleatorio = Convert.ToInt32(random.Next(48, 122).ToString());

                if ((numeroAleatorio >= 48 && numeroAleatorio <= 57) || (numeroAleatorio >= 97 && numeroAleatorio <= 122))
                {
                    string _char = ((char)numeroAleatorio).ToString();
                    if (!codigo.Contains(_char))
                    {
                        codigo += _char;
                    }
                    else
                    {
                        i--;
                    }
                }
                else
                {
                    i--;
                }
            }
            return codigo;
        }

        public static string GerarTemplateEmail(string titulo, List<string> linhas)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h1>{titulo}</h1>");
            sb.AppendLine("<p>");

            foreach (var linha in linhas)
            {
                sb.AppendLine($"{linha}<br>");
            }

            sb.AppendLine("</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}
