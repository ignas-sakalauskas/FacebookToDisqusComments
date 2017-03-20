using Newtonsoft.Json;

namespace FacebookToDisqusComments.ApiWrappers.Dtos
{
    public class FacebookCommentUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
