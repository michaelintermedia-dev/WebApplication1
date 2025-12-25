using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=recordings.db"));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");        
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

app.MapPost("/UploadAudio", async (HttpRequest request, AppDbContext _context) => 
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

    // 1. Save the file to a folder
    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

    var filePath = Path.Combine(folderPath, file.FileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // 2. Save the metadata to the Database
    var recording = new AudioRecording
    {
        FileName = file.FileName,
        FilePath = filePath
    };
    _context.Recordings.Add(recording);
    await _context.SaveChangesAsync();

    return Results.Ok(new { message = "Uploaded successfully!" });
});

app.MapHealthChecks("/healthz/live");

app.Run();
