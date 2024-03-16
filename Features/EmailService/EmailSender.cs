using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
using Serilog;

namespace EmailServices;

public class EmailSender(EmailService emailService) : IEmailSenderCustome
{
    private readonly EmailService emailService = emailService;

    Task<bool> IEmailSenderCustome.SendConfirmationCodeAsync(User user, string email, string code)
    {
        var path = "Features/EmailService/Templets/VerifyEmail.htm";

        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "DisplayName", user.DisplayName },
            { "Code", code.ToString() },
            { "CodeLifeTime", ((int)VerificationCode.CodeMaxLifeInMin.EmailVerification).ToString() }
        };

        var emailBody = EmailService.CreateEmailFromHTMLFileWithParamters(path, parameters);


        var success = emailService.SendEmail(
             subject: "Email Confirmation",
             body: emailBody,
             receiver_email: email);

        System.Console.WriteLine(" --> Send confirmation code");
        return success;
    }

    Task<bool> IEmailSenderCustome.SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {

        var path = "Features/EmailService/Templets/ResetPassword.htm";

        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "DisplayName", user.DisplayName },
            { "Code", resetCode.ToString() },
            { "CodeLifeTime", ((int)VerificationCode.CodeMaxLifeInMin.PasswordRest).ToString() }
        };

        var emailBody = EmailService.CreateEmailFromHTMLFileWithParamters(path, parameters);


        var success = emailService.SendEmail(
            subject: "Reset password code",
           body: emailBody,
            receiver_email: email);

        System.Console.WriteLine(" --> Send password reset code");
        return success;
    }
}

public class EmailService(IConfiguration configuration)
{
    IConfiguration configuration = configuration;
    public async Task<bool> SendEmail(string subject,string body,string receiver_email)
    {
        var senderEmail = configuration.GetSection("EmailsSender").GetChildren().First(c=>c.Key == "SenderEmail").Value;
        var senderPassword = configuration.GetSection("EmailsSender").GetChildren().First(c=>c.Key == "SenderPassword").Value;
        var reciverMail = receiver_email;
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(senderEmail));   
        email.To.Add(MailboxAddress.Parse(reciverMail));    
        
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        try
        {

            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail,senderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

        }
        catch(Exception ex)
        {
            Log.Error(ex.Message);
            return false;
        }

        return true;
    }


    public static string CreateEmailFromHTMLFileWithParamters(string emailBodyFilePath,Dictionary<string,string> parameters)
    {
        string body = string.Empty;

        using (StreamReader reader = new StreamReader(emailBodyFilePath))
        {
            body = reader.ReadToEnd();
        }

        foreach(var p in parameters)
        {
            body = body.Replace("@{" + p.Key + "}", p.Value);
        }
        return body;
    }

}

public interface IEmailSenderCustome
{
    Task<bool> SendConfirmationCodeAsync(User user, string email, string code);
    Task<bool> SendPasswordResetCodeAsync(User user, string email, string resetCode);   
}

