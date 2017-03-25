using FacebookToDisqusComments.ApiWrappers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using NSubstitute;

namespace FacebookToDisqusComments.Tests.ApiWrappers
{
    [TestClass]
    public class FacebookCommentsApiWrapperTests
    {
        private Func<HttpClient> _httpClientFactory;
        private MockHttpMessageHandler _httpMockHandler;
        private IFacebookResponseParser _responseParser;

        private const string FacebookOkCommentsUrlPattern = "https://graph.facebook.com/comments*";

        [TestInitialize]
        public void Init()
        {
            _httpMockHandler = new MockHttpMessageHandler();
            _httpClientFactory = () => _httpMockHandler.ToHttpClient();
            _responseParser = Substitute.For<IFacebookResponseParser>();
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenHttpFactoryIsNull()
        {
            // Arrange
            // Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FacebookCommentsApiWrapper(null, _responseParser);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenParserIsNull()
        {
            // Arrange
            // Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FacebookCommentsApiWrapper(_httpClientFactory, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void GetAccessToken_ShouldThrowArgumentNullException_WhenAppIdIsInvalid(string appId)
        {
            // Arrange
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            var result = await wrapper.GetAccessToken("appId", "appSecret");

            // Assert
            result.Should().Be("123");
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void GetPageComments_ShouldThrowArgumentNullException_WhenAccessTokenIsInvalid(string accessToken)
        {
            // Arrange
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            Func<Task> action = async () => await wrapper.GetPageComments(accessToken, "pageId");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void GetPageComments_ShouldThrowArgumentNullException_WhenPageIdIsInvalid(string pageId)
        {
            // Arrange
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            Func<Task> action = async () => await wrapper.GetPageComments("accessToken", pageId);

            // Assert
            action.ShouldThrow<ArgumentNullException>();

        }

        [TestMethod]
        public void GetPageComments_ShouldThrowFacebookApiException_WhenClientReponseIsNotSuccess()
        {
            // Arrange
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.InternalServerError, "application/json", string.Empty);
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            Func<Task> action = async () => await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            action.ShouldThrow<FacebookApiException>()
                .And.Message.Should().Be("Http client response was not successful.");
        }

        [TestMethod]
        public void GetPageComments_ShouldThrowFacebookApiException_WhenClientReponseContentIsEmpty()
        {
            // Arrange
            _httpMockHandler.When(FacebookOkCommentsUrlPattern)
                .Respond(HttpStatusCode.OK, "application/json", string.Empty);
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            Func<Task> action = async () => await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            action.ShouldThrow<FacebookApiException>()
                .And.Message.Should().Be("Http client response is empty.");
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnEmptyList_WhenPageIsNull()
        {
            // Arrange
            _httpMockHandler.When(FacebookOkCommentsUrlPattern)
                .Respond(HttpStatusCode.OK, "application/json", "{}");
            _responseParser.ParseFacebookCommentsPage(Arg.Any<string>()).Returns(null as FacebookCommentsPage);
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnEmptyList_WhenCommentsListIsNull()
        {
            // Arrange
            _httpMockHandler.When(FacebookOkCommentsUrlPattern)
                .Respond(HttpStatusCode.OK, "application/json", "{}");
            _responseParser.ParseFacebookCommentsPage(Arg.Any<string>())
                .Returns(new FacebookCommentsPage { Comments = null });
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnEmptyList_WhenCommentsListIsEmpty()
        {
            // Arrange
            _httpMockHandler.When(FacebookOkCommentsUrlPattern)
                .Respond(HttpStatusCode.OK, "application/json", "{}");
            _responseParser.ParseFacebookCommentsPage(Arg.Any<string>())
                .Returns(new FacebookCommentsPage { Comments = new List<FacebookComment>() });
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnOneComment_WhenCommentsListHasOneComment()
        {
            // Arrange
            _httpMockHandler.When(FacebookOkCommentsUrlPattern)
                .Respond(HttpStatusCode.OK, "application/json", "{}");
            _responseParser.ParseFacebookCommentsPage(Arg.Any<string>())
                .Returns(new FacebookCommentsPage { Comments = new List<FacebookComment> { new FacebookComment() } });
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory, _responseParser);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.Should().HaveCount(1);
        }
    }
}
