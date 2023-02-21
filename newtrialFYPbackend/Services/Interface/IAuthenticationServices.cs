using newtrialFYPbackend.Authentication;
using newtrialFYPbackend.Responses;
using System.Threading.Tasks;

namespace newtrialFYPbackend.Services.Interface
{
    public interface IAuthenticationServices
    {
        public  Task<ApiResponse> RegisterUser(RegisterModel model);
        public  Task<ApiResponse> CheckValidations(RegisterModel model);
        public  Task<ApiResponse> ValidatePassword(string password);

        public  Task<bool> ValidateEmail(string email);
        public  Task<bool> ValidateEmailRegExp(string email);

        public Task<ApiResponse> CreateOTP(string username, string email);
        public Task<ApiResponse> SendOTP(string username, string email);
        public Task<ApiResponse> ValidateOTP(int inputPin, string username, string email);


    }
}
