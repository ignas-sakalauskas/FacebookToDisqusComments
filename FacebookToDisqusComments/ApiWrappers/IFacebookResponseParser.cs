using FacebookToDisqusComments.ApiWrappers.Dtos;

namespace FacebookToDisqusComments.ApiWrappers
{
    public interface IFacebookResponseParser
    {
        FacebookCommentsPage ParseFacebookCommentsPage(string jsonContent);
    }
}