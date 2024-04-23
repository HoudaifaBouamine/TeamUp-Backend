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

#region HTML
    string body = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Email Template</title>

<style>

/* Define web-safe fonts */
body {
    font-family: Arial, sans-serif;
}

.container {
  width: 100%;
  margin: 0 auto;
  background: #F5F5F5;
  padding: 48px;
}

.card {
  width: 600px;
  border-radius: 32px;
  overflow: hidden;
  margin: 0;
  padding: 96px 48px;
  background-color: white;
}

.headline {
  text-align: center;
  color: #141414;
  font-size: 40px;
  font-weight: 700;
  line-height: 48px;
  margin-bottom: 24px;
  max-width: 60ch;
}

.eyebrow {
  text-align: center;
  color: #3A00E5;
  font-size: 16px;
  font-weight: 600;
  line-height: 24px;
  letter-spacing: 0.32px;
  margin-bottom: 16px;
}

.body-text {
  text-align: center;
  color: #595959;
  font-size: 20px;
  font-weight: 400;
  max-width: 50ch;
  line-height: 34px;
  margin: 0 auto; /* Center align horizontally */
  width: 100%; /* Fill the container */
  max-width: none; /* Remove the maximum width constraint */
}

.pin-code {
  text-align: center;
  color: black;
  font-size: 40px;
  font-weight: 700;
  line-height: 56px;
  letter-spacing: 4.80px;
  margin-top: 32px;
  margin-bottom: 32px;
}

.caption {
  text-align: center;
  color: #8C8C8C;
  font-size: 12px;
  font-weight: 400;
  line-height: 30px;
  max-width: 60ch;
  margin: 0 auto;
}

.footer {
    width: 696px;
    text-align: center;
    padding: 64px 0; /* Increased padding for extra negative space (8 units) */
    
}

.logo img {
  height: 48px;
  width: auto;

}

.social-media {
  margin-top: 32px; /* Increased margin-top for extra space (4 units) */
}

.social-icon {
  display: inline-block;
  margin: 0 24px; /* Increased margin between social links (4 units) */
}

.social-icon img {
  width: 32px;
  height: auto;
}


.copyrights {
  color: #777777;
  font-size: 12px;
  margin-top: 24px;
  line-height: 24px;
}

@media only screen and (max-width: 600px) {
  .card {
    border-radius: 12px;
    padding: 48px;
    width: 360px;
  }

  .headline {
    font-size: 24px;
    line-height: 28px;
    margin-bottom: 16px;
  }

  .eyebrow {
    font-size: 14px;
    margin-bottom: 8px;
  }

  .body-text {
    font-size: 14px;
    line-height: 22px;
    max-width: 40ch;
    margin-bottom: 24px;
  }

  .pin-code {
    font-size: 20px;
    line-height: 28px;
    margin-top: 12px;
    margin-bottom: 12px;
  }

  .caption {
    font-size: 12px;
    line-height: 16px;
    max-width: 90%;
    margin-bottom: 16px;
  }

  .footer {
/* Increased margin-top for extra space (8 units) */
    padding: 32px 0; 
    width: 465px;
    /* Increased padding for extra negative space (4 units) */
  }

  .logo img {
  height: 32px;
  width: auto;
}

  .social-icon img {
    width: 24px;
  }
}

</style>
</head>
<body>
<div class=""container"">

  <div class=""card"" id=""email-card"">
        <div class=""eyebrow"">Welcome To TeamUp!</div>
      <div class=""headline"" >Confirm your email</div>
      <div class=""body-text"">Hi @{DisplayName}!<br/><br/>Thanks for signing up for TeamUp! To access all the features at TeamUp, verify your email address by entering the PIN code</div>
      <div class=""pin-code"">@{Code}</div>
      <div class=""caption"">The code expires in 10 minutes.</div>
   
  </div>

  <div class=""footer"" id=""email-footer"">
    <div class=""logo"">
      <img src=""https://drive.google.com/thumbnail?sz=w1000&id=1raH-Wmf4GRR8vIQgz9p0M4Z1tRnk7GLF"" alt="""">
    </div>
    <div class=""social-media"">
      <div class=""social-icon""><img src=""https://drive.google.com/thumbnail?sz=w1000&id=1kXKgsoVMLY7LRA2_0C6d7b-plwxQfUvB""></div>
      <div class=""social-icon""><img src=""https://drive.google.com/thumbnail?sz=w1000&id=1DdyHzrCZWGpQw7qYJtEtrTqaqfjJ6JHJ""></div>
      <div class=""social-icon""><img src=""https://drive.google.com/thumbnail?sz=w1000&id=1JoCCQ_wOW4oMHdG504rwZgEprTpT0ieO""></div>
    </div>
    <div class=""copyrights"">Copyright © 2024 TeamUp, All rights reserved.<br>You are receiving this email because you opted-in via our website.</div>
  </div>
