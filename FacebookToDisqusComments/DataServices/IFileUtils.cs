using System.Collections.Generic;
using System.Xml.Linq;
using FacebookToDisqusComments.DataServices.Dtos;

namespace FacebookToDisqusComments.DataServices
{
    public interface IFileUtils
    {
        IEnumerable<CommentsPageInfo> LoadCommentsPageInfo(string inputFilePath);
        void SaveAsXml(XDocument disqusCommentsXml, string fileName);
    }
}