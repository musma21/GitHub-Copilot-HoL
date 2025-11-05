// ============================================================================
// File: UpdateSimulator.cs
// Purpose: Simulates a firmware/update patch copy with progress reporting.
// Features:
//   * Async copy with configurable progress (0..100) scaled by bytes processed.
//   * Artificial delay (Task.Delay 30ms) to make UI progress visually noticeable.
//   * Used by MainViewModel.StartPatch for patch simulation & checksum verification.
// Notes:
//   * Real implementation would include transactional integrity & rollback logic.
//   * Cancellation token supported for future cancel button wiring.
// ============================================================================
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;


namespace MockWinAppInstaller.Services
{
    /// <summary>
    /// Provides an asynchronous simulation of a patch file copy w/ progress callbacks.
    /// </summary>
    public class UpdateSimulator
    {
        /// <summary>
        /// Simulate patching by copying the selected file to targetPath with progress callback.
        /// </summary>
        /// <param name="sourceFile">Original file path.</param>
        /// <param name="targetFolder">Destination folder (created if missing).</param>
        /// <param name="report">Reports progress 0..100.</param>
        /// <param name="token">Cancellation token (optional).</param>
        /// <returns>Destination file path.</returns>
        public async Task<string> CopyWithProgressAsync(string sourceFile, string targetFolder, IProgress<double> report, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(sourceFile) || !File.Exists(sourceFile)) throw new FileNotFoundException("Source file missing", sourceFile);
            Directory.CreateDirectory(targetFolder);
            string destPath = Path.Combine(targetFolder, Path.GetFileName(sourceFile));

            const int BufferSize = 64 * 1024;
            var buffer = new byte[BufferSize];
            long total = new FileInfo(sourceFile).Length;
            long copied = 0;
            report?.Report(0);

            using var src = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);
            using var dst = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true);
            int read;
            while ((read = await src.ReadAsync(buffer.AsMemory(0, BufferSize), token)) > 0)
            {
                await dst.WriteAsync(buffer.AsMemory(0, read), token);
                copied += read;
                double percent = total > 0 ? (copied / (double)total) * 100.0 : 100.0;
                report?.Report(percent);
                // Small artificial delay to make progress visible
                await Task.Delay(30, token);
            }
            report?.Report(100);
            return destPath;
        }
    }
}
