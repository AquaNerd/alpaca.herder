using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new { BaseAddress = builder.HostEnvironment.BaseAddress });
builder.Services.AddSingleton<HubConnection>(sp => {
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/marketdatahub"))
        .WithAutomaticReconnect()
        .Build();
});

await builder.Build().RunAsync();
