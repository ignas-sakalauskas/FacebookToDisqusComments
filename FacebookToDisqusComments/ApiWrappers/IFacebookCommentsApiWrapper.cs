using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.ApiWrappers
{
    public interface IFacebookCommentsApiWrapper
    {
        Task<string> GetAccessTokenAsync(string appId, string appSecret);
        Task<IList<FacebookComment>> GetPageCommentsAsync(string accessToken, string pageId);
    }
}