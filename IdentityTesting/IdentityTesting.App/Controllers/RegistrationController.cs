using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityTesting.App.Controllers
{
    public class RegisterModel2
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }
    }

    [Route("Account")]
    public class RegistrationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IEmailSender _emailSender;

        public RegistrationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<RegistrationController> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [Route("Register")]
        [HttpGet]
        public IActionResult Register([FromQuery] string returnUrl = "")
        {
            return View(new RegisterModel2 { ReturnUrl = returnUrl ?? Url.Content("~/") });
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterModel2 registerModel, string returnUrl = "")
        {
            _logger.LogInformation("User registration: Started");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = registerModel.Input.Email, Email = registerModel.Input.Email };
                var result = await _userManager.CreateAsync(user, registerModel.Input.Password);
                if (result.Succeeded)
                {

                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Link("ConfirmEmailRoute",
                    //    values: new { userId = user.Id, code = code });

                    //await _emailSender.SendEmailAsync(registerModel.Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //_logger.LogInformation("User registration: Success");

                    //_logger.LogInformation("User registration: Signing-in new user");
                    //await _signInManager.SignInAsync(user, isPersistent: false);

                    //_logger.LogInformation("User registration: Redirect to return url");

                    ViewData["ReturnUrl"] = returnUrl;
                    return View("ConfirmRegistration", registerModel);
                }
                else
                {
                    _logger.LogWarning("User registration: Error with user input (from user manager)");

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                _logger.LogWarning("User registration: Error with user input (model validation)");
            }

            registerModel.ReturnUrl = returnUrl;
            return View(registerModel);
        }

        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return View("ConfirmEmail");
        }
    }
}
