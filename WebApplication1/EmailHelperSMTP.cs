using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace WebApplication1
{
    public class EmailHelperSMTP
    {

        public bool SendEmail(string userEmail, string emailBody, IConfiguration configuration)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("talbinadia800@gmail.com"));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = "Verifier Email";
            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);

            smtp.Authenticate("talbinadia800@gmail.com", "qdxghmobdgghonjy");

            smtp.Send(email);
            smtp.Disconnect(true);

            return true;
        }
    }
}
