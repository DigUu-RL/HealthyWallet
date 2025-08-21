using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthyWallet.Infrastructure.CrossCutting;
using HealthyWallet.Infrastructure.CrossCutting.Conventions;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();
builder.Services.AddRepositories();
builder.Services.AddDbContexts(configuration);
builder.Services.AddSingletonDependencies(configuration);
builder.Services.AddMiddlewares();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options => options.Conventions.Add(new RoutePrefixConvention("api")))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

        var converters = new List<JsonConverter>
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
        };
        
        converters.ForEach(converter => options.JsonSerializerOptions.Converters.Add(converter));
    });

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCors();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Healthy Wallet API",
        Version = "v1",
        Description = "Healthy Wallet API with JWT Authentication."
    });
    
    string xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string path = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(path)) options.IncludeXmlComments(path, includeControllerXmlComments: true);
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insert the JWT token in the format: **Bearer {your_token}**"
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
            Array.Empty<string>()
        }
    });
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthyWallet API v1");
        options.DocumentTitle = "HealthyWallet API Docs";
        options.RoutePrefix = string.Empty;
    });
}

app.UseMiddlewares();

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
