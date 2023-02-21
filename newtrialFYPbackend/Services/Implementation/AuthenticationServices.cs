using newtrialFYPbackend.Authentication;
using newtrialFYPbackend.Responses;
using newtrialFYPbackend.Services.Interface;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Net.Http.Json;
using newtrialFYPbackend.Responses.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using newtrialFYPbackend.Model;
//using newtrialFYPbackend.Migrations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace newtrialFYPbackend.Services.Implementation
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> roleManager;

        
        private ApplicationDbContext _context;
        public AuthenticationServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            this.userManager = userManager;
            _configuration = configuration;
            this.roleManager = roleManager;
            _context = context;
        }

        public async Task<string> CreateRoles()
        {
            //create the "User" Role
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                return "Role successfully created";
            }
            return "Role already exists";
        }



        //REMEMBER TO CHANGE THIS
        public async Task<ApiResponse> RegisterUser(RegisterModel model)
        {
            //create the "user" role
            await CreateRoles();

            var validateEmail = await ValidateEmail(model.Email);
            if (!validateEmail)
            {
                ReturnedResponse returnedResponse = new ReturnedResponse();
                return returnedResponse.ErrorResponse("Email is Invalid",null);
            }

            //ensure no other user has the same username
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                ReturnedResponse returnedResponse = new ReturnedResponse();
                return returnedResponse.ErrorResponse("User with that Username Already Exists", null);
            }

            //ensure no other user has the same email
            var emailExists = await userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                ReturnedResponse returnedResponse = new ReturnedResponse();
                return returnedResponse.ErrorResponse("User with that Email Already Exists", null);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            return new ApiResponse
            {
                data = user,
                error = null,
                Message = ApiResponseEnum.success.ToString(),
                code = "200"

            };
           
            /*
            //create user and add the "user" role.
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ///RETURN A NEW API RESPONSE THAT THE USER WAS NOT CREATED

            }
            await userManager.AddToRoleAsync(user, UserRoles.User);

            */


            //create ACCOUNT instance with additional properties like height,age, gender, etc and save the account to the database.
            /*Owner owner = new Owner
            {
                Username = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateCreated = DateTime.Now
            };

            await _context.Owners.AddAsync(owner);
            */

           /// await _context.SaveChangesAsync();

            ///return new AuthenticationResponse { Status = AuthenticationResponseEnum.Success.GetEnumDescription(), Message = "User Created Succesfully" };


        }


        public async Task<ApiResponse> CheckValidations(RegisterModel model)
        {
            ReturnedResponse returnedResponse = new ReturnedResponse();

            //create the "user" role
            await CreateRoles();

            //ensure that the email is valid using regular expressions.
            var validateEmail = await ValidateEmailRegExp(model.Email);
            if (!validateEmail)
            {
                return returnedResponse.ErrorResponse("Email is Invalid", null);
            }
            

            //ensure no other user has the same username
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return returnedResponse.ErrorResponse("User with that Username Already Exists", null);
            }

            //ensure no other user has the same email
            var emailExists = await userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                return returnedResponse.ErrorResponse("User with that Email Already Exists", null);
            }

            //ensure password meets the validations
            var validatePassword = await ValidatePassword(model.Password);
            if(validatePassword.error != null)
            {
                return returnedResponse.ErrorResponse(validatePassword.error.message, null);
            }

            //ensure password and confirm password are the same
            if(model.Password != model.ConfirmPassword)
            {
                return returnedResponse.ErrorResponse("Password and Confirm Password do not match", null);
            }


            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            return returnedResponse.CorrectResponse(user);

            /*
            //create user and add the "user" role.
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ///RETURN A NEW API RESPONSE THAT THE USER WAS NOT CREATED

            }
            await userManager.AddToRoleAsync(user, UserRoles.User);

            */


            //create ACCOUNT instance with additional properties like height,age, gender, etc and save the account to the database.
            /*Owner owner = new Owner
            {
                Username = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateCreated = DateTime.Now
            };

            await _context.Owners.AddAsync(owner);
            */

            /// await _context.SaveChangesAsync();

            ///return new AuthenticationResponse { Status = AuthenticationResponseEnum.Success.GetEnumDescription(), Message = "User Created Succesfully" };


        }

        //SEEMS THIS MAY BE USELESS
        public async Task<bool>  ValidateEmail(string email)
        {
            bool isEmailValid = false; 

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://zerobounce1.p.rapidapi.com/v2/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add($"X-RapidAPI-Key", $"26c54984bcmsh448ceed8e8e9cecp1e9f68jsn5dc0e458b769");
            client.DefaultRequestHeaders.Add($"X-RapidAPI-Host", $"zerobounce1.p.rapidapi.com");

            string apiKey = "api_key=5b63a372c59744d7a09150493eb744d9";
            string path = $"valdate?{apiKey}&email={email}";

            try
            {
                HttpResponseMessage Res = await client.GetAsync(path);

                if (Res.IsSuccessStatusCode)
                {
                    var emailValidationResponse = await Res.Content.ReadFromJsonAsync<EmailValidationResponse>();
                    if(emailValidationResponse.status == EmailValidationEnum.valid.ToString())
                    {
                        isEmailValid = true;
                    }
                }

                return isEmailValid;
            }
            
            catch(Exception my_ex) 
            {
                ///refine later
                throw new BadHttpRequestException(my_ex.Message);
            }


        }

        public async Task<ApiResponse> ValidatePassword(string password)
        {

            string errorMessage = "";

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniChars = new Regex(@".{8,}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            
            ReturnedResponse returnedResponse = new ReturnedResponse();

            if (!hasLowerChar.IsMatch(password))
            {
                errorMessage = "Password should contain At least one lower case letter";
                return returnedResponse.ErrorResponse(errorMessage, null);
            }
            else if (!hasUpperChar.IsMatch(password))
            {
                errorMessage = "Password should contain At least one upper case letter";
                return returnedResponse.ErrorResponse(errorMessage, null);
            }
            else if (!hasMiniChars.IsMatch(password))
            {
                errorMessage = "Password should not be less than 8 characters";
                return returnedResponse.ErrorResponse(errorMessage, null);
            }
            else if (!hasNumber.IsMatch(password))
            {
                errorMessage = "Password should contain At least one numeric value";
                return returnedResponse.ErrorResponse(errorMessage, null);
            }

            else if (!hasSymbols.IsMatch(password))
            {
                errorMessage = "Password should contain At least one special case characters";
                return returnedResponse.ErrorResponse(errorMessage, null);
            }
            else
            {
                return returnedResponse.CorrectResponse(true);
            }
        }

        public async Task<bool> ValidateEmailRegExp(string email)
        {
            var validEmail = new Regex("^\\S+@\\S+\\.\\S+$");
            if (validEmail.IsMatch(email))
            {
                return true;
            }
            return false;

        }

        public async Task<ApiResponse> CreateOTP(string username, string email)
        {
            ReturnedResponse returnedResponse = new ReturnedResponse();
            try
            {
                Random random = new Random();
                int pin = random.Next(100000, 1000000);

                var newOTP = new OTP
                {
                    pin = pin,
                    username = username,
                    email = email,

                };
                await _context.OTPs.AddAsync(newOTP);
                await _context.SaveChangesAsync();

                return returnedResponse.CorrectResponse(newOTP.pin);
            }
            
            catch (Exception myEx)
            {
                return returnedResponse.ErrorResponse(myEx.ToString(), null);
            }

            
            

        }

        public async Task<ApiResponse> SendOTP(string username, string email)
        {
            ReturnedResponse returnedResponse = new ReturnedResponse();
            var newOTP = await CreateOTP(username, email);

            if(newOTP.error != null)
            {
                return returnedResponse.ErrorResponse(newOTP.error.message, null);
            }

            HttpClient client = new HttpClient();
            string baseUrl = "https://rapidprod-sendgrid-v1.p.rapidapi.com/";

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "26c54984bcmsh448ceed8e8e9cecp1e9f68jsn5dc0e458b769");
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "rapidprod-sendgrid-v1.p.rapidapi.com");

            SendEmailRequest request = new SendEmailRequest
            {
                personalizations = new List<Personalization>()
                {
                    new Personalization
                    {
                        to = new List<To>()
                        {
                            new To
                            {
                                email = email,
                            }
                        },
                        subject = "YOUR ONE TIME PASSOWRD (OTP)",
                    }
                },

                from = new From
                {
                    email = "gniweriebor@gmail.com",
                },

                content = new List<Content>()
                {
                    new Content
                    {
                        type = "text/plain",
                         value = $"YOUR ONE TIME PASSWORD TO LOG IN TO FOODIFIED HAS BEEN GENERATED. DO NOT GIVE ANYONE, IT IS {newOTP.data}",
                    }
                }

            };

            try
            {
                string path = "mail/send";
                HttpResponseMessage Res = await client.PostAsJsonAsync(path, request);

                if (Res.IsSuccessStatusCode)
                {
                    var response = await Res.Content.ReadAsStringAsync();
                    return returnedResponse.CorrectResponse("Email Successfully Sent");

                }

                return returnedResponse.ErrorResponse(Res.Content.ToString(), null);

            }

            catch (Exception my_ex)
            {
                return returnedResponse.ErrorResponse(my_ex.Message.ToString(), null);

            }


        }

        public async Task<ApiResponse> ValidateOTP(int inputPin, string username, string email)
        {
            ReturnedResponse returnedResponse = new ReturnedResponse();
            var userOTP =  await _context.OTPs.Where(o => o.email == email).OrderBy(o=>o.Id).LastAsync();
            if (userOTP.pin == inputPin)
            {
                return returnedResponse.CorrectResponse("OTP successfully validated");
            }
            return returnedResponse.ErrorResponse("Invalid OTP", null);
        }
    }
}
