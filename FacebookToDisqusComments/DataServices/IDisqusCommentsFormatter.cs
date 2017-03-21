using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.DataServices
{
    public interface IDisqusCommentsFormatter
    {
        XDocument ConvertCommentsIntoXml(IList<FacebookComment> comments, string pageTitle, Uri pageUrl, string pageId);
        XElement CreateComment(FacebookComment comment, string parentId);
        IList<XElement> CreateCommentsList(IList<FacebookComment> comments);
    }
}