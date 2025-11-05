// ============================================================================
// File: ChecksumService.cs
// Purpose: Provides lightweight hashing utilities (SHA-256 string hash) and
//          an async streaming CRC16 (0x1021 poly) computation for files used
//          by the mock installer to verify source/destination integrity.
// Key Features Implemented (2025-11-05):
//   * Async CRC16 calculation with buffered streaming (64KB) to avoid loading
//     large files into memory.
//   * Human-readable size formatting (Bytes/KB/MB/GB).
//   * SHA-256 helper retained for potential future signature checks.
// Notes:
//   * CRC16 chosen to mirror legacy firmware tooling, not cryptographically
//     secure. For tamper detection consider SHA-256 comparison or code signing.
//   * All methods are synchronous-safe except ComputeFileCrc16Async which must
//     be awaited by callers.
// ============================================================================
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MockWinAppInstaller.Services
{
    /// <summary>
    /// Exposes hashing and CRC16 file verification helpers for the mock installer.
    /// </summary>
    public class ChecksumService
    {
        /// <summary>
        /// Compute SHA-256 hash of a UTF-8 string, returned as lowercase hex.
        /// </summary>
        public string Sha256(string text)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(text);
            var hash = sha.ComputeHash(bytes);
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        // CRC16 (poly 0x1021, initial 0xFFFF) over file contents, async streaming
        private const ushort Crc16Poly = 0x1021;
        private const ushort Crc16Init = 0xFFFF;
        /// <summary>
        /// Stream a file and compute CRC16 (XMODEM style polynomial 0x1021, init 0xFFFF).
        /// </summary>
        /// <param name="path">File path to read.</param>
        /// <returns>Uppercase 4-hex-character CRC16 string.</returns>
        public async Task<string> ComputeFileCrc16Async(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("File not found", path);
            ushort crc = Crc16Init;
            var buffer = new byte[64 * 1024];
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, buffer.Length, true);
            int read;
            while ((read = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < read; i++)
                {
                    crc ^= (ushort)(buffer[i] << 8);
                    for (int b = 0; b < 8; b++)
                    {
                        crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ Crc16Poly) : (ushort)(crc << 1);
                    }
                }
            }
            return crc.ToString("X4");
        }

        /// <summary>
        /// Human-readable size formatter with rounding to two decimals.
        /// </summary>
        public string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} Bytes";
            double kb = bytes / 1024.0;
            if (kb < 1024) return $"{kb:0.##} KB";
            double mb = kb / 1024.0;
            if (mb < 1024) return $"{mb:0.##} MB";
            double gb = mb / 1024.0;
            return $"{gb:0.##} GB";
        }
    }
}
