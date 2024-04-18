using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//Services for authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>(); //to be deleted innit

await builder.Build().RunAsync();
