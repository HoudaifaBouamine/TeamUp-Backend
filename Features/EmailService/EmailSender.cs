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
    async Task<bool> IEmailSenderCustome.SendConfirmationCodeAsync(User user, string email, string code)
    {
        var path = "Features/EmailService/Templets/VerifyEmail.htm";
        
        Dictionary<string,string> parameters = new Dictionary<string, string>();
        parameters.Add("DisplayName",user.DisplayName);
        parameters.Add("Code",code.ToString());
        parameters.Add("CodeLifeTime",((int)VerificationCode.CodeMaxLifeInMin.EmailVerification).ToString());
        
        var emailBody = EmailService.CreateEmailWithParamters(path,parameters);
        

       var success = emailService.SendEmail(
            subject: "Email Confirmation",
            // body: $"<h3>Confirmation code : {code}</h3>" +
            // $"<h5>Expired in {(int)VerificationCode.CodeMaxLifeInMin.EmailVerification} minutes",
            body:emailBody,
            receiver_email: email);

        System.Console.WriteLine(" --> Send confirmation code");
        return success;
    }

    async Task<bool> IEmailSenderCustome.SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {

        var path = "Features/EmailService/Templets/ResetPassword.htm";
        
        Dictionary<string,string> parameters = new Dictionary<string, string>();
        parameters.Add("DisplayName",user.DisplayName);
        parameters.Add("Code",resetCode.ToString());
        parameters.Add("CodeLifeTime",((int)VerificationCode.CodeMaxLifeInMin.PasswordRest).ToString());
        
        var emailBody = EmailService.CreateEmailWithParamters(path,parameters);
        

        var success = emailService.SendEmail(
            subject: "Reset password code",
            // body: $"<h2>your reset password code : {resetCode} </h2>" +
            // $"<h5>Expired in {(int)VerificationCode.CodeMaxLifeInMin.PasswordRest} minutes",
           body:emailBody,
            receiver_email: email);

        System.Console.WriteLine(" --> Send password reset code");
        return success;     
    }
}

public class EmailService(IConfiguration configuration)
{
    IConfiguration configuration = configuration;
    public bool SendEmail(string subject,string body,string receiver_email)
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

        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(senderEmail,senderPassword);
        smtp.Send(email);
        smtp.Disconnect(true);

        }
        catch(Exception ex)
        {
            Log.Error(ex.Message);
            return false;
        }

        return true;
    }


    public static string CreateEmailWithParamters(string emailBodyFilePath,Dictionary<string,string> parameters)
    {
        string body = string.Empty;
        //"Features/EmailService/Templets/ResetPassword.htm"
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



//
// Summary:
//     This API supports the ASP.NET Core Identity infrastructure and is not intended
//     to be used as a general purpose email abstraction. It should be implemented by
//     the application so the Identity infrastructure can send confirmation and password
//     reset emails.
public interface IEmailSenderCustome
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
    Task<bool> SendConfirmationCodeAsync(User user, string email, string code);

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
    Task<bool> SendPasswordResetCodeAsync(User user, string email, string resetCode);
    
}

