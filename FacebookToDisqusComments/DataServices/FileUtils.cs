using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FacebookToDisqusComments.DataServices.Dtos;

namespace FacebookToDisqusComments.DataServices
{
    public class FileUtils : IFileUtils
    {
        /// <summary>
        /// Reads TAB seperated file with input data.
        /// Format: Facebook comments page ID, Target Page Title, Target Page URL, Target Page ID
        /// </summary>
        /// <param name="inputFilePath">Path to the file with input data</param>
        /// <returns>List of CommentsPageInfo objects.</returns>
        public IList<CommentsPageInfo> LoadCommentsPageInfo(string inputFilePath)
        {
            var result = new List<CommentsPageInfo>();
            if (!File.Exists(inputFilePath))
            {
                return result;
            }

            var fileLines = File.ReadAllLines(inputFilePath);
            foreach (var line in fileLines)
            {
                var items = line.Split('\t');
                var pageInfo = new CommentsPageInfo
                {
                    FacebookPageId = items[0],
                    TargetPageTitle = items[1],
                    TargetPageUrl = new Uri(items[2]),
                    TargetPageId = items[3]
                };

                result.Add(pageInfo);
            }

            return result;
        }

        public void SaveAsXml(XDocument disqusCommentsXml, string outputFilePath)
        {
            using (var fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate))
            {
                disqusCommentsXml.Save(fileStream);
            }
        }
    }
}
