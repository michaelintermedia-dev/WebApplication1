using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IUploadService
{
    Task<(bool Success, string Message, int? RecordingId)> UploadAudioAsync(IFormFile file, AppDbContext context);
}

public class UploadService : IUploadService
{
    private readonly string _uploadsFolder;

    public UploadService()
    {
        _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    }

    public async Task<(bool Success, string Message, int? RecordingId)> UploadAudioAsync(IFormFile file, AppDbContext context)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                return (false, "No file provided", null);
            }

            // Create uploads folder if it doesn't exist
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            // Save file to disk
            var filePath = Path.Combine(_uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save metadata to database
            var recording = new Recording
            {
                Name = file.FileName,
                Date = DateTime.Now
            };
            context.Recordings.Add(recording);
            await context.SaveChangesAsync();

            return (true, "Uploaded successfully!", recording.Id);
        }
        catch (Exception ex)
        {
            return (false, $"Upload failed: {ex.Message}", null);
        }
    }
}