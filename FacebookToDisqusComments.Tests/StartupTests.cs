using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using FluentAssertions;
using System.Threading.Tasks;
using System.Xml.Linq;
using FacebookToDisqusComments.ApiWrappers;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using FacebookToDisqusComments.DataServices;
using FacebookToDisqusComments.DataServices.Dtos;

namespace FacebookToDisqusComments.Tests
{
    [TestClass]
    public class StartupTests
    {
        private IOptions<AppSettings> _settings;
        private IFacebookCommentsApiWrapper _facebookApi;
        private IDisqusCommentsFormatter _disqusFormatter;
        private IFileUtils _fileUtils;

        [TestInitialize]
        public void Init()
        {
            _settings = Substitute.For<IOptions<AppSettings>>();
            _settings.Value.Returns(new AppSettings());
            _facebookApi = Substitute.For<IFacebookCommentsApiWrapper>();
            _disqusFormatter = Substitute.For<IDisqusCommentsFormatter>();
            _fileUtils = Substitute.For<IFileUtils>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenSettingsIsNull()
        {
            // Arrange & Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Startup(null, _facebookApi, _disqusFormatter, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenSettingsValueIsNull()
        {
            // Arrange
            _settings.Value.Returns(null as AppSettings);

            // Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenFacebookApiWrapperIsNull()
        {
            // Arrange & Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Startup(_settings, null, _disqusFormatter, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenDisqusCommentsFormatterIsNull()
        {
            // Arrange & Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Startup(_settings, _facebookApi, null, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenFileUtilsIsNull()
        {
            // Arrange & Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Startup(_settings, _facebookApi, _disqusFormatter, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnUnexpectedErrorCode_WhenExceptionThrown()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Throws(new Exception());
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
           var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.UnexpectedError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnFacebookApiErrorCode_WhenExceptionThrown()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Throws(new FacebookApiException());
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
           var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.FacebookGetTokenError);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public async Task RunAsync_ShouldReturnAccessTokenErrorCode_WhenAccessTokenIsInvalid(string accessToken)
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(accessToken);
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.AccessTokenError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnNoCommentsInfoErrorCode_WhenNullReturned()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns("token");
            _fileUtils.LoadCommentsPageInfo(Arg.Any<string>())
                .Returns(null as List<CommentsPageInfo>);
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.NoCommentsInfoError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnNoCommentsInfoErrorCode_WhenNoCommentsLoadedFromFile()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns("token");
            _fileUtils.LoadCommentsPageInfo(Arg.Any<string>())
                .Returns(new List<CommentsPageInfo>());
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.NoCommentsInfoError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnFacebookApiErrorCode_WhenNullFacebookCommentsRetrieved()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns("token");
            _fileUtils.LoadCommentsPageInfo(Arg.Any<string>())
                .Returns(new List<CommentsPageInfo>{new CommentsPageInfo()});
            _facebookApi.GetPageCommentsAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult<IList<FacebookComment>>(null));
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.FacebookGetCommentsError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnFacebookApiErrorCode_WhenEmptyListFacebookCommentsRetrieved()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns("token");
            _fileUtils.LoadCommentsPageInfo(Arg.Any<string>())
                .Returns(new List<CommentsPageInfo>{new CommentsPageInfo()});
            _facebookApi.GetPageCommentsAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult<IList<FacebookComment>>(new List<FacebookComment>()));
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.FacebookGetCommentsError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnDisqusConvertionErrorCode_WhenNullFacebookCommentsRetrieved()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns("token");
            _fileUtils.LoadCommentsPageInfo(Arg.Any<string>())
                .Returns(new List<CommentsPageInfo> { new CommentsPageInfo() });
            _facebookApi.GetPageCommentsAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult<IList<FacebookComment>>(new List<FacebookComment>{new FacebookComment()}));
            _disqusFormatter.ConvertCommentsIntoXml(Arg.Any<List<FacebookComment>>(), Arg.Any<string>(), Arg.Any<Uri>(), Arg.Any<string>())
                .Returns(null as XDocument);
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.DisqusConvertionError);
        }

        [TestMethod]
        public async Task RunAsync_ShouldReturnDisqusConvertionErrorCode_WhenRootNullFacebookCommentsRetrieved()
        {
            // Arrange
            _facebookApi.GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns("token");
            _fileUtils.LoadCommentsPageInfo(Arg.Any<string>())
                .Returns(new List<CommentsPageInfo> { new CommentsPageInfo() });
            _facebookApi.GetPageCommentsAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult<IList<FacebookComment>>(new List<FacebookComment>{new FacebookComment()}));
            _disqusFormatter.ConvertCommentsIntoXml(Arg.Any<List<FacebookComment>>(), Arg.Any<string>(), Arg.Any<Uri>(), Arg.Any<string>())
                .Returns(new XDocument());
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.RunAsync();

            // Assert
            result.Should().Be(ReturnCodes.DisqusConvertionError);
        }
    }
}
