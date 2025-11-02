namespace MockWinAppInstaller.Services
{
    public class FileService
    {
        public bool Exists(string path) => System.IO.File.Exists(path);
    }
}
