using System;

namespace FacebookToDisqusComments.DataServices.Dtos
{
    public class CommentsPageInfo
    {
        public string FacebookPageId { get; set; }
        public Uri TargetPageUrl { get; set; }
        public string TargetPageTitle { get; set; }
        public string TargetPageId { get; set; }
    }
}
