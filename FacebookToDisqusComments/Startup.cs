using System;
using System.IO;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using FacebookToDisqusComments.DisqusComments;

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

        public async Task Run()
        {
            try
            {
                var accessToken = await _facebookApi.GetAccessToken(_settings.AppId, _settings.AppSecret);
                Console.WriteLine($"Access token retrieved.");

                var pageItems = _fileUtils.LoadCommentsPageInfo(_settings.InputFilePath);
                foreach (var page in pageItems)
                {
                    var comments = await _facebookApi.GetPageComments(accessToken, page.FacebookPageId);
                    Console.WriteLine($"Page '{page.TargetPageTitle}', count of comments retrieved: {comments.Count}");

                    var disqusCommentsXml = _diqusFormatter.ConvertCommentsIntoXml(comments, page.TargetPageTitle, page.TargetPageUrl.ToString(), page.TargetPageId);

                    var filePath = Path.Combine(_settings.OutputPath, $"{page.TargetPageTitle}.xml");
                    _fileUtils.SaveAsXml(disqusCommentsXml, filePath);
                    Console.WriteLine($"Disqus comments saved into: {filePath}");
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing application: {ex.Message}");
            }
        }
    }
}