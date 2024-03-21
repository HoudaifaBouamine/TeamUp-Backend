using System.Diagnostics;
using FluentEmail.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
using Models;
using Serilog;

namespace EmailServices;

public class EmailSender(EmailService emailService) : IEmailSenderCustome
{
    private readonly EmailService emailService = emailService;

    Task<bool> IEmailSenderCustome.SendConfirmationCodeAsync(User user, string email, string code)
    {
        // var path = "Features/EmailService/Templets/VerifyEmail.htm";
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "DisplayName", user.DisplayName },
            { "Code", code.ToString() },
            { "CodeLifeTime", ((int)VerificationCode.CodeMaxLifeInMin.EmailVerification).ToString() }
        };

        string body = @"
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title></title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        .header {
            width: 700px;
            height: 120px;
            align-self: stretch;
            background: var(--Main-piccolo, #682DFE);
        }
        .header .welcome {
            height: 63px;
            flex-shrink: 0;
            color: var(--Main-goten, #FFF);
            text-align: center;
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 32px;
            font-style: normal;
            font-weight: 600;
            line-height: 64px;
            text-align: center;
            vertical-align: middle;
            line-height: 120px;
        }
        .body {
            height: 571px;
            align-self: stretch;
            background: var(--Main-goten, #FFF);
            padding-left: 79px;
            padding-right: 79px;
        }
        .footer {
            height: 158px;
            align-self: stretch;
            background: var(--black-night, #070707);
        }
        .container {
            width: 700px;
        }
        .confrm-your-email {
            flex-shrink: 0;
            color: var(--Main-trunks, #595D62);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 32px;
            font-style: normal;
            font-weight: 600;
            line-height: 64px;
            text-align: center;
            vertical-align: middle;
            line-height: 120px;
        }
        .Hi {
            width: 383px;
            height: 65px;
            color: var(--Main-bulma, #000);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 24px;
            font-style: normal;
            font-weight: 600;
            line-height: 64px;
        }
        .Code {
            width: 436px;
            height: 112px;
            flex-shrink: 0;
            color: var(--Main-bulma, #000);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: ""DM Sans"";
            font-size: 64px;
            font-style: normal;
            font-weight: 700;
            line-height: 72px;
            text-align: center;
            vertical-align: middle;
            margin-top: 80px;
            margin-bottom: 80px;
        }
        .expires-in {
            color: var(--Main-bulma, #000);
            text-align: center;
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 16px;
            font-style: normal;
            font-weight: 400;
            line-height: 30px;
            text-align: center;
            vertical-align: middle;
        }
        .logo {
            margin: 47px;
        }
        .teamup-com {
            display: inline-block;
            width: 112px;
            height: 23px;
            color: rgba(255, 255, 255, 0.60);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 13px;
            font-style: normal;
            font-weight: 400;
            line-height: 30px;
            letter-spacing: 2px;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <p class=""welcome"">Welcome To TeamUp !</p>
        </div>
        <div class=""body"">
            <div class=""confrm-your-email"">Confirm Your Email</div>
            <div class=""Hi"">Hi, @{DisplayName}</div>
            Thanks for signing up for TeamUp! To access all the features and start building your dream team, please verify your email address by entering the PIN code
            <div class=""Code"">@{Code}</div>
            <div class=""expires-in"">The code expires in @{CodeLifeTime} minutes.</div>
        </div>
        <div class=""footer"">
            <img src=""https://i.ibb.co/GJKb5b3/Primery-1.png"" alt="""" class=""logo"">
            <p class=""teamup-com"">TeamUp.com</p>
        </div>
    </div>
</body>
</html>
";


        var emailBody = EmailService.CreateEmailFromStringWithParamters(body, parameters);


        var success = emailService.SendEmail(
             subject: "Email Confirmation",
             body: emailBody,
             receiver_email: email);

        System.Console.WriteLine(" --> Send confirmation code");
        return success;
    }

    Task<bool> IEmailSenderCustome.SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {

        // var path = "Features/EmailService/Templets/ResetPassword.htm";

        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "DisplayName", user.DisplayName },
            { "Code", resetCode.ToString() },
            { "CodeLifeTime", ((int)VerificationCode.CodeMaxLifeInMin.PasswordRest).ToString() }
        };

string htmlCode = @"
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title></title>

    <style>
        *{
            margin:0;
            padding: 0;
            box-sizing: border-box;
            
        }

        .header
        {
            width: 700px;
            height: 120px;
            align-self: stretch;
            background: var(--Main-piccolo, #682DFE); 
        }

        .header .welcome
        {
            height: 63px;
            flex-shrink: 0;
            color: var(--Main-goten, #FFF);
            text-align: center;
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 32px;
            font-style: normal;
            font-weight: 600;
            line-height: 64px;
            text-align: center;
            vertical-align: middle;
            line-height: 120px;
        }

        .body
        {
            height: 571px;
            align-self: stretch; 
            background: var(--Main-goten, #FFF); 

            padding-left: 79px;
            padding-right: 79px;

        }

        .footer
        {
            height: 158px;
            align-self: stretch; 
            background: var(--black-night, #070707); 
        }

        .container
        {
            width: 700px;
        }

        .confrm-your-email
        {
            flex-shrink: 0; 
            color: var(--Main-trunks, #595D62);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 32px;
            font-style: normal;
            font-weight: 600;
            line-height: 64px; /* 200% */ 
            text-align: center;
            vertical-align: middle;
            line-height: 120px;
        }

        .Hi
        {
            width: 383px;
            height: 65px;
            color: var(--Main-bulma, #000);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 24px;
            font-style: normal;
            font-weight: 600;
            line-height: 64px; /* 266.667% */
        }

        .Code
        {
            width: 436px;
            height: 112px;
            flex-shrink: 0; 
            color: var(--Main-bulma, #000);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: ""DM Sans"";
            font-size: 64px;
            font-style: normal;
            font-weight: 700;
            line-height: 72px; /* 112.5% */

            text-align: center;
            vertical-align: middle;
            
            margin-top: 80px;
            margin-bottom: 80px;

        }

        .expires-in
        {
            color: var(--Main-bulma, #000);
            text-align: center;
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 16px;
            font-style: normal;
            font-weight: 400;
            line-height: 30px; /* 187.5% */ 

            text-align: center;
            vertical-align: middle;

            
        }

        .logo
        {
            margin: 47px;
        }

        .teamup-com
        {
            display: inline-block;
            width: 112px;
            height: 23px;
            color: rgba(255, 255, 255, 0.60);
            font-feature-settings: 'clig' off, 'liga' off;
            font-family: Montserrat;
            font-size: 13px;
            font-style: normal;
            font-weight: 400;
            line-height: 30px; /* 230.769% */
            letter-spacing: 2px;
        }
    </style>
</head>
<body>

    <div class=""container"">

    

    <div class=""header"">
        <p class=""welcome"">Welcome To TeamUp !</p>
    </div>

    <div class=""body"">

        <div class=""confrm-your-email"">
            Teset Your Password
        </div>

        <div class=""Hi"">Hi, @{DisplayName}</div>
        You have requested password reset code , please past the following code there :
        
        <div class=""Code"">@{Code}</div>

        <div class=""expires-in"">
            The code expires in @{CodeLifeTime} minutes.

        </div>
    </div>

    <div class=""footer"">
        <img src=""https://i.ibb.co/GJKb5b3/Primery-1.png"" alt="""" class=""logo"">
        <p class=""teamup-com"">TeamUp.com</p>
    </div>

    </div>
</body>
</html>
";
        var emailBody = EmailService.CreateEmailFromStringWithParamters(htmlCode, parameters);


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

    public static string CreateEmailFromStringWithParamters(string emailString,Dictionary<string,string> parameters)
    {
        string body = emailString;

        
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

