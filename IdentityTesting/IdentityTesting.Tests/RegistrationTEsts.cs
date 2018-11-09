using System;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using IdentityTesting.App.Areas.Identity.Pages.Account;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityTesting.Tests
{
    public class FakeSignInManager : SignInManager<IdentityUser>
    {
        public FakeSignInManager()
            : base(new Mock<FakeUserManager>().Object,
                  new Mock<IHttpContextAccessor>().Object,
                  new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
                  new Mock<IAuthenticationSchemeProvider>().Object)
        {

        }
    }

    public class FakeUserManager : UserManager<IdentityUser>       
    {
        public FakeUserManager()
             : base(new Mock<IUserStore<IdentityUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<IdentityUser>>().Object,
                  new IUserValidator<IdentityUser>[0],
                  new IPasswordValidator<IdentityUser>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<IdentityUser>>>().Object)
        {

        }
    }

    public class RegistrationTests
    {
        [Fact]
        public async Task User_is_registered_with_valid_form()
        {
            //Arrange
            var userManager = new Mock<FakeUserManager>();
            var signInManager = new Mock<FakeSignInManager>();
            var logger = new Mock<ILogger<RegisterModel>>();
            var emailSender = new Mock<IEmailSender>();

            var url = new Mock<IUrlHelper>();

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            userManager
                .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.FromResult("THE_CODE"));

            var registerPage = new RegisterModel(userManager.Object, signInManager.Object, logger.Object, emailSender.Object)
            {
                Input = new RegisterModel.InputModel
                {
                    ConfirmPassword = "sss",
                    Email = "sadasd@ASdqsad.com",
                    Password = "sss"
                },
                Url = url.Object,
                PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext { }
            };

            //Act
            await registerPage.OnPostAsync("theUrl");

            //Assert
            userManager.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));
            userManager.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>()));
            emailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(y => y == "THE_CODE")));
        }
    }
}
