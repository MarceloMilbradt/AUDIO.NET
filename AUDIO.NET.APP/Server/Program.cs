using AUDIO.NET.APP.Server.Hubs;
using AUDIO.NET.APP.Server.Implementation;
using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Services.Implementation;
using AUDIO.NET.APP.Server.Utils;
using MediatR;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Sinks.File;
using SmartLedKit;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
builder.Services.AddResponseCompression(options => 
    options.MimeTypes = ResponseCompressionDefaults
    .MimeTypes
    .Concat(new[] {"application/octect-stream"})
);

var servicesConfiguration = FileManager.ReadFromJsonFile<Configuration>("configs.json");

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>loggingBuilder.AddSerilog(Log.Logger, dispose: true));



builder.Services.AddSingleton<ISpotifyAPI>(new SpotifyApi(servicesConfiguration));
builder.Services.AddSingleton<ISmartLedManager>(
    new SmartLedManager(
        servicesConfiguration.tuya.accessId,
        servicesConfiguration.tuya.apiSecret,
        servicesConfiguration.tuya.anyDeviceId
        )
    );

builder.Services.AddHostedService<SpotifyHostedService>();

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

