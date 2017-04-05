using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using FluentAssertions;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers;
using FacebookToDisqusComments.DataServices;

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
            result.Should().Be(ReturnCodes.FacebookApiError);
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

        // TODO cover other Run test cases
    }
}
