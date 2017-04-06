using System;
using System.Collections.Generic;
using FacebookToDisqusComments.DataServices;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacebookToDisqusComments.Tests.ApiWrappers
{
    [TestClass]
    public class FacebookResponseParserTests
    {
        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void ParseJsonResponse_ShouldThrowArgumentNullException_WhenContentIsInvalid(string content)
        {
            // Arrange
            var parser = new JsonParser();

            // Act
            Action action = () => parser.ParseJsonResponse<FacebookCommentsPage>(content);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void ParseJsonResponse_ShouldReturnNull_WhenContentIsEmptyJsonObject()
        {
            // Arrange
            const string json = "{}";
            var parser = new JsonParser();

            // Act
            var result = parser.ParseJsonResponse<FacebookCommentsPage>(json);

            // Assert
            result.Comments.Should().BeNull();
        }

        [TestMethod]
        public void ParseJsonResponse_ShouldReturnEmptyCommentsList_WhenDataIsEmpty()
        {
            // Arrange
            const string json = "{data:[]}";
            var parser = new JsonParser();

            // Act
            var result = parser.ParseJsonResponse<FacebookCommentsPage>(json);

            // Assert
            result.Comments.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseJsonResponse_ShouldReturnTwoCommentsWith_WhenClientReponseContentContainsThreeComments()
        {
            // Arrange
            var parser = new JsonParser();
            var expectedPage = new FacebookCommentsPage
            {
                Comments = GetFakeCommentsList()
            };

            // Act
            var result = parser.ParseJsonResponse<FacebookCommentsPage>(GetFakeCommentsJson());

            // Assert
            result.ShouldBeEquivalentTo(expectedPage);
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
