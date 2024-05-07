// using Microsoft.AspNetCore.Authentication;
// using Microsoft.Extensions.Options;
// using Microsoft.Extensions.Logging;
// using System.Text.Encodings.Web;
// using System.Security.Claims;
//
// namespace TeamUp.Test.IntegrationTesting.Authentication;
//
// public partial class FullAuthTests
// {
//     public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//     {
//         public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
//         {
//         }
//
//         protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//         {
//             var identity = new ClaimsIdentity(Array.Empty<Claim>(),"test");
//             var principal = new ClaimsPrincipal(identity);
//             var ticket = new AuthenticationTicket(principal,"TestScheme");
//
//             var result = AuthenticateResult.Success(ticket);
//
//             return Task.FromResult(result);
//         }
//     }
//
//
// }
