using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IDbService
    {
        Task<List<Recording>> GetAllRecordingsAsync();
        Task<Recording> AddRecordingAsync(string name, DateTime date);
    }

    public class DbService : IDbService
    {
        private readonly AppDbContext _context;

        public DbService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recording>> GetAllRecordingsAsync()
        {
            try
            {
                var recordings = await _context.Recordings
                    .OrderByDescending(r => r.Date)
                    .AsNoTracking()
                    .ToListAsync();

                return recordings;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to fetch recordings: {ex.Message}", ex);
            }
        }

        public async Task<Recording> AddRecordingAsync(string name, DateTime date)
        {
            try
            {
                var recording = new Recording
                {
                    Name = name,
                    Date = date
                };

                _context.Recordings.Add(recording);
                await _context.SaveChangesAsync();

                return recording;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to add recording: {ex.Message}", ex);
            }
        }
    }
}