using System;
using FacebookToDisqusComments.DataServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacebookToDisqusComments.Tests.DataServices
{
    [TestClass]
    public class FileUtilsTests
    {
        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void FormatOutputFilePath_ShouldThrowArgumentNullException_WhenFolderPathIsInvalid(string folderPath)
        {
            // Arrange
            var fileUtils = new FileUtils();

            // Act
            Action action = () => fileUtils.FormatOutputFilePath(folderPath, "file");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow(" ", DisplayName = "Whitespace")]
        public void FormatOutputFilePath_ShouldThrowArgumentNullException_WhenFilenameIsInvalid(string filename)
        {
            // Arrange
            var fileUtils = new FileUtils();

            // Act
            Action action = () => fileUtils.FormatOutputFilePath("folder", filename);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void FormatOutputFilePath_ShouldCombinePathAndAddXmlExtension_WhenFolderAndFileNameValid()
        {
            // Arrange
            var fileUtils = new FileUtils();

            // Act
            var result = fileUtils.FormatOutputFilePath("folder", "file");

            // Assert
            result.Should().Be(@"folder\file.xml");
        }
    }
}
