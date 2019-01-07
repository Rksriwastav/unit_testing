using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApiForTesting;
using WebApiForTesting.Controllers;
using WebApiForTesting.Models;
namespace WebApiForTesting.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void AddLoginTest()
        {
            // Arrange  
            var controller = new HomeController();
            loginModel loginModeldetail = new loginModel
            {
                UserName = "ritesh.sriwastav@striderinfotech.com",
                UserId = 1,
                Password = "abc1234"
            };

            var response = controller.login(loginModeldetail) as OkResult;
            Assert.IsNotNull(response, "User unable to log in with correct login info");

    }

        [TestMethod]
        public void CreateCredentials()
        {
            // Arrange  
            var controller = new HomeController();
            Login login = new Login
            {
                UserId=11,
                UserName = "riteshkumar50@gmail.com",
                Password="1234",
                UserRoleId=1,
                IsEmailVerified=true,
                ActivationCode=123,
                ResetPasswordCode= "86a6b1ae-571c-4e85-bab0-71f7036cdab8"
            };
            // Act  
            IHttpActionResult actionResult = controller.CreateCredential(login);
            var contentResult = actionResult as NegotiatedContentResult<Login>;
            Assert.IsNotNull(contentResult.Content);
            // Assert  
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }


        [TestMethod]
        public void GetUserByUserId()
        {
            // Set
            var controller = new HomeController();
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            // Act 
            var response = controller.GetUserById(4);
            // Assert 
            Login login;
            Assert.IsTrue(response.TryGetContentValue<Login>(out login));
            Assert.AreEqual(4, login.UserId);
        }
    }
}

