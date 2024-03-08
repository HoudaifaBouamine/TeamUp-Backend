using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;

namespace TeamUp_Backend.Features.EmailService;
public class EmailSender(EmailService emailService) : IEmailSender<IdentityUser>
{
    private readonly EmailService emailService = emailService;

    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
    {
        emailService.SendEmail(
            subject: "Email Confirmation",
            body: $"<h2>confirme this is you</h2><h3>link : {confirmationLink}</h3>",
            receiver_email: email);

        System.Console.WriteLine(" --> Send confirmation link");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
    {
        emailService.SendEmail(
            subject: "Reset password code",
            body: $"<h2>your reset password code : {resetCode} </h2>",
            receiver_email: email);

        System.Console.WriteLine(" --> Send password reset code");
        return Task.CompletedTask;    
    }

    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
    {
        emailService.SendEmail(
            subject: "Reset password link",
            body: $"<h2>your reset password code : {resetLink} </h2>",
            receiver_email: email);

        System.Console.WriteLine(" --> Send password reset link");
        return Task.CompletedTask;
    }
}

public class EmailService
{
    public bool SendEmail(string subject,string body,string receiver_email)
    {
        var senderMail = "houdaifa.bouamine@gmail.com";
        var senderPass = "akqldlyvuuxhbgwj";//true
        var reciverMail = receiver_email;
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(senderMail));   
        email.To.Add(MailboxAddress.Parse(reciverMail));    
        
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(senderMail,senderPass);
        smtp.Send(email);
        smtp.Disconnect(true);

        return true;
    }
}
