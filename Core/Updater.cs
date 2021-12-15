using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using TinyVersionUpdater.Settings;
using System.Text.Json;
using TinyVersionUpdater.FtpWorkflow;
using TinyVersionUpdaterConsole;

namespace TinyVersionUpdater
{
  public class Updater
  {
    private Config _config;

    public Updater()
      => Initialize();

    public Subject<bool> IsNeedUpdate()
    {
      var result = new Subject<bool>();
      var versionInfo = FileVersionInfo.GetVersionInfo(_config.ExecutablePath);
      var version = new Version(versionInfo.FileVersion);

      LastVersion()
        .Subscribe(lastVersion =>
        {
          result.OnNext(lastVersion.CompareTo(version) == 1);
        });

      return result;
    }
    
    public Subject<Version> LastVersion()
    {
      var version = new Subject<Version>();
      
      new FtpService(_config?.FtpConfig)
        .List()
        .Subscribe(response =>
        {
          using var stream = response.GetResponseStream();
          using var streamReader = new StreamReader(stream ?? throw new Exception());

          var allList = streamReader
            .ReadToEnd()
            .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
          
          var lastVersion = allList
            .Select(x => new Version(x.Replace(".zip", "")))
            .OrderByDescending(y => y.GetHashCode())
            .First();

          version.OnNext(lastVersion);
        }, error => Console.WriteLine(error));

      return version;
    }

    public Subject<Result> LoadLastVersion()
    {
      var result = new Subject<Result>();

      LastVersion().Subscribe(Load);

      return result;

      void Load(Version version)
      {
        new FtpService(_config?.FtpConfig)
          .Download(_config?.FtpConfig?.Path + version.ToString() + ".zip")
          .Subscribe(ParseResponse, error => Console.WriteLine(error));

        void ParseResponse(WebResponse response)
        {
          // PARSING RESPONSE
          using var stream = response.GetResponseStream();
          using var streamReader = new StreamReader(stream ?? throw new Exception());
          var buffer = new byte[2048];
          var fs = new FileStream(version + ".zip", FileMode.Create);
          var readCount = stream.Read(buffer, 0, buffer.Length);
          while (readCount > 0)
          {
            fs.Write(buffer, 0, readCount);
            readCount = stream.Read(buffer, 0, buffer.Length);
          }
          fs.Close();
          stream.Close();
          
          // EXTRACTING ZIP
          
          ZipFile.ExtractToDirectory(version + ".zip", version.ToString());
          
          // REPLACING FILES

          var sourcePath = version.ToString();
          var targetPath = _config.RootDirectory;
          
          foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

          foreach (var newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
          
          // DELETE TEMPORARY FILES
          
          File.Delete(version + ".zip");
          Directory.Delete(version.ToString(), true);
          
          // CALLS RESULT
          result.OnNext(Result.Ok);
        }
      }
    }
    
    private void Initialize()
    {
      if (_config != null)
        return;
      
      Config config = null;
      
      try
      {
        config = JsonSerializer.Deserialize<Config>(File.ReadAllText("updater.config"));
      }
      catch(Exception e)
      {
        Console.Write(e);
      }

      _config = config;
    }
  }
}