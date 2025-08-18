using System.Reflection;
using HealthyWallet.Infrastructure.CrossCutting;
using HealthyWallet.Infrastructure.CrossCutting.Conventions;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Serviços da aplicação
builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();
builder.Services.AddRepositories();
builder.Services.AddDbContexts(configuration);
builder.Services.AddSingletonDependencies(configuration);
builder.Services.AddMiddlewares();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options => 
    options.Conventions.Add(new RoutePrefixConvention("api"))
);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCors();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HealthyWallet API",
        Version = "v1",
        Description = "API da HealthyWallet com autenticação JWT."
    });
    
    string xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
    if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: **Bearer {seu_token}**"
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
