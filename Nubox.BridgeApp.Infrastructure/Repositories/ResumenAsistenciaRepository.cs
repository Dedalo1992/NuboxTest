using Nubox.BridgeApp.Domain.Entities;
using Nubox.BridgeApp.Domain.Interfaces;
using Nubox.BridgeApp.Infrastructure.Data;

namespace Nubox.BridgeApp.Infrastructure.Repositories
{
    public class ResumenAsistenciaRepository : IResumenAsistenciaRepository
    {
        private readonly AppDbContext _appDbContext;
        public ResumenAsistenciaRepository(AppDbContext _appDbContext) => this._appDbContext = _appDbContext;
        public Task<IReadOnlyList<ResumenAsistencia>> GetByPeriodoAsync(string empresaId, string periodoClave, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task UpsertAsync(ResumenAsistencia resumenAsistencia, CancellationToken ct)
        {
            var existing = _appDbContext.ResumenesAsistencia
                .FirstOrDefault(x => x.EmpresaId == resumenAsistencia.EmpresaId
                                  && x.TrabajadorId == resumenAsistencia.TrabajadorId
                                  && x.PeriodoClave == resumenAsistencia.PeriodoClave);

            if (existing == null)
            {
                await _appDbContext.ResumenesAsistencia.AddAsync(resumenAsistencia, ct);
            }
            else
            {
                existing.ActualizarTotales(
                    resumenAsistencia.HorasTrabajadas,
                    resumenAsistencia.HorasExtras,
                    resumenAsistencia.Ausencias,
                    resumenAsistencia.AusenciaLicenciaMedica,
                    resumenAsistencia.CorrelationId);

            }
            await _appDbContext.SaveChangesAsync(ct);
        }
    }
}
