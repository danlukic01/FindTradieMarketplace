// ===== PROGRAM.CS =====
// Program.cs
using FindTradie.Web.Services;
using FindTradie.Web.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using FindTradie.Shared.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IQuoteApiService, QuoteApiService>();

// Add HTTP Client for API calls
builder.Services.AddHttpClient("FindTradieAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add API Services
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<ITradieApiService, TradieApiService>();
builder.Services.AddScoped<IJobApiService, JobApiService>();
builder.Services.AddScoped<IQuoteApiService, QuoteApiService>();
// Add Authorization
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("TradieOnly", policy =>
        policy.RequireClaim("UserType", nameof(UserType.Tradie)));
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();