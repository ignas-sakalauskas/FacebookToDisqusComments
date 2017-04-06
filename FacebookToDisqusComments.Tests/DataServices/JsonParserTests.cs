using System;
using FacebookToDisqusComments.DataServices;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacebookToDisqusComments.Tests.DataServices
{
    [TestClass]
    public class JsonParserTests
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
    }
}
