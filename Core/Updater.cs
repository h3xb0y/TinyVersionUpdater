using TinyVersionUpdater.Settings;
using System.Text.Json;

namespace TinyVersionUpdater
{
  public class Updater
  {
    private Config _config;
    
    public string LastVersion()
    {
      InitializeIfNeeded();

      if (_config == null)
        return default;
      
      return "0.0.0.5"; // todo
    }

    private void InitializeIfNeeded()
    {
      if (_config != null)
        return;
      
      Config config = null;
      
      try
      {
        config = JsonSerializer.Deserialize<Config>("updater.config");
      }
      catch
      {
        // ignored
      }

      _config = config;
    }
  }
}