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

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void GetPageComments_ShouldThrowArgumentNullException_WhenAccessTokenIsInvalid(string accessToken)
        {
            // Arrange
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

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
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

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
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.OK, "application/json", string.Empty);
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            Func<Task> action = async () => await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            action.ShouldThrow<FacebookApiException>()
                .And.Message.Should().Be("Http client response is empty.");
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnEmptyList_WhenClientReponseContentIsEmptyJsonObject()
        {
            // Arrange
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.OK, "application/json", "{}");
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnEmptyList_WhenClientReponseContentIsJsonObjectWithEmptyCommentsList()
        {
            // Arrange
            _httpMockHandler.When("*")
                .Respond(HttpStatusCode.OK, "application/json", "{data:[]}");
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetPageComments_ShouldReturnTwoCommentsWith_WhenClientReponseContentContainsThreeComments()
        {
            // Arrange
            var expectedList = GetFakeCommentsList();

            _httpMockHandler.When("https://graph.facebook.com/comments*")
                .Respond(HttpStatusCode.OK, "application/json", GetFakeCommentsJson());
            var wrapper = new FacebookCommentsApiWrapper(_httpClientFactory);

            // Act
            var result = await wrapper.GetPageComments("accessToken", "pageId");

            // Assert
            result.ShouldBeEquivalentTo(expectedList);
        }

        private static string GetFakeCommentsJson()
        {
            return @"
                {
                data:
                [
                {
	                id:'id', 
	                message:'message', 
	                created_time:'2017-03-20 12:13:14', 
	                from:{id:'userId', name:'userName'}
                },
	            {
		            id:'id2', 
		            message:'message2', 
		            created_time:'2017-03-22 14:15:16', 
		            from:{id:'userId2', name:'userName2'}
	            }
                ]
                }
            ";
        }

        private static IList<FacebookComment> GetFakeCommentsList()
        {
            var comment = new FacebookComment
            {
                Id = "id",
                Message = "message",
                CreatedTime = DateTime.Parse("2017-03-20 12:13:14"),
                From = new FacebookCommentUser
                {
                    Id = "userId",
                    Name = "userName"
                }
            };

            var comment2 = new FacebookComment
            {
                Id = "id2",
                Message = "message2",
                CreatedTime = DateTime.Parse("2017-03-22 14:15:16"),
                From = new FacebookCommentUser
                {
                    Id = "userId2",
                    Name = "userName2"
                }
            };

            return new List<FacebookComment>
            {
                comment,
                comment2
            };
        }
    }
}
