using System;
using System.Reflection;

namespace TinyVersionUpdater.Settings
{
  [Serializable]
  public class Config
  {
    public string ExecutablePath { get; set; }
    public string RootDirectory { get; set; }

    public FtpConfig FtpConfig { get; set; }
  }
}