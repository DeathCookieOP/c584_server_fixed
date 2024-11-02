using AngularApp1.Server;
using DataModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WorldContext>(
        (options) => { 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
        }
    );
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DbContext>();

//Dependency Injection creates the object
//Transient means creating an object everytime you need it
//Scoped means creating an obj everytime you need it for a requestion
//Singleton means creating an obj only once
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddAuthentication(options => {
    //Bearer is 
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    //Send Jwt Token as value (that was generated when logged in) thru Bearer token
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new()
    {
        RequireAudience = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true, //most important one; generated and now we need to validate
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:SecurityKey"]!)),
    }; 
});


WebApplication app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
