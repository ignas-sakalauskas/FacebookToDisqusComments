using System.Collections.Generic;
using System.Xml.Linq;
using FacebookToDisqusComments.DataServices.Dtos;

namespace FacebookToDisqusComments
{
    public interface IFileUtils
    {
        IList<CommentsPageInfo> LoadCommentsPageInfo(string inputFilePath);
        void SaveAsXml(XDocument disqusCommentsXml, string fileName);
    }
}