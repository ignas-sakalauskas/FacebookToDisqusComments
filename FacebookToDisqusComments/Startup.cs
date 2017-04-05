using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using FacebookToDisqusComments.ApiWrappers;
using FacebookToDisqusComments.DataServices;

namespace FacebookToDisqusComments
{
    public class Startup
    {
        private readonly AppSettings _settings;
        private readonly IFacebookCommentsApiWrapper _facebookApi;
        private readonly IDisqusCommentsFormatter _diqusFormatter;
        private readonly IFileUtils _fileUtils;

        public Startup(IOptions<AppSettings> settings, IFacebookCommentsApiWrapper facebookApi, IDisqusCommentsFormatter disqusFormatter, IFileUtils fileUtils)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _facebookApi = facebookApi ?? throw new ArgumentNullException(nameof(facebookApi));
            _diqusFormatter = disqusFormatter ?? throw new ArgumentNullException(nameof(disqusFormatter));
            _fileUtils = fileUtils ?? throw new ArgumentNullException(nameof(fileUtils));
        }

        public async Task<ReturnCodes> RunAsync()
        {
            try
            {
                var accessToken = await _facebookApi.GetAccessTokenAsync(_settings.AppId, _settings.AppSecret);
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    Console.WriteLine("Error. Access token was not retrieved.");
                    return ReturnCodes.AccessTokenError;
                }

                Console.WriteLine("Access token retrieved.");

                var pageItems = _fileUtils.LoadCommentsPageInfo(_settings.InputFilePath);
                if (pageItems == null || !pageItems.Any())
                {
                    Console.WriteLine($"No input data retrieved from: {_settings.InputFilePath}");
                    return ReturnCodes.NoCommentsInfoError;
                }

                foreach (var page in pageItems)
                {
                    var comments = await _facebookApi.GetPageCommentsAsync(accessToken, page.FacebookPageId);
                    if (comments == null || !comments.Any())
                    {
                        Console.WriteLine($"No Facebook comments  retrieved.");
                        return ReturnCodes.FacebookGetCommentsError;
                    }

                    Console.WriteLine($"Page '{page.TargetPageTitle}'");

                    var disqusCommentsXml = _diqusFormatter.ConvertCommentsIntoXml(comments, page.TargetPageTitle, page.TargetPageUrl, page.TargetPageId);

                    var filePath = Path.Combine(_settings.OutputPath, $"{page.TargetPageTitle}.xml");
                    _fileUtils.SaveAsXml(disqusCommentsXml, filePath);
                    Console.WriteLine($"Disqus comments saved into: {filePath}");
                }

                return ReturnCodes.Success;
            }
            catch (FacebookApiException ex)
            {
                Console.WriteLine($"Facebook API error occurred: {ex.Message}");
                return ReturnCodes.FacebookGetTokenError;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                return ReturnCodes.UnexpectedError;
            }
        }
    }
}