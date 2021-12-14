namespace TinyVersionUpdaterConsole
{
  public interface ICommand
  {
    string Name();
    
    Result Execute(string[] args);
  }
}