using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace FacebookToDisqusComments.DisqusComments
{
    public class DisqusCommentsFormatter : IDisqusCommentsFormatter
    {
        public XDocument ConvertCommentsIntoXml(IEnumerable<FacebookComment> comments, string pageTitle, string pageUrl, string pageId)
        {
            XNamespace content = "http://purl.org/rss/1.0/modules/content/";
            XNamespace dsq = "http://www.disqus.com/";
            XNamespace dc = "http://purl.org/dc/elements/1.1/";
            XNamespace wp = "http://wordpress.org/export/1.0/";

            var doc = new XDocument(
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "content", content),
                    new XAttribute(XNamespace.Xmlns + "dsq", dsq),
                    new XAttribute(XNamespace.Xmlns + "dc", dc),
                    new XAttribute(XNamespace.Xmlns + "wp", wp),
                    new XElement("channel",
                        new XElement("item",
                            new XElement("title", pageTitle),
                            new XElement("link", pageUrl),
                            new XElement(content + "encoded", string.Empty),
                            new XElement(dsq + "thread_identifier", pageId),
                            new XElement(wp + "post_date_gmt", string.Empty),
                            new XElement(wp + "comment_status", "open"),
                            CreateCommentsList(comments)
                ))));

            return doc;
        }

        public IList<XElement> CreateCommentsList(IEnumerable<FacebookComment> comments)
        {
            var list = new List<XElement>();

            foreach (var comment in comments)
            {
                list.Add(CreateComment(comment, "0"));
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

        public XElement CreateComment(FacebookComment comment, string parentId)
        {
            XNamespace wp = "http://wordpress.org/export/1.0/";

            var commentElement = new XElement(wp + "comment",
                new XElement(wp + "comment_id", comment.Id),
                new XElement(wp + "comment_author", comment.From.Name),
                new XElement(wp + "comment_author_email", string.Empty),
                new XElement(wp + "comment_author_url", string.Empty),
                new XElement(wp + "comment_author_IP", string.Empty),
                new XElement(wp + "comment_date_gmt", comment.CreatedTime.ToString("yyyy-MM-dd HH:MM:ss")),
                new XElement(wp + "comment_content", new XCData(comment.Message)),
                new XElement(wp + "comment_approved", "1"),
                new XElement(wp + "comment_parent", parentId)
            );

            return commentElement;
        }
    }
}
