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

namespace TinyVersionUpdater
{
  public class Updater
  {
    private static Config _config;
    public static Config Config => _config;

    public Updater()
    {
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

    public Subject<bool> IsNeedUpdate()
    {
      var result = new Subject<bool>();
      var versionInfo = FileVersionInfo.GetVersionInfo(_config.ExecutablePath);
      var version = new Version(versionInfo.FileVersion ?? "0.0.0.0");

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
        }, _ => version.OnNext(new Version(0,0,0,0)));

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
          .Download(_config?.FtpConfig?.Path + version + ".zip")
          .Subscribe(ParseResponse, _ => result.OnNext(Result.Fail));

        void ParseResponse(WebResponse response)
        {
          ParseResponseAndCreateZip(response, version.ToString());

          ParseAndCreateZip(version.ToString());

          KillAllProcesses(new DirectoryInfo(_config.ExecutablePath).Name.Replace(".exe", ""));
          
          ReplaceAllFiles(version.ToString());
          
          result.OnNext(Result.Ok);
        }
      }
    }

    private void ParseAndCreateZip(string version)
    {
      Console.WriteLine("Extracting files...");
          
      if(Directory.Exists(version))
        Directory.Delete(version, true);
          
      ZipFile.ExtractToDirectory(version + ".zip", version);
    }

    private void KillAllProcesses(string name)
    {
      foreach (var process in Process.GetProcessesByName(name))
        process.Kill();
    }

    private void ReplaceAllFiles(string version)
    {
      Console.WriteLine("Copying files...");

      var currentPath = Directory.GetCurrentDirectory();
      var sourcePath = currentPath + "\\" + version;
      var targetPath = currentPath + "\\" + _config.RootDirectory;
      var skipDirectory = currentPath + "\\updater";
          
      foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
      {
        if(dirPath.Contains(skipDirectory))
          continue;
            
        Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
      }

      foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
      {
        if(newPath.Contains(skipDirectory))
          continue;
            
        File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
      }
          
      Console.WriteLine("Removing temp files...");
          
      File.Delete(version + ".zip");
      Directory.Delete(version.ToString(), true);
    }

    private void ParseResponseAndCreateZip(WebResponse response, string version)
    {
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
    }
  }
}