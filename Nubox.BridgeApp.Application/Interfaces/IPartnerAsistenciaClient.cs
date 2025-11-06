using Nubox.BridgeApp.Application.Dto;
using Nubox.BridgeApp.Domain.ValueObjects;

namespace Nubox.BridgeApp.Application.Interfaces
{
    public interface IPartnerAsistenciaClient
    {
        Task<IReadOnlyList<PartnerAsistenciaDto>> GetAsistenciaAsync(string empresaId, PeriodoClave periodoClave, CancellationToken cancellationToken);
    }
}
