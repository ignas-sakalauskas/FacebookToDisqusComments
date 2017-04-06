using System;
using Newtonsoft.Json;

namespace FacebookToDisqusComments.DataServices
{
    public class JsonParser : IJsonParser
    {
        public T ParseJsonResponse<T>(string jsonContent) where T : class
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentNullException(nameof(jsonContent));
            }

            return JsonConvert.DeserializeObject<T>(jsonContent);
        }
    }
}
