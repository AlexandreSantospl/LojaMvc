using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SetorDeCompras.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]!);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(senderEmail!);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(smtpServer, port))
                {
                    smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                    Console.WriteLine("Email enviado com sucesso.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar email: {ex.Message}");
        }
    }

}
