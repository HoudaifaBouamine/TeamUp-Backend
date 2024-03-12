using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;

namespace EmailServices;

public class EmailSender(EmailService emailService) : IEmailSenderCustome<User>, IEmailSender<IdentityUser> 
{
    private readonly EmailService emailService = emailService;

    public Task SendConfirmationCodeAsync(IdentityUser user, string email, string code)
    {
        emailService.SendEmail(
            subject: "Email Confirmation",
            body: $"<h3>Confirmation code : {code}</h3>" +
            $"<h5>Expired in {(int)VerificationCode.CodeMaxLifeInMin.EmailVerification} minutes",
            receiver_email: email);

        System.Console.WriteLine(" --> Send confirmation code");
        return Task.CompletedTask;
    }

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
            body: $"<h2>your reset password code : {resetCode} </h2>" +
            $"<h5>Expired in {(int)VerificationCode.CodeMaxLifeInMin.PasswordRest} minutes",
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

    Task IEmailSenderCustome<User>.SendConfirmationCodeAsync(User user, string email, string confirmationLink)
    {
        return SendConfirmationCodeAsync(user,email,confirmationLink);
    }

    Task IEmailSenderCustome<User>.SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        return SendPasswordResetCodeAsync(user,email,resetCode);
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



//
// Summary:
//     This API supports the ASP.NET Core Identity infrastructure and is not intended
//     to be used as a general purpose email abstraction. It should be implemented by
//     the application so the Identity infrastructure can send confirmation and password
//     reset emails.
public interface IEmailSenderCustome<TUser> where TUser : class
{
    

    //
    // Summary:
    //     This API supports the ASP.NET Core Identity infrastructure and is not intended
    //     to be used as a general purpose email abstraction. It should be implemented by
    //     the application so the Identity infrastructure can send confirmation emails.
    //
    //
    // Parameters:
    //   user:
    //     The user that is attempting to confirm their email.
    //
    //   email:
    //     The recipient's email address.
    //
    //   confirmationLink:
    //     The link to follow to confirm a user's email. Do not double encode this.
    Task SendConfirmationCodeAsync(TUser user, string email, string code);

    //
    // Summary:
    //     This API supports the ASP.NET Core Identity infrastructure and is not intended
    //     to be used as a general purpose email abstraction. It should be implemented by
    //     the application so the Identity infrastructure can send password reset emails.
    //
    //
    // Parameters:
    //   user:
    //     The user that is attempting to reset their password.
    //
    //   email:
    //     The recipient's email address.
    //
    //   resetCode:
    //     The code to use to reset the user password. Do not double encode this.
    Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode);
    
}

