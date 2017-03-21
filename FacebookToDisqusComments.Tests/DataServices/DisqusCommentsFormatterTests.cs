using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using FacebookToDisqusComments.DataServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacebookToDisqusComments.Tests.DataServices
{
    [TestClass]
    public class DisqusCommentsFormatterTests
    {
        private readonly XNamespace _wp = "http://wordpress.org/export/1.0/";

        [TestMethod]
        public void CreateCommentsList_ShouldReturnEmptyList_WhenEmptyListProvided()
        {
            // Arrange
            var formatter = new DisqusCommentsFormatter();

            // Act
            var result = formatter.CreateCommentsList(new List<FacebookComment>());

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void CreateCommentsList_ShouldThrowArgumentNullException_WhenNullArgumentProvided()
        {
            // Arrange
            var formatter = new DisqusCommentsFormatter();

            // Act
            Action action = () => formatter.CreateCommentsList(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateCommentsList_ShouldReturnOneComment_WhenOneCommentProvided()
        {
            // Arrange
            var list = new List<FacebookComment>
            {
                new FacebookComment
                {
                    Message = string.Empty,
                    Children = null,
                    From = new FacebookCommentUser()
                }
            };

            var formatter = new DisqusCommentsFormatter();

            // Act
            var result = formatter.CreateCommentsList(list);

            // Assert
            result.Should().HaveCount(1);
            result.FirstOrDefault().Should().NotBeNull();
        }

        [TestMethod]
        public void CreateComment_ShouldThrowArgumentNullException_WhenCommentIsNull()
        {
            // Arrange
            var formatter = new DisqusCommentsFormatter();

            // Act
            Action action = () => formatter.CreateComment(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateComment_ShouldAddZeroParentId_WhenParentIdNotProvided()
        {
            // Arrange
            var comment = new FacebookComment
            {
                Message = string.Empty,
                From = new FacebookCommentUser()
            };

            var formatter = new DisqusCommentsFormatter();

            // Act
            var result = formatter.CreateComment(comment);

            // Assert
            result.Descendants(_wp + "comment_parent").FirstOrDefault().Value.Should().Be("0");
        }

        [TestMethod]
        public void CreateComment_ShouldAddParentId_WhenParentIdProvided()
        {
            // Arrange
            const string parentId = "123";
            var comment = new FacebookComment
            {
                Message = string.Empty,
                From = new FacebookCommentUser()
            };

            var formatter = new DisqusCommentsFormatter();

            // Act
            var result = formatter.CreateComment(comment, parentId);

            // Assert
            result.Descendants(_wp + "comment_parent").FirstOrDefault().Value.Should().Be(parentId);
        }

        [TestMethod]
        public void CreateComment_ShouldReturnXml_WhenCommentProvided()
        {
            // Arrange
            var comment = new FacebookComment
            {
                Id = "id",
                CreatedTime = DateTime.Parse("2017-03-21 00:03:00"),
                Message = "message",
                From = new FacebookCommentUser
                {
                    Id = "userid",
                    Name = "username"
                }
            };

            var formatter = new DisqusCommentsFormatter();

            // Act
            var result = formatter.CreateComment(comment);

            // Assert
            result.Descendants(_wp + "comment_id").FirstOrDefault().Value.Should().Be("id");
            result.Descendants(_wp + "comment_author").FirstOrDefault().Value.Should().Be("username");
            result.Descendants(_wp + "comment_date_gmt").FirstOrDefault().Value.Should().Be("2017-03-21 00:03:00");
            result.Descendants(_wp + "comment_approved").FirstOrDefault().Value.Should().Be("1");

        }

        [TestMethod]
        public void ConvertCommentsIntoXml_ShouldThrowArgumentNullException_WhenCommentsIsNull()
        {
            // Arrange
            var formatter = new DisqusCommentsFormatter();

            // Act
            Action action = () => formatter.ConvertCommentsIntoXml(null, "title", new Uri("https://ignas.me"), "pageId");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void ConvertCommentsIntoXml_ShouldThrowArgumentNullException_WhenPageUrlIsNull()
        {
            // Arrange
            var formatter = new DisqusCommentsFormatter();

            // Act
            Action action = () => formatter.ConvertCommentsIntoXml(new List<FacebookComment>(), "title", null, "pageId");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void ConvertCommentsIntoXml_ShouldReturnXml_WhenCommentProvided()
        {
            // Arrange
            XNamespace dsq = "http://www.disqus.com/";
            XNamespace content = "http://purl.org/rss/1.0/modules/content/";
            var formatter = new DisqusCommentsFormatter();

            // Act
            var result = formatter.ConvertCommentsIntoXml(new List<FacebookComment>(), "title", new Uri("https://ignas.me/"), "id");

            // Assert
            result.Descendants("title").FirstOrDefault().Value.Should().Be("title");
            result.Descendants("link").FirstOrDefault().Value.Should().Be("https://ignas.me/");
            result.Descendants(_wp + "post_date_gmt").FirstOrDefault().Value.Should().Be(string.Empty);
            result.Descendants(_wp + "comment_status").FirstOrDefault().Value.Should().Be("open");
            result.Descendants(dsq + "thread_identifier").FirstOrDefault().Value.Should().Be("id");
            result.Descendants(content + "encoded").FirstOrDefault().Value.Should().Be(string.Empty);
        }
    }
}
