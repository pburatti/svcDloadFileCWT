using Microsoft.AspNetCore.StaticFiles;
using Serilog;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using webapi_DLoadFile.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// remove default logging providers
builder.Logging.ClearProviders();
// Configure Serilog
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Logger = logger;
builder.Host.UseSerilog();

Log.Information("Starting web application");

try
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.MapGet("/fileDownload/", (string fileName, ILogger<Program> loggerInput) =>
    {
        loggerInput.LogInformation($"Richiesta interna downloadFile {fileName}");

        if ((fileName.Length % 4 == 0) && Regex.IsMatch(fileName, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            fileName = Common.base64Decode(fileName);

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contenttype))
            contenttype = "application/octet-stream";

        return Results.File(fileName, contentType: contenttype.ToString(), Path.GetFileName(fileName));


    });
    //call from internet user
    app.MapGet("/downloadFile/", async (string encfileName, ILogger<Common> loggerInput) =>
    {
        loggerInput.LogInformation($"Richiesta esterna downloadFile {encfileName}");
        if ((encfileName.Length % 4 == 0) && Regex.IsMatch(encfileName, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            encfileName = Common.base64Decode(encfileName);
        var result = await new Common(loggerInput).CWTIntenalGetFile(encfileName);
        //var result = await new Common(loggerInput).CwtInternalFileGet(encfileName);
        //var result =  new Common(loggerInput).test(encfileName);
        result.Seek(0, SeekOrigin.Begin);
        loggerInput.LogInformation($"Richiesta esterna downloadFilecompletata");
        return Results.File(result, contentType: "application/octet-stream", Path.GetFileName(encfileName));
    });

    app.Run();
}
catch (Exception ex) {
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    await Log.CloseAndFlushAsync();
}
