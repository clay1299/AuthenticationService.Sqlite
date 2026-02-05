using System.IO;

namespace AuthenticationService.Sqlite.Helpers;
public static class DirectoryHelper
{
    public static string GetAppDataPath(string AppName, string FileName)
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dbDir = Path.Combine(appData, AppName);
        Directory.CreateDirectory(dbDir); 
        return Path.Combine(dbDir, FileName);
    }
}
