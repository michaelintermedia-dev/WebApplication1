using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapGet("/hello", () => "Hello, World!");

            app.MapPost("/UploadAudio", async (HttpRequest request, IUploadService uploadService) =>
            {
                if (!request.HasFormContentType)
                {
                    return Results.BadRequest(new { message = "Expected multipart/form-data request" });
                }

                var form = await request.ReadFormAsync();
                if (form?.Files == null || form.Files.Count == 0)
                {
                    return Results.BadRequest(new { message = "No file provided" });
                }

                var file = form.Files[0];
                var (success, message, recordingId) = await uploadService.UploadAudioAsync(file);

                if (!success)
                {
                    return Results.BadRequest(new { message });
                }

                return Results.Ok(new { message, recordingId });
            });

            app.MapGet("/GetRecordings", async (IDbService dbService) =>
            {
                try
                {
                    var recordings = await dbService.GetAllRecordingsAsync();
                    return Results.Ok(recordings);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            });
        }
    }
}