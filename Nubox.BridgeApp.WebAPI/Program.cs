using Microsoft.AspNetCore.Authentication;
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

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//    options.UseSqlServer(connectionString);
//});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("NuboxBridgeDb");
});

static IAsyncPolicy<HttpResponseMessage> Retry()
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
    c.Timeout = TimeSpan.FromSeconds(15);
    c.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["PartnerApi:ApiKey"]!);
})
.AddPolicyHandler(Retry())
.AddPolicyHandler(Breaker());

builder.Services.AddScoped<IResumenAsistenciaRepository, ResumenAsistenciaRepository>();
builder.Services.AddScoped<ISincronizacionRepository, SincronizacionRepository>();
builder.Services.AddScoped<AsistenciaSyncService>();
builder.Services.AddScoped<AsistenciaQueryService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

secured.MapPost("/sync/asistencia", async (string empresaId, PeriodoClave periodoClave, AsistenciaSyncService svc, CancellationToken ct) =>
{
    var ejecucion = await svc.SincronizacionAsync(empresaId, periodoClave, ct);
    return Results.Ok(new { ejecucion.Id, ejecucion.Status, ejecucion.CorrelationId });
});

secured.MapPost("/sync/asistencia/upload", 
    async (
    AsistenciaUploadForm form,
    AsistenciaSyncService svc,
    ExcelUploadService excelSvc,
    CancellationToken ct) =>
{
    if (form.File == null || form.File.Length == 0)
        return Results.BadRequest("Archivo vacío o no proporcionado.");

    var dtos = await excelSvc.ParseAsync(form.File.OpenReadStream(), ct);

    var run = await svc.SincronizacionDesdeArchivoAsync(form.EmpresaId, form.PeriodoClave, dtos, ct);

    return Results.Ok(new { run.Id, run.Status, run.CorrelationId });
});


secured.MapGet("/payroll/asistencia/{empresaId}/{periodoClave}",
async (string empresaId, string periodoClave, AsistenciaQueryService q, CancellationToken ct) =>
{
    var items = await q.GetAsync(empresaId, periodoClave, ct);
    return Results.Ok(new { empresaId, periodoClave, items, generatedAtUtc = DateTime.UtcNow });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
