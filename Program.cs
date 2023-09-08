using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(new JsonFormatter())
    .Enrich.FromLogContext()
    .CreateLogger();

var jwtParts = new List<string> {
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9",
    "eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ",
    "SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
};

var jwtString = string.Join('.', jwtParts);

var token = new JwtSecurityToken(jwtString);
var identity = new ClaimsIdentity();

foreach (var claim in token.Claims)
{
    if (claim.Type == "iat")
    {
        Claim c = new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer);
        identity.AddClaim(c);
        using (LogContext.PushProperty("Claim", claim, true))
            Log.Information($"Added claim \"{claim.Type}\"");
        // identity.AddClaim(claim.Clone());
    }
}

using (LogContext.PushProperty("Identity", identity, true))
    Log.Information("Populated identity with claims");

string json = JsonConvert.SerializeObject(identity);
System.Console.WriteLine(json);
