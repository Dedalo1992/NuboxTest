using Nubox.BridgeApp.Application.Dto;
using Nubox.BridgeApp.Application.Interfaces;
using Nubox.BridgeApp.Domain.ValueObjects;
using System.Text.Json;

namespace Nubox.BridgeApp.Infrastructure.Services
{
    public class PartnerAsistenciaClient : IPartnerAsistenciaClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public PartnerAsistenciaClient() => _httpClient = new HttpClient();

        public async Task<IReadOnlyList<PartnerAsistenciaDto>> GetAsistenciaAsync(string empresaId, PeriodoClave periodoClave, CancellationToken ct)
        {
            var respuesta = await _httpClient.GetAsync($"/attendance?companyId={empresaId}&periodKey={periodoClave}", ct);
            respuesta.EnsureSuccessStatusCode();
            var json = await respuesta.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<List<PartnerAsistenciaDto>>(json, _jsonSerializerOptions) ?? new();
        }
    }
}
