using System.Collections.Generic;
using System.Xml.Linq;
using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.DataServices
{
    public interface IDisqusCommentsFormatter
    {
        XDocument ConvertCommentsIntoXml(IEnumerable<FacebookComment> comments, string pageTitle, string pageUrl, string pageId);
        XElement CreateComment(FacebookComment comment, string parentId);
        IList<XElement> CreateCommentsList(IEnumerable<FacebookComment> comments);
    }
}