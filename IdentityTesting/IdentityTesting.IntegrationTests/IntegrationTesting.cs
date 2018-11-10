using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net.Http;
using Newtonsoft.Json;
using IdentityTesting.App.Controllers;
using System.Text.RegularExpressions;

namespace IdentityTesting.IntegrationTests
{
    public partial class IntegrationTesting : IClassFixture<WebApplicationFactory<IdentityTesting.App.Startup>>
    {
        private readonly WebApplicationFactory<App.Startup> _appFactory;

        public IntegrationTesting(WebApplicationFactory<App.Startup> appFactory)
        {
            _appFactory = appFactory;
        }

        [Fact]        
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _appFactory.CreateClient();

            var r1 = await client.GetAsync("/Identity/Account/Register");
            r1.EnsureSuccessStatusCode();

            string antiForgeryToken = await AntiForgeryHelper.ExtractAntiForgeryToken(r1);

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                {"__RequestVerificationToken", antiForgeryToken},
                {"Input.Email", "sat@sat.com" },
                {"Input.Password", "HelloSatnam123!"},
                {"Input.ConfirmPassword", "HelloSatnam123!"} }
            );
            
            // Act
            var response = await client.PostAsync("/Identity/Account/Register", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        public class AntiForgeryHelper
        {
            public static string ExtractAntiForgeryToken(string htmlResponseText)
            {
                if (htmlResponseText == null) throw new ArgumentNullException("htmlResponseText");

                System.Text.RegularExpressions.Match match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
                return match.Success ? match.Groups[1].Captures[0].Value : null;
            }

            public static async Task<string> ExtractAntiForgeryToken(HttpResponseMessage response)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                return await Task.FromResult(ExtractAntiForgeryToken(responseAsString));
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Xunit;
//using System.Net.Http;
//using Newtonsoft.Json;
//using IdentityTesting.App.Controllers;
//using System.Text.RegularExpressions;

//namespace IdentityTesting.IntegrationTests
//{
//    public partial class IntegrationTesting : IClassFixture<WebApplicationFactory<IdentityTesting.App.Startup>>
//    {
//        private readonly WebApplicationFactory<App.Startup> _appFactory;

//        public IntegrationTesting(WebApplicationFactory<App.Startup> appFactory)
//        {
//            _appFactory = appFactory;
//        }

//        [Fact]        
//        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
//        {
//            // Arrange
//            var client = _appFactory.CreateClient();

//            var r1 = await client.GetAsync("/account/register");
//            r1.EnsureSuccessStatusCode();

//            string antiForgeryToken = await AntiForgeryHelper.ExtractAntiForgeryToken(r1);

//            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
//                //{"__RequestVerificationToken", antiForgeryToken},
//                {"Input.Email", "sat@sat.com" },
//                {"Input.Password", "HelloSatnam123!"} }
//            );

//            var requestMessage = PostRequestHelper.CreateWithCookiesFromResponse("/account/register", new Dictionary<string, string> {
//                //{"__RequestVerificationToken", antiForgeryToken},
//                {"Input.Email", "sat@sat.com" },
//                {"Input.Password", "HelloSatnam123!"} }, r1);

//            // Act
//            var response = await client.PostAsync("/account/register", content);

//            // Assert
//            response.EnsureSuccessStatusCode(); // Status Code 200-299
//            Assert.Equal("text/html; charset=utf-8",
//                response.Content.Headers.ContentType.ToString());
//        }

//        public class AntiForgeryHelper
//        {
//            public static string ExtractAntiForgeryToken(string htmlResponseText)
//            {
//                if (htmlResponseText == null) throw new ArgumentNullException("htmlResponseText");

//                System.Text.RegularExpressions.Match match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
//                return match.Success ? match.Groups[1].Captures[0].Value : null;
//            }

//            public static async Task<string> ExtractAntiForgeryToken(HttpResponseMessage response)
//            {
//                string responseAsString = await response.Content.ReadAsStringAsync();
//                return await Task.FromResult(ExtractAntiForgeryToken(responseAsString));
//            }
//        }
//    }
//}
