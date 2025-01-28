using System.ComponentModel;
using API.Middleware;
using API.RequestHelpers;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//connectie maken met de database via de gegeven connectionstring
builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//een service voorzien voor de meegegeven repositories zolang het http request geldt
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//een service voorzien voor de unit of work repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddCors();

//een service voorzien voor de connectie met Redis tijdens de volledige levensduur van het project
builder.Services.AddSingleton<IConnectionMultiplexer>(config => {
    var connectionstring = builder.Configuration.GetConnectionString("Redis");
    if(connectionstring == null) throw new Exception("Kan de Redis connectionString niet vinden.");
    var configuration = ConfigurationOptions.Parse(connectionstring, true);
    return ConnectionMultiplexer.Connect(configuration);
});

//een service voorzien om data weg te schrijven of op te halen vanuit de Redis database tijdens de volledige levensduur van het project
builder.Services.AddSingleton<ICartService, CartService>();

//een service voorzien om data op te halen of weg te schrijven naar de Redis database 1 voor cache gegevens
builder.Services.AddSingleton<IResponseCahceService, ResponseCacheService>();

//services voorzien voor .NET Identity
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<StoreContext>();

//een service voorzien voor de meegegeven repositories zolang het http request geldt
builder.Services.AddScoped<IPaymentService, PaymentService>();

//een service voorzien voor SignalR
builder.Services.AddSignalR();

//een service voorzien voor de Coupon
builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddScoped<UploadHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

//enkel request van dit adres toestaan in het project, vermijden van request gestuurd door andere websites
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>(); //endpoint api
app.MapHub<NotificationHub>("/hub/notifications");
app.MapFallbackToController("Index", "Fallback");

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context, userManager);
}
catch(Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
