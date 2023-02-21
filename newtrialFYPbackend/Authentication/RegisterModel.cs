using System.ComponentModel.DataAnnotations;

namespace newtrialFYPbackend.Authentication
{
    public class RegisterModel
    {
        //These are the properties to be used to create an account - Username, password, etc
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [Required(ErrorMessage = "USERNAME IS REQUIRED")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "EMAIL IS REQUIRED")]
        public string Email { get; set; }


        [Required(ErrorMessage = "PASSWORD IS REQUIRED")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
