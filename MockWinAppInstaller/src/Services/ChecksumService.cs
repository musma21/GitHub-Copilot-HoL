using System.Security.Cryptography;
using System.Text;

namespace MockWinAppInstaller.Services
{
    public class ChecksumService
    {
        public string Sha256(string text)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(text);
            var hash = sha.ComputeHash(bytes);
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
