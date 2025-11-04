using Nubox.BridgeApp.Domain.Entities;
using Nubox.BridgeApp.Domain.Interfaces;
using Nubox.BridgeApp.Infrastructure.Data;

namespace Nubox.BridgeApp.Infrastructure.Repositories
{
    public class SincronizacionRepository : ISincronizacionRepository
    {
        private readonly AppDbContext _appDbContext;
        public SincronizacionRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;


        public async Task<Sincronizacion> IniciarAsync(string empresaId, string periodoClave, string? correlationId, CancellationToken ct)
        {
            var ejecucion = Sincronizacion.Inicio(empresaId, periodoClave, correlationId);
            _appDbContext.Sincronizaciones.Add(ejecucion);
            await _appDbContext.SaveChangesAsync(ct);
            return ejecucion;
        }

        public async Task TerminarAsync(Guid sincronizacionId, bool exito, string? error, CancellationToken ct)
        {
            var ejecucion = await _appDbContext.Sincronizaciones.FindAsync(new object?[] { sincronizacionId }, ct);
            if (ejecucion == null) return;

            if (exito)
            {
                ejecucion.TerminadoExito();
            }
            else
            {
                ejecucion.TerminadoError(error ?? "Error desconocido");
            }
            await _appDbContext.SaveChangesAsync(ct);
        }
    }
}
