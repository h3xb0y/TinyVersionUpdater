using System;

namespace TinyVersionUpdater.Settings
{
  [Serializable]
  public class FtpConfig
  {
    public string Path { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
  }
}