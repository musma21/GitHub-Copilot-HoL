// ============================================================================
// File: FileService.cs
// Purpose: Thin abstraction for file existence checks (extensible if future
//          mocking or additional IO helpers are needed).
// ============================================================================
namespace MockWinAppInstaller.Services
{
    /// <summary>
    /// Basic file utility methods.
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// Determine if a file exists at the given path.
        /// </summary>
        public bool Exists(string path) => System.IO.File.Exists(path);
    }
}
