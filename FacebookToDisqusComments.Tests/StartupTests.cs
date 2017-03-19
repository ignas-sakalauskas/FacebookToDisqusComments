using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using FluentAssertions;
using System.Threading.Tasks;

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
            Action action = () => new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenFacebookApiWrapperIsNull()
        {
            // Arrange & Act
            Action action = () => new Startup(_settings, null, _disqusFormatter, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenDisqusCommentsFormatterIsNull()
        {
            // Arrange & Act
            Action action = () => new Startup(_settings, _facebookApi, null, _fileUtils);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenFileUtilsIsNull()
        {
            // Arrange & Act
            Action action = () => new Startup(_settings, _facebookApi, _disqusFormatter, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public async Task Run_ShouldReturnUnexpectedExceptionCode_WhenExceptionThrown()
        {
            // Arrange
            _facebookApi.GetAccessToken(Arg.Any<string>(), Arg.Any<string>())
                .Throws(new Exception());
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
           var result = await app.Run();

            // Assert
            result.Should().Be(ReturnCodes.UnexpectedError);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public async Task Run_ShouldReturnAccessTokenErrorCode_WhenAccessTokenIsInvalid(string accessToken)
        {
            // Arrange
            _facebookApi.GetAccessToken(Arg.Any<string>(), Arg.Any<string>())
                .Returns(accessToken);
            var app = new Startup(_settings, _facebookApi, _disqusFormatter, _fileUtils);

            // Act
            var result = await app.Run();

            // Assert
            result.Should().Be(ReturnCodes.AccessTokenError);
        }

        // TODO cover other Run test cases
    }
}
