using DaimyoDataSolutions.API.Authentication;
using DaimyoDataSolutions.Application;
using DaimyoDataSolutions.Infrastructure;
//using DotNetEnv;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Env.Load();

// initialize Firebase
var firebaseConfigPath = Path.Combine(AppContext.BaseDirectory, "firebase-config.json");

string jsonTemplate = File.ReadAllText(firebaseConfigPath);

string GetSecret(string key) =>
    Environment.GetEnvironmentVariable(key) ?? builder.Configuration[key];

string privateKey = GetSecret("FIREBASE_PRIVATE_KEY")?
    .Replace("\\n", "\n");

string completedJson = jsonTemplate
    .Replace("{PRIVATE_KEY_ID}", GetSecret("FIREBASE_PRIVATE_KEY_ID"))
    .Replace("{PRIVATE_KEY}", privateKey)
    .Replace("{CLIENT_EMAIL}", GetSecret("FIREBASE_CLIENT_EMAIL"))
    .Replace("{CLIENT_ID}", GetSecret("FIREBASE_CLIENT_ID"));

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(completedJson)
});

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication("Firebase")
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", null);

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {   Title = "DaimyoDataSolutions API",
        Version = "v1",
        Description = "DaimyoDataSolutions API Documentation",
        Contact = new OpenApiContact
        {
            Name = "DaimyoDataSolutions team",
            Email = ""
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"Enter Firebase JWT token **_without_** the 'Bearer ' prefix."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseRouting();

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();