using Azure.Core;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IDeviceService
    {
        Task RegisterDeviceAsync(string token, string platform);
    }
    public class DeviceService : IDeviceService
    {
        private readonly DbService _dbService;

        public DeviceService(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task RegisterDeviceAsync(string token, string platform)
        {
            var device = new Device
            {
                Token = token,
                Platform = platform,
                RegisteredAt = DateTime.UtcNow
            };

            await _dbService.RegisterDeviceAsync(device);


        }
    }
}
