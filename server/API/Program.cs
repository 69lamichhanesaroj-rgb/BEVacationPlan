using Infra;
using Infra.Entities;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Service;
using Service.DemoData;
using Service.Security;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

//Creates web application builder and configures services and middleware for the application. 
var builder = WebApplication.CreateBuilder(args);

//Database connection
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=holiday.db";

//Create options object once and share it for the whole application lifetime. This is important for SQLite, as it will keep the connection open and allow multiple connections to the same database.
builder.Services.AddSingleton(new DataOptions().UseSQLite(connectionString, SQLiteProvider.Microsoft));  //new DataOptions() creates LinqToDB's configuration object

//Create anew connection for each HTTP request and dispose it when the request ends
builder.Services.AddScoped<AppDataConnection>(); //Scoped means one instance per http request

builder.Services.AddSingleton<HashingPassword>(); //singleton because it holds no data and no connection, so one shared copy is fine
builder.Services.AddScoped<SeedRole>();
builder.Services.AddScoped<SeedUser>();

// JWT configuration
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() // reads the Jwt section from appsettings.json and fills in a JwtOptions object with the values. This is possible because the property names match the JSON section names.
                 ?? throw new InvalidOperationException("Missing 'Jwt' section in configuration.");

if (Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<TokenGen>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => //This configures the JWT authentication middleware to validate tokens using the settings from JwtOptions
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();


var app = builder.Build(); // before this line we register the services and this line built the container and is locked, after this we request pipeline on app



// Create tables on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataConnection>();
    db.CreateTable<Role>(statementHeader:"CREATE TABLE IF NOT EXISTS {0}");
    db.CreateTable<User>(statementHeader:"CREATE TABLE IF NOT EXISTS {0}");
    db.Execute("Create Unique Index IF NOT EXISTS IX_Users_Username ON Users (Username)");
    scope.ServiceProvider.GetRequiredService<SeedRole>().SeedRoles();
    scope.ServiceProvider.GetRequiredService<SeedUser>().SeedUsers();
}



app.UseAuthentication();
app.UseAuthorization();

//maps controllers to handle HTTP requests.(Registers MVC controller support ,discovering classes that inherits ControllerBase
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();

app.Run();
