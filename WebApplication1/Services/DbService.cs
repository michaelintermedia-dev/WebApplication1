using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IDbService
    {
        Task<List<Recording>> GetAllRecordingsAsync();
        Task<Recording> AddRecordingAsync(string name, DateTime date);
        //Task SaveAnalysisResultAsync(int audioId, WhisperResult result);
    }

    public class DbService : IDbService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DbService> _logger;

        public DbService(AppDbContext context, ILogger<DbService> logger)
        {
            _context = context;
            _logger = logger;
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