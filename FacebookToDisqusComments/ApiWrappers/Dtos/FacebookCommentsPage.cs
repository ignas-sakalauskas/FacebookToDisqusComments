using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacebookToDisqusComments.ApiWrappers.Dtos
{
    public class FacebookCommentsPage
    {
        [JsonProperty("data")]
        public IEnumerable<FacebookComment> Comments { get; set; }
    }
}
