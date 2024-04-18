using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

//Services for authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();


await builder.Build().RunAsync();
