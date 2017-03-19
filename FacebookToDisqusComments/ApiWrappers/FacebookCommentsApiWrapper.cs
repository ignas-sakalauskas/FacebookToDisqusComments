using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FacebookToDisqusComments
{
    public class FacebookCommentsApiWrapper : IFacebookCommentsApiWrapper
    {
        public async Task<string> GetAccessToken(string appId, string appSecret)
        {
            const string AccessTokenKey = "access_token=";

            using (var client = new HttpClient())
            {
                var uri = new Uri($"https://graph.facebook.com/oauth/access_token?client_id={appId}&client_secret={appSecret}&grant_type=client_credentials");

                var response = await client.GetAsync(uri);
                if (response == null)
                {
                    throw new Exception("change with smart excep");
                }

                var content = await response.Content.ReadAsStringAsync();
                if (!content.Contains(AccessTokenKey))
                {
                    throw new Exception(" no auth token, change with smart excep");
                }

                return content.Replace(AccessTokenKey, string.Empty);
            }
        }

        public async Task<IList<FacebookComment>> GetPageComments(string accessToken, string pageId)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri($"https://graph.facebook.com/comments?id={pageId}&access_token={accessToken}");

                var response = await client.GetAsync(uri);
                if (response == null)
                {
                    throw new Exception("change with smart excep");
                }

                var content = await response.Content.ReadAsStringAsync();

                var commentsPage = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookCommentsPage>(content);
                if (commentsPage == null)
                {
                    throw new Exception(" no auth token, change with smart excep");
                }

                return commentsPage.Comments;
            }
        }
    }
}
