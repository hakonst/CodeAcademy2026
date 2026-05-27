using Kaffebar.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Lim inn et gyldig JWT access token"
        };
        var components = document.Components ??= new OpenApiComponents();
        components.SecuritySchemes!["Bearer"] = scheme;
        return Task.CompletedTask;
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();

// Delt in-memory ordrelagring, injisert i OrdersController
builder.Services.AddSingleton<Dictionary<Guid, OrderResponse>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .AddPreferredSecuritySchemes("Bearer")
        .AddHttpAuthentication("Bearer", http => http.Token = ""));
}

app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
