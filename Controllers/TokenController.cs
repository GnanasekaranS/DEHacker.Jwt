using Microsoft.AspNetCore.Http;

using System.Net;
//using System.Web.Http;
using DEHacker.Jwt.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using DEHacker.Businesslogic;
using DEHacker.Jwt.Helpers;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace DEHacker.Jwt.Controllers
{
    [Produces("application/json")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        private readonly ILogger<TokenController> _logger;
        private readonly IUserService _userService;
        private readonly TelemetryClient _telemetryClient;
        public TokenController(ILogger<TokenController> logger, IUserService userService,TelemetryClient telemetryClient)
        {
            _logger = logger;
            _userService = userService;
            _telemetryClient = telemetryClient;
        }
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [AllowAnonymous]
        public IActionResult Post([Microsoft.AspNetCore.Mvc.FromBody] AuthenticateRequest userCredential)
        {
            _telemetryClient.TrackEvent("Post Authentication", new Dictionary<string, string>() { { "userCredential.Username", userCredential.Username }, { "userCredential.Password", userCredential.Password } });
            _logger.LogInformation("Entering into Authentication method");
            var authenticateResponse = _userService.Authenticate(userCredential);
            if (authenticateResponse != null && authenticateResponse.Token != null)
            {

                _logger.LogInformation("Token is being generated");
                return new JsonResult(new { token = authenticateResponse.Token }) { StatusCode = StatusCodes.Status200OK };

            }
            else
            {
                _logger.LogError("Unauthorized issue - Invalid user credentials");
                return new JsonResult(new { message = "Invalid user credentials" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
