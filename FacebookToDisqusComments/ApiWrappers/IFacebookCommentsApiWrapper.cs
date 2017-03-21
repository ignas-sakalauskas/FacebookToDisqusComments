using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.ApiWrappers
{
    public interface IFacebookCommentsApiWrapper
    {
        Task<string> GetAccessToken(string appId, string appSecret);
        Task<IList<FacebookComment>> GetPageComments(string accessToken, string pageId);
    }
}