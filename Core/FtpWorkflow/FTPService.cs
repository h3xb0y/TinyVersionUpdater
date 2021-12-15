using System;
using System.Net;
using System.Reactive.Linq;
using TinyVersionUpdater.Settings;

namespace TinyVersionUpdater.FtpWorkflow
{
  public class FtpService
  {
    private readonly FtpConfig _config;
    
    public FtpService(FtpConfig config)
      => _config = config;

    public IObservable<WebResponse> List()
      => Do(WebRequestMethods.Ftp.ListDirectory, _config.Path);

    public IObservable<WebResponse> Download(string path)
    {
      return Do(WebRequestMethods.Ftp.DownloadFile, path);
    }

    private IObservable<WebResponse> Do(string method, string path)
    {
      var requestUri = new Uri(path);
      var request = WebRequest.Create(requestUri);
      request.Credentials = new NetworkCredential(_config.Login, _config.Password);
      request.Method = method;

      return Observable.FromAsyncPattern(request.BeginGetResponse, request.EndGetResponse)();
    }
  }
}