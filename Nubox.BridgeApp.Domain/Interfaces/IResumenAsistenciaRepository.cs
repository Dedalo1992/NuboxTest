using Nubox.BridgeApp.Domain.Entities;

namespace Nubox.BridgeApp.Domain.Interfaces
{
    public interface IResumenAsistenciaRepository
    {
        Task UpsertAsync(ResumenAsistencia resumenAsistencia, CancellationToken ct);
        Task<IReadOnlyList<ResumenAsistencia>> GetByPeriodoAsync(string empresaId, string periodoClave, CancellationToken ct);
    }
}
