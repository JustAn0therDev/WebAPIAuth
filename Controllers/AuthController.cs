using System;
using WebAPIAuth.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPIAuth.BusinessRules;
using Microsoft.Extensions.Logging;
using WebAPIAuth.Responses.UserSession;
using WebAPIAuth.Responses.GenericResponses;

namespace WebAPIAuth.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {
        private readonly UserSessionBO _userSessionBO;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger) {
            _logger = logger;
            _userSessionBO = new UserSessionBO();
        }

        [HttpPost]
        public async ValueTask<IActionResult> PostAsync(User user) {
            AuthenticateUserResponse resp = new AuthenticateUserResponse();
            try {
                resp.UserSessionID = await _userSessionBO.StartSession(user);
            }
            catch (ArgumentNullException aex) {
                return BadRequest(new ErrorResponse(aex.Message));
            }
            catch (InvalidOperationException inex) {
                return BadRequest(new ErrorResponse(inex.Message));
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorResponse(ex.Message));
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("logout")]
        public async ValueTask<IActionResult> PostEndSessionAsync([FromHeader] int userSessionID)
        {
            LogoutUserResponse response = new LogoutUserResponse();
            try {
                response.LoggedOut = await _userSessionBO.EndSession(userSessionID);
            }
            catch (UnauthorizedAccessException inex) {
                return BadRequest(new ErrorResponse(inex.Message));
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorResponse(ex.Message));
            }

            return Ok(response);
        }
    }
}