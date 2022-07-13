using AUDIO.NET.APP.Server.Hubs;
using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Utils.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Sinks.File;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
builder.Services.AddResponseCompression(options => 
    options.MimeTypes = ResponseCompressionDefaults
    .MimeTypes
    .Concat(new[] {"application/octect-stream"})
);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>loggingBuilder.AddSerilog(Log.Logger, dispose: true));

var spotifyListener = new ListenerBuilder()
    .UseRedirectUrl("https://localhost:5001/auth/Token")
    .WithClientId("18037b72c0344bffa9a093fdb0c9c3de")
    .AndClientSecret("55bf9ad995f64409bb4f5649d773d1bf")
    .Build();


builder.Services.AddSingleton<ISpotify>(spotifyListener);

var app = builder.Build();
app.UseResponseCompression();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<TrackHub>("/trackhub");

app.Run();

