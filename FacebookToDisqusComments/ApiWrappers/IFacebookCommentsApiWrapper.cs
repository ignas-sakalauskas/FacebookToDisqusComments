using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacebookToDisqusComments
{
    public interface IFacebookCommentsApiWrapper
    {
        Task<string> GetAccessToken(string appId, string appSecret);
        Task<IList<FacebookComment>> GetPageComments(string accessToken, string pageId);
    }
}