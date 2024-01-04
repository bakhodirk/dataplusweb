using Blazor.SubtleCrypto;
using Blazored.LocalStorage;
using DataPlusWeb.Client;
using DataPlusWeb.Handlers;
using DataPlusWeb.Client.Provider;
using DataPlusWeb.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DataPlusWeb.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<AlertService>();
//builder.Services.AddHttpClient("AuthAPI", options =>
//{
//    options.BaseAddress = new Uri("https://localhost:7086/");
//}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddSubtleCrypto(opt =>
    opt.Key = "ELE9xOyAyJHCsIPLMbbZHQ7pVy7WUlvZ60y5WkKDGMSw5xh5IM54kUPlycKmHF9VGtYUilglL8iePLwr");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddScoped<CustomHttpHandler>();

await builder.Build().RunAsync();
