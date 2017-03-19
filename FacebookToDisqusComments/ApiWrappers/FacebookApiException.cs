using System;

namespace FacebookToDisqusComments.ApiWrappers
{
    public class FacebookApiException : Exception
    {
        public FacebookApiException()
        {
        }

        public FacebookApiException(string message)
        : base(message)
        {
        }

        public FacebookApiException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
