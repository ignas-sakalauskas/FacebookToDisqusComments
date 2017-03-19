using FacebookToDisqusComments.ApiWrappers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FacebookToDisqusComments.Tests.ApiWrappers
{
    [TestClass]
    public class FacebookCommentsApiWrapperTests
    {
        private Func<HttpClient> _httpClientFactory;
        private MockHttpMessageHandler _httpMockHandler;

        [TestInitialize]
        public void Init()
        {
            _httpMockHandler = new MockHttpMessageHandler();
            _httpClientFactory = () =>
            {
                return _httpMockHandler.ToHttpClient();
            };
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void GetAccessToken_ShouldThrowArgumentNullException_WhenAppIdIsInvalid(string appId)
        {
            // Arrange
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            Func<Task> action = async () => await wrapper.GetAccessToken(appId, "secret");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void GetAccessToken_ShouldThrowArgumentNullException_WhenAppSecretIsInvalid(string appSecret)
        {
            // Arrange
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            Func<Task> action = async () => await wrapper.GetAccessToken("appId", appSecret);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void GetAccessToken_ShouldThrowFacebookApiException_WhenClientReponseIsNotSuccess()
        {
            // Arrange
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.InternalServerError, "application/json", string.Empty);
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            Func<Task> action = async () => await wrapper.GetAccessToken("appId", "appSecret");

            // Assert
            action.ShouldThrow<FacebookApiException>()
                .And.Message.Should().Be("Http client response was not successful.");
        }

        [TestMethod]
        public void GetAccessToken_ShouldThrowFacebookApiException_WhenClientReponseContentIsEmpty()
        {
            // Arrange
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.OK, "application/json", string.Empty);
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            Func<Task> action = async () => await wrapper.GetAccessToken("appId", "appSecret");

            // Assert
            action.ShouldThrow<FacebookApiException>()
                .And.Message.Should().Be("Http client response doesn't contain access token.");
        }

        [TestMethod]
        public void GetAccessToken_ShouldThrowFacebookApiException_WhenClientReponseContentDoesNotContainKey()
        {
            // Arrange
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.OK, "application/json", "nokeycontent");
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            Func<Task> action = async () => await wrapper.GetAccessToken("appId", "appSecret");

            // Assert
            action.ShouldThrow<FacebookApiException>()
                .And.Message.Should().Be("Http client response doesn't contain access token.");
        }

        [TestMethod]
        public async Task GetAccessToken_ShouldReturnAccessToken_WhenClientReponseContentContainsAccessToken()
        {
            // Arrange
            _httpMockHandler.When("https://graph.facebook.com/oauth/access_token*")
                .Respond(HttpStatusCode.OK, "application/json", "access_token=123");
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            var result = await wrapper.GetAccessToken("appId", "appSecret");

            // Assert
            result.Should().Be("123");
        }
    }
}
