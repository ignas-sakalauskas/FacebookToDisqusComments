using Newtonsoft.Json;
using System.Collections.Generic;

namespace FacebookToDisqusComments
{
    public class FacebookCommentsPage
    {
        [JsonProperty("data")]
        public IList<FacebookComment> Comments { get; set; }
    }
}
