using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;

namespace Etapa2.Authentication
{
    public class EmailSender
    {
        private const string MailFromAddress = "pi.trello.g16@gmail.com";
        private const bool UseSsl = true;
        private const int ServerPort = 587;
        private const string ServerName = "smtp.gmail.com";
        private const string UserName = "pi.trello.g16@gmail.com";
        private const string Password = "pitrello";
        private const int Timeout = 10000;
        
        public void ProcessEmail(string emailTo, Url link)
        {
            using(var smtpClient = new SmtpClient(ServerName,ServerPort))
            {
                smtpClient.EnableSsl = UseSsl;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(UserName, Password);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Timeout = Timeout;

                var body = new StringBuilder()
                    .AppendLine("Olá,")
                    .AppendLine(string.Empty)
                    .AppendLine("Para completar o seu registo clique no link abaixo sff")
                    .AppendLine(link.Value)
                    .AppendLine(string.Empty)
                    .AppendLine("Atentamente,")
                    .AppendLine(string.Empty)
                    .AppendLine("PI TRELLO ADMINISTRATION");

                var mailMessage = new MailMessage(
                            MailFromAddress,
                            emailTo,    
                            "PI_Trello: Novo utilizador!",
                            body.ToString()
                    );

                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                smtpClient.Send(mailMessage);
            }
        }
    }
}