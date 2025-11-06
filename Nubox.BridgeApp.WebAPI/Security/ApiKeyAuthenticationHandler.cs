using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Nubox.BridgeApp.WebAPI.Security
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "ApiKeyScheme";
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration) : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("x-apy-key", out var provided))
                return Task.FromResult(AuthenticateResult.Fail("Api Key no encontrada"));

            var appApiKey = _configuration["Security:ApiKey"];
            if (string.IsNullOrWhiteSpace(appApiKey) || provided != appApiKey)
                return Task.FromResult(AuthenticateResult.Fail("Api Key invalida"));

            var identity = new ClaimsIdentity(SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));

        }
    }
}
