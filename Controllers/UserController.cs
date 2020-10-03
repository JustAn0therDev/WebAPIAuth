using System;
using WebAPIAuth.Models;
using WebAPIAuth.Interfaces;
using System.Threading.Tasks;
using WebAPIAuth.BusinessRules;
using Microsoft.AspNetCore.Mvc;
using WebAPIAuth.Requests.User;
using WebAPIAuth.Responses.User;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAPIAuth.Responses.GenericResponses;
namespace WebAPIAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase, IRequestObservationSubject {

        private readonly UserSessionBO _userSessionBO;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger) {
            _logger = logger;
            _userSessionBO = new UserSessionBO();
        }

        [HttpGet]
        public async ValueTask<IActionResult> GetAsync([FromHeader] int userSessionId) {
            List<User> users = null;
            try {
                await NotifyObserver(userSessionId);
                users = await Task.FromResult(await UserBO.GetAllUsersAsync());
            }
            catch (UnauthorizedAccessException uanex) {
                _logger.LogInformation(uanex.Message);
                return BadRequest(new ErrorResponse(uanex.Message));
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
            return Ok(users);
        }

        [HttpPost]
        public async ValueTask<IActionResult> PostAsync(User user) {
            CreateUserResponse resp = new CreateUserResponse();

            try {
                resp.UserID = await UserBO.CreateUserAsync(user);
            }
            catch (InvalidOperationException inex) {
                _logger.LogInformation(inex.Message);
                return BadRequest(new ErrorResponse(inex.Message));
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorResponse(ex.Message));
            }

            return Ok(resp.UserID);
        }

        [HttpDelete]
        public async ValueTask<IActionResult> DeleteAsync(DeleteUser deleteUser, [FromHeader] int userSessionId) {
            DeleteUserResponse resp = new DeleteUserResponse();
            try {
                await NotifyObserver(userSessionId);
                resp.Deleted = await UserBO.DeleteUserAsync(deleteUser.ID);
            }
            catch (InvalidOperationException inex) {
                _logger.LogInformation(inex.Message);
                return BadRequest(new ErrorResponse(inex.Message));
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorResponse(ex.Message));
            }

            return Ok(resp);
        }

        public async ValueTask NotifyObserver(int userSessionId) => await _userSessionBO.OnNotified(userSessionId);
    }
}