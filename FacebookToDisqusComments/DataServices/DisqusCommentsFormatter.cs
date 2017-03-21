using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.DataServices
{
    public class DisqusCommentsFormatter : IDisqusCommentsFormatter
    {
        private readonly XNamespace _contentNs = "http://purl.org/rss/1.0/modules/content/";
        private readonly XNamespace _dsqNs = "http://www.disqus.com/";
        private readonly XNamespace _dcNs = "http://purl.org/dc/elements/1.1/";
        private readonly XNamespace _wpNs = "http://wordpress.org/export/1.0/";

        public XDocument ConvertCommentsIntoXml(IList<FacebookComment> comments, string pageTitle, Uri pageUrl, string pageId)
        {
            if (comments == null)
            {
                throw new ArgumentNullException(nameof(comments));
            }

            if (pageUrl == null)
            {
                throw new ArgumentNullException(nameof(pageUrl));
            }

            var doc = new XDocument(
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "content", _contentNs),
                    new XAttribute(XNamespace.Xmlns + "dsq", _dsqNs),
                    new XAttribute(XNamespace.Xmlns + "dc", _dcNs),
                    new XAttribute(XNamespace.Xmlns + "wp", _wpNs),
                    new XElement("channel",
                        new XElement("item",
                            new XElement("title", pageTitle),
                            new XElement("link", pageUrl),
                            new XElement(_contentNs + "encoded", string.Empty),
                            new XElement(_dsqNs + "thread_identifier", pageId),
                            new XElement(_wpNs + "post_date_gmt", string.Empty),
                            new XElement(_wpNs + "comment_status", "open"),
                            CreateCommentsList(comments)
                ))));

            return doc;
        }

        public IList<XElement> CreateCommentsList(IList<FacebookComment> comments)
        {
            if (comments == null)
            {
                throw new ArgumentNullException(nameof(comments));
            }

            var list = new List<XElement>();

            foreach (var comment in comments)
            {
                list.Add(CreateComment(comment));
                if (comment.Children == null)
                {
                    comment.Children = new List<FacebookComment>();
                }

                foreach (var commentChild in comment.Children)
                {
                    list.Add(CreateComment(commentChild, comment.Id));
                }
            }

            return list;
        }

        public XElement CreateComment(FacebookComment comment, string parentId = "0")
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            var commentElement = new XElement(_wpNs + "comment",
                new XElement(_wpNs + "comment_id", comment.Id),
                new XElement(_wpNs + "comment_author", comment.From.Name),
                new XElement(_wpNs + "comment_author_email", string.Empty),
                new XElement(_wpNs + "comment_author_url", string.Empty),
                new XElement(_wpNs + "comment_author_IP", string.Empty),
                new XElement(_wpNs + "comment_date_gmt", comment.CreatedTime.ToString("yyyy-MM-dd HH:MM:ss")),
                new XElement(_wpNs + "comment_content", new XCData(comment.Message)),
                new XElement(_wpNs + "comment_approved", "1"),
                new XElement(_wpNs + "comment_parent", parentId)
            );

            return commentElement;
        }
    }
}
