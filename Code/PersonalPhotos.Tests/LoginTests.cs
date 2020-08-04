using PersonalPhotos.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using PersonalPhotos.Models;
using System.Threading.Tasks;
using Core.Models;

namespace PersonalPhotos.Tests
{
    public class LoginTests
    {
        private readonly LoginsController _controller;
        private readonly Mock<ILogins> _logins;
        private readonly Mock<IHttpContextAccessor> _acessor;

        public LoginTests()
        {
            _logins = new Mock<ILogins>();           

            var session = Mock.Of<ISession>();
            var httpContext = Mock.Of<HttpContext>(x => x.Session == session);
            _acessor = new Mock<IHttpContextAccessor>();
            _acessor.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new LoginsController(_logins.Object, _acessor.Object);
        }

        [Fact]
        public void Index_GivenNorReturnUrl_ReturnLoginView()
        {
            var result = _controller.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Login", result.ViewName, ignoreCase: true);
        }

        [Fact]
        public async Task Login_GivenModelStateInvalid_ReturnLoginValue()
        {
            _controller.ModelState.AddModelError("Test", "teste");
            
            var result = await _controller.Login(Mock.Of<LoginViewModel>()) as ViewResult;
            Assert.Equal("Login", result.ViewName, ignoreCase: true);
        }

        [Fact]
        public async Task Login_GivenCorrectPassword_RedirectToDisplayAction()
        {
            const string passw = "123";
            var modelView = Mock.Of<LoginViewModel>(x => x.Email == "a@b.com" && x.Password == passw);
            var user = Mock.Of<User>(x => x.Password == passw);

            //Como o email não é importante pro teste foi passado um parametro It.IsAny para passar qualquer valor
            //_logins.Setup(x => x.GetUser(modelView.Email)).ReturnsAsync(user);
            _logins.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _controller.Login(modelView);
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
