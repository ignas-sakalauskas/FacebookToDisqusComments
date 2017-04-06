using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using FacebookToDisqusComments.DataServices;

namespace FacebookToDisqusComments.ApiWrappers
{
    public class FacebookCommentsApiWrapper : IFacebookCommentsApiWrapper
    {
        private readonly IJsonParser _jsonParser;
        private readonly Func<HttpClient> _httpClientFactory;

        public FacebookCommentsApiWrapper(Func<HttpClient> httpClientFactory, IJsonParser jsonParser)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonParser = jsonParser ?? throw new ArgumentNullException(nameof(jsonParser));
        }

        public async Task<string> GetAccessTokenAsync(string appId, string appSecret)
        {
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
                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new FacebookApiException("Http client response was empty.");
                }

                var tokenObject = _jsonParser.ParseJsonResponse<FacebookAccessToken>(content);
                if (tokenObject == null)
                {
                    throw new FacebookApiException("Token parsing failed.");
                }

                return tokenObject.AccessToken;
            }
        }

        public async Task<IList<FacebookComment>> GetPageCommentsAsync(string accessToken, string pageId)
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

                var commentsPage = _jsonParser.ParseJsonResponse<FacebookCommentsPage>(content);
                if (commentsPage?.Comments == null || commentsPage.Comments.Any() == false)
                {
                    return new List<FacebookComment>();
                }

                // Recursive call to retrieve all child comments (replies)
                foreach (var comment in commentsPage.Comments)
                {
                    comment.Children = await GetPageCommentsAsync(accessToken, comment.Id);
                }

                return commentsPage.Comments;
            }
        }
    }
}
