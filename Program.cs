using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using People.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Set up Azure Key Vault Secrets.
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential(new DefaultAzureCredentialOptions()
    {
        ManagedIdentityClientId = builder.Configuration["ManagedIdentityClientId"]
    }));

// Add services to the container.
builder.Services.AddControllersWithViews();

// PeopleDbContext
builder.Services.AddDbContext<PeopleDbContext>(options =>
{
    if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
    {
        options.UseInMemoryDatabase("People");
    }
    else
    {
        var connectionStringSecretName = builder.Configuration.GetValue<string>("SecretName");        

        if(connectionStringSecretName is not null)
        {            
            options.UseSqlServer(builder.Configuration.GetValue<string>(connectionStringSecretName));  
        }
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");        
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
