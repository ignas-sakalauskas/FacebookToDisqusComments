using System;
using FacebookToDisqusComments.ApiWrappers.Dtos;
using Newtonsoft.Json;

namespace FacebookToDisqusComments.ApiWrappers
{
    public class FacebookResponseParser : IFacebookResponseParser
    {
        public FacebookCommentsPage ParseFacebookCommentsPage(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentNullException(nameof(jsonContent));
            }

            return JsonConvert.DeserializeObject<FacebookCommentsPage>(jsonContent);
        }
    }
}
