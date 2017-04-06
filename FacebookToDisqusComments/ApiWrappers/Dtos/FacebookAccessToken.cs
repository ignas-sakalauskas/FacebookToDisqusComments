using Newtonsoft.Json;

namespace FacebookToDisqusComments.ApiWrappers.Dtos
{
    public class FacebookAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
