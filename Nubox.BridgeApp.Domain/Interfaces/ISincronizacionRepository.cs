using Nubox.BridgeApp.Domain.Entities;

namespace Nubox.BridgeApp.Domain.Interfaces
{
    public interface ISincronizacionRepository
    {
        Task<Sincronizacion> IniciarAsync(string empresaId, string periodoClave, string? correlationId,  CancellationToken ct);
        Task TerminarAsync(Guid sincronizacionId, bool exito, string? error, CancellationToken ct);
    }
}
