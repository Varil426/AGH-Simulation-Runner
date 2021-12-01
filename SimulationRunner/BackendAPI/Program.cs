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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using BackendAPI.Docker;
using SimulationHandler;

var builder = WebApplication.CreateBuilder(args);

// Add configuration

// Add services to the container.
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithExposedHeaders("WWW-Authenticate");
    });
});*/

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
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])) // TODO Add secret
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddSingleton<ISimulationHandler, SimulationHandler.SimulationHandler>();
builder.Services.AddSingleton<IDockerContainerManager, DockerContainerManager>();

builder.Services.AddMediatR(typeof(Application.User.UserDto).Assembly);
builder.Services.AddAutoMapper(typeof(Application.User.UserDto).Assembly);

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                         .RequireAuthenticatedUser()
                         .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Application.User.UserDto>();
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseLazyLoadingProxies();

        if (Environment.GetEnvironmentVariable("IS_DOCKER") is null)
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"), x => x.MigrationsAssembly(typeof(DataContext).Assembly.GetName().Name));
        else
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLDocker"), x => x.MigrationsAssembly(typeof(DataContext).Assembly.GetName().Name));
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
