using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newtrialFYPbackend.Authentication;
using newtrialFYPbackend.Responses;
using newtrialFYPbackend.Responses.Enums;
using newtrialFYPbackend.Services.Interface;
using System.Threading.Tasks;

namespace newtrialFYPbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;
        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        [HttpPost("SignUp")]
        public async Task<ActionResult<ApiResponse>> Register(RegisterModel registerModel)
        {

            var response = await _authenticationServices.RegisterUser(registerModel);
            if (response.Message == ApiResponseEnum.success.ToString())
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [HttpPost("Validate")]
        public async Task<ActionResult<ApiResponse>> CheckValidations(RegisterModel registerModel)
        {

            var response = await _authenticationServices.CheckValidations(registerModel);
            if (response.Message == ApiResponseEnum.success.ToString())
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [HttpPost("SendOTP")]
        public async Task<ActionResult<ApiResponse>> SendOTP(string username, string email)
        {

            var response = await _authenticationServices.SendOTP(username, email);
            if (response.Message == ApiResponseEnum.success.ToString())
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [HttpPost("ValidateOTP")]
        public async Task<ActionResult<ApiResponse>> ValidateOTP(int inputPin, string username, string email)
        {

            var response = await _authenticationServices.ValidateOTP(inputPin, username, email);
            if (response.Message == ApiResponseEnum.success.ToString())
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

    }
}
