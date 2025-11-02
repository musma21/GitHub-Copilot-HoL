using System.Threading.Tasks;

namespace MockWinAppInstaller.Services
{
    public class UpdateSimulator
    {
        public async Task<string> RunAsync()
        {
            await Task.Delay(500); // simulate work
            return "Update complete";
        }
    }
}
