using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace WebApplication1
{
    public class EmailHelperSMTP
    {
       /* public static void CreateTestMessage3()
        {
            MailAddress to = new MailAddress("jane@contoso.com");
            MailAddress from = new MailAddress("ben@contoso.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Using the new SMTP client.";
            message.Body = @"Using this new feature, you can send an email message from an application very easily.";
            // Use the application or machine configuration to get the
            // host, port, and credentials.
            SmtpClient client = new SmtpClient();
            Console.WriteLine("Sending an email message to {0} at {1} by using the SMTP host={2}.",
                to.User, to.Host, client.Host);
            client.Send(message);
        }*/

        public bool SendEmail(string userEmail, string emailBody)
        {
            

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("talbinadia800@gmail.com"));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = "Verifier Email";
            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);

            smtp.Authenticate("talbinadia800@gmail.com", "qdxghmobdgghonjy");

            smtp.Send(email);
            smtp.Disconnect(true);

            return true;
        }
    }
}
