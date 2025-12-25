namespace WebApplication1
{
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This creates a table called "Recordings"
        public DbSet<AudioRecording> Recordings { get; set; }
    }
    public class AudioRecording
    {
        public int Id { get; set; }
        public string FileName { get; set; } // e.g., "rec_001.m4a"
        public string FilePath { get; set; } // e.g., "/uploads/audio/"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
