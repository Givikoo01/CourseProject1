using CourseProject1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CourseProject1.Models;
using CourseProject1.ServiceContracts;
using CourseProject1.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISearchEngine, SearchEngineService>();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Retrieve configuration values
var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value;
var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId").Value;
var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret").Value;
var keyVaultDirectoryID = builder.Configuration.GetSection("KeyVault:DirectoryID").Value;

// Create a credential using the configuration values
var credential = new ClientSecretCredential(keyVaultDirectoryID, keyVaultClientId, keyVaultClientSecret);

// Create a URI for the Key Vault URL
var keyVaultUri = new Uri(keyVaultURL);

// Register SecretClient as a singleton service
builder.Services.AddSingleton(provider => new SecretClient(keyVaultUri, credential));

// Register IUploadImage service
builder.Services.AddScoped<IUploadImage>(provider =>
{
    // Retrieve configuration
    var configuration = provider.GetRequiredService<IConfiguration>();

    // Retrieve SecretClient
    var secretClient = provider.GetRequiredService<SecretClient>();

    // Return a new instance of UploadImageService with SecretClient and IConfiguration
    return new UploadImageService(secretClient, configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();