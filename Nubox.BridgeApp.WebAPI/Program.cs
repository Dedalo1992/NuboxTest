using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nubox.BridgeApp.Application.Interfaces;
using Nubox.BridgeApp.Application.Services;
using Nubox.BridgeApp.Domain.Interfaces;
using Nubox.BridgeApp.Domain.ValueObjects;
using Nubox.BridgeApp.Infrastructure.Data;
using Nubox.BridgeApp.Infrastructure.Repositories;
using Nubox.BridgeApp.Infrastructure.Services;
using Nubox.BridgeApp.WebAPI.Models;
using Nubox.BridgeApp.WebAPI.Security;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' not found.");
    options.UseSqlServer(connectionString);
});

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseInMemoryDatabase("NuboxBridgeDb");
//});

static IAsyncPolicy<HttpResponseMessage> Reintento()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * Math.Pow(2, i)));
static IAsyncPolicy<HttpResponseMessage> Breaker()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient<IPartnerAsistenciaClient, PartnerAsistenciaClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["PartnerApi:BaseUrl"]!);
    c.DefaultRequestHeaders.Add("partner-api-key", builder.Configuration["PartnerApi:ApiKey"]!);
    c.Timeout = TimeSpan.FromSeconds(15);
})
.AddPolicyHandler(Reintento())
.AddPolicyHandler(Breaker());

builder.Services.AddScoped<IResumenAsistenciaRepository, ResumenAsistenciaRepository>();
builder.Services.AddScoped<ISincronizacionRepository, SincronizacionRepository>();
builder.Services.AddScoped<AsistenciaSyncService>();
builder.Services.AddScoped<AsistenciaQueryService>();
builder.Services.AddScoped<ExcelUploadService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = ApiKeyAuthenticationHandler.SchemeName;
    o.DefaultChallengeScheme = ApiKeyAuthenticationHandler.SchemeName;
}).
AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();

var secured = app.MapGroup("/");
secured.RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Gatilla sincronizacion desde Api del partner para obtener asistencias
secured.MapPost("/sync/asistencia",
async (string empresaId, PeriodoClave periodoClave, AsistenciaSyncService svc, CancellationToken ct) =>
{
    var ejecucion = await svc.SincronizacionAsync(empresaId, periodoClave, ct);
    return Results.Ok(new { ejecucion.Id, ejecucion.Status, ejecucion.CorrelationId });
});

//Obtiene asistencias desde un archivo Excel
var upload = secured.MapPost("/sync/asistencia/upload",
async (
    [FromForm] string empresaId,
    [FromForm] PeriodoClave periodoClave,
    [FromForm(Name = "file")] IFormFile file,
    AsistenciaSyncService svc,
    ExcelUploadService excelSvc,
    CancellationToken ct) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("Archivo vacío o no proporcionado.");

    using var stream = file.OpenReadStream();
    var dtos = await excelSvc.ParseAsync(stream, ct);
    var run = await svc.SincronizacionDesdeArchivoAsync(empresaId, periodoClave, dtos, ct);

    return Results.Ok(new { run.Id, run.Status, run.CorrelationId });
});

upload.DisableAntiforgery();

//Obtiene el resumen de asistencias ya procesadas para el calculo de remuneraciones
secured.MapGet("/payroll/asistencia/{empresaId}/{periodoClave}",
async (string empresaId, string periodoClave, AsistenciaQueryService q, CancellationToken ct) =>
{
    var items = await q.GetAsync(empresaId, periodoClave, ct);
    return Results.Ok(new { empresaId, periodoClave, items, generatedAtUtc = DateTime.UtcNow });
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // aplica migraciones pendientes en runtime
}

app.Run();