</div>
</body>
</html>
";
#endregion

    // var body = File.ReadAllText("Features/EmailService/Templets/VerifyEmail.htm");

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
#region HTML
        string htmlCode = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Email Template</title>

<style>

/* Define web-safe fonts */
body {
    font-family: Arial, sans-serif;
}

.container {
  width: 100%;
  margin: 0 auto;
  background: #F5F5F5;
  padding: 48px;
}

.card {
  width: 600px;
  border-radius: 32px;
  overflow: hidden;
  margin: 0;
  padding: 96px 48px;
  background-color: white;
}

.headline {
  text-align: center;
  color: #141414;
  font-size: 40px;
  font-weight: 700;
  line-height: 48px;
  margin-bottom: 24px;
  max-width: 60ch;
}

.eyebrow {
  text-align: center;
  color: #3A00E5;
  font-size: 16px;
  font-weight: 600;
  line-height: 24px;
  letter-spacing: 0.32px;
  margin-bottom: 16px;
}

.body-text {
  text-align: center;
  color: #595959;
  font-size: 20px;
  font-weight: 400;
  max-width: 50ch;
  line-height: 34px;
  margin: 0 auto; /* Center align horizontally */
  width: 100%; /* Fill the container */
  max-width: none; /* Remove the maximum width constraint */
}

.pin-code {
  text-align: center;
  color: black;
  font-size: 40px;
  font-weight: 700;
  line-height: 56px;
  letter-spacing: 4.80px;
  margin-top: 32px;
  margin-bottom: 32px;
}

.caption {
  text-align: center;
  color: #8C8C8C;
  font-size: 14px;
  font-weight: 400;
  line-height: 28px;
  max-width: 50ch;
  margin: 0 auto;
}

.footer {
    width: 696px;
    text-align: center;
    padding: 64px 0; /* Increased padding for extra negative space (8 units) */
    
}

.logo img {
  height: 48px;
  width: auto;

}

.social-media {
  margin-top: 32px; /* Increased margin-top for extra space (4 units) */
}

.social-icon {
  display: inline-block;
  margin: 0 24px; /* Increased margin between social links (4 units) */
}

.social-icon img {
  width: 32px;
  height: auto;
}


.copyrights {
  color: #777777;
  font-size: 12px;
  margin-top: 24px;
  line-height: 24px;
}

@media only screen and (max-width: 600px) {
  .card {
    border-radius: 12px;
    padding: 48px;
    width: 360px;
  }

  .headline {
    font-size: 24px;
    line-height: 28px;
    margin-bottom: 16px;
  }

  .eyebrow {
    font-size: 14px;
    margin-bottom: 8px;
  }

  .body-text {
    font-size: 14px;
    line-height: 22px;
    max-width: 40ch;
    margin-bottom: 24px;
  }

  .pin-code {
    font-size: 20px;
    line-height: 28px;
    margin-top: 12px;
    margin-bottom: 12px;
  }

  .caption {
    font-size: 12px;
    line-height: 16px;
    max-width: 90%;
    margin-bottom: 16px;
  }

  .footer {
/* Increased margin-top for extra space (8 units) */
    padding: 32px 0; 
    width: 465px;
    /* Increased padding for extra negative space (4 units) */
  }

  .logo img {
  height: 32px;
  width: auto;
}

  .social-icon img {
    width: 24px;
  }
}

</style>
</head>
<body>
<div class=""container"">

  <div class=""card"" id=""email-card"">
        <div class=""eyebrow"">Reset Your Password</div>
      <div class=""headline"" >Forgot your password?</div>
      <div class=""body-text"">Hi @{DisplayName}!<br/><br/>If you've forgotten your password. Follow the instructions below to reset your password and regain access to your account.</div>
      <div class=""pin-code"">@{Code}</div>
      <div class=""caption"">This code expires in 10 minutes.</div>
   
  </div>

  <div class=""footer"" id=""email-footer"">
    <div class=""logo"">
      <img src=""https://drive.google.com/thumbnail?sz=w1000&id=1raH-Wmf4GRR8vIQgz9p0M4Z1tRnk7GLF"" alt="""">
    </div>
    <div class=""social-media"">
      <div class=""social-icon""><img src=""https://drive.google.com/thumbnail?sz=w1000&id=1kXKgsoVMLY7LRA2_0C6d7b-plwxQfUvB""></div>
      <div class=""social-icon""><img src=""https://drive.google.com/thumbnail?sz=w1000&id=1DdyHzrCZWGpQw7qYJtEtrTqaqfjJ6JHJ""></div>
      <div class=""social-icon""><img src=""https://drive.google.com/thumbnail?sz=w1000&id=1JoCCQ_wOW4oMHdG504rwZgEprTpT0ieO""></div>
    </div>
    <div class=""copyrights"">Copyright © 2024 TeamUp, All rights reserved.<br>You are receiving this email because you requested a password reset.</div>
  </div>
</div>
</body>
</html>
";
#endregion

        // var htmlCode = File.ReadAllText("Features/EmailService/Templets/ResetPassword.htm");


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

