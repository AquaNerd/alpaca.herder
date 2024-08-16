using Alpaca.Markets;
using AlpacaHerder.Components;
using AlpacaHerder.Configuration;
using AlpacaHerder.Hubs;
using AlpacaHerder.Services;
using AlpacaHerder.Workers;
using Blazr.RenderState.Server;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddSignalR()
    .AddHubOptions<MarketDataHub>(options => {
        options.EnableDetailedErrors = true;
    });

builder.Services.AddResponseCompression(options => {
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddOptions();
builder.Services.Configure<AlpacaConfig>(builder.Configuration.GetSection(nameof(AlpacaConfig)));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddSingleton<IMarketDataHub, MarketDataHub>();
builder.Services.AddSingleton<IAlpacaDataStreamingClient>(sp => {
    var config = sp.GetService<IOptions<AlpacaConfig>>();
    return Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(
        new SecretKey(config.Value.ApiKey, config.Value.ApiSecret));
});
builder.Services.AddSingleton<IStreamingDataService, StreamingDataService>();

builder.Services.AddHostedService<MarketDataStreamer>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("localhost:5178") });

builder.AddBlazrRenderStateServerServices();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
} else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<MarketDataHub>("/marketdatahub");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AlpacaHerder.Client._Imports).Assembly);

app.Run();
