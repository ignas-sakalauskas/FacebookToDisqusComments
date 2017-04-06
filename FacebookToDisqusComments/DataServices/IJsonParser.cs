namespace FacebookToDisqusComments.DataServices
{
    public interface IJsonParser
    {
        T ParseJsonResponse<T>(string jsonContent) where T : class;
    }
}