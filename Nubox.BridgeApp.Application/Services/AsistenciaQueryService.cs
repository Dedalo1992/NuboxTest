using Nubox.BridgeApp.Application.Dto;
using Nubox.BridgeApp.Domain.Interfaces;

namespace Nubox.BridgeApp.Application.Services
{
    public sealed class AsistenciaQueryService
    {
        private readonly IResumenAsistenciaRepository _resumenAsistenciaRepository;
        public AsistenciaQueryService(IResumenAsistenciaRepository resumenAsistenciaRepository) => _resumenAsistenciaRepository = resumenAsistenciaRepository;

        public async Task<IReadOnlyList<ResumenAsistenciaDto>> GetAsync(string empresaId, string periodoClave, CancellationToken ct)
        {
            var items = await _resumenAsistenciaRepository.GetByPeriodoAsync(empresaId, periodoClave, ct);

            return items.Select(x => new ResumenAsistenciaDto
            {
                EmpresaId = x.EmpresaId,
                TrabajadorId = x.TrabajadorId,
                PeriodoClave = x.PeriodoClave,
                Rut = x.Rut,
                HorasTrabajadas = x.HorasTrabajadas,
                HorasExtras = x.HorasExtras,
                Ausencias = x.Ausencias,
                AusenciaLicenciaMedica = x.AusenciaLicenciaMedica,
                TotalHorasRemunerables = x.TotalHorasRemunerables,
                Version = x.Version,
                CorrelationId = x.CorrelationId,
                CalculatedAtUtc = x.CalculatedAtUtc
            }).ToList();
        }
    }
}
