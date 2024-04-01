using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace EmailServices;

public class EmailSenderMock : IEmailSenderCustome
{
    public Task<bool> SendConfirmationCodeAsync(User user, string email, string code)
    {
        return Task.FromResult(true);
    }

    public Task<bool> SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        return Task.FromResult(true);
    }
}