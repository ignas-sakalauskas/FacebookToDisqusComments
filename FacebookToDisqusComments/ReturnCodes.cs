namespace FacebookToDisqusComments
{
    public enum ReturnCodes
    {
        UnexpectedError = -1,
        Success = 0,
        AccessTokenError = 1,
        FacebookGetTokenError = 2,
        NoCommentsInfoError = 3,
        FacebookGetCommentsError = 4,
        DisqusConvertionError = 5
    }
}
