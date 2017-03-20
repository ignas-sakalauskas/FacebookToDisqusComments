using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacebookToDisqusComments.ApiWrappers.Dtos
{
    public class FacebookCommentsPage
    {
        [JsonProperty("data")]
        public IList<FacebookComment> Comments { get; set; }
    }
}
