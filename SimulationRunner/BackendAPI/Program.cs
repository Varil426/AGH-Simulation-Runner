using Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces;
using BackendAPI.Security;
using BackendAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add configuration

// Add services to the container.
builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Application.User.User>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDefaultIdentity<Domain.User>().AddEntityFrameworkStores<DataContext>();
builder.Services
    .AddIdentityCore<Domain.User>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<Domain.User>>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

//builder.Services.AddIdentityServer().AddApiAuthorization<Domain.User, DataContext>();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            /*ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = Configuration["JWT:ValidAudience"],
            ValidIssuer = Configuration["JWT:ValidIssuer"],*/
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])) // TODO Add secret
        };
    });
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

builder.Services.AddMediatR(typeof(Application.User.User).Assembly);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"), x => x.MigrationsAssembly(typeof(DataContext).Assembly.GetName().Name));
    });
}
else
{
    // TODO
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<DataContext>();
    context?.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

//app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
