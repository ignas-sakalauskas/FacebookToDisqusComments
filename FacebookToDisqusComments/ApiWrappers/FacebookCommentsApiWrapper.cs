using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.ApiWrappers
{
    public class FacebookCommentsApiWrapper : IFacebookCommentsApiWrapper
    {
        private readonly IFacebookResponseParser _responseParser;
        private readonly Func<HttpClient> _httpClientFactory;

        public FacebookCommentsApiWrapper(Func<HttpClient> httpClientFactory, IFacebookResponseParser responseParser)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _responseParser = responseParser ?? throw new ArgumentNullException(nameof(responseParser));
        }

        public async Task<string> GetAccessToken(string appId, string appSecret)
        {
            const string accessTokenKey = "access_token=";

            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentNullException(nameof(appId));
            }

            if (string.IsNullOrWhiteSpace(appSecret))
            {
                throw new ArgumentNullException(nameof(appSecret));
            }

            using (var client = _httpClientFactory())
            {
                var uri = new Uri($"https://graph.facebook.com/oauth/access_token?client_id={appId}&client_secret={appSecret}&grant_type=client_credentials");

                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new FacebookApiException("Http client response was not successful.");
                }

                var content = await response.Content.ReadAsStringAsync();
                if (!content.Contains(accessTokenKey))
                {
                    throw new FacebookApiException("Http client response doesn't contain access token.");
                }

                return content.Replace(accessTokenKey, string.Empty);
            }
        }

        // TODO add children comments population - refer to the POC implementation
        public async Task<IList<FacebookComment>> GetPageComments(string accessToken, string pageId)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (string.IsNullOrWhiteSpace(pageId))
            {
                throw new ArgumentNullException(nameof(pageId));
            }

            using (var client = _httpClientFactory())
            {
                var uri = new Uri($"https://graph.facebook.com/comments?id={pageId}&access_token={accessToken}");

                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new FacebookApiException("Http client response was not successful.");
                }

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new FacebookApiException("Http client response is empty.");
                }

                var commentsPage = _responseParser.ParseFacebookCommentsPage(content);

                return commentsPage?.Comments ?? new List<FacebookComment>();
            }
        }
    }
}
