using Nubox.BridgeApp.Application.Exceptions;
using Nubox.BridgeApp.Application.Interfaces;
using Nubox.BridgeApp.Domain.Entities;
using Nubox.BridgeApp.Domain.Interfaces;
using Nubox.BridgeApp.Domain.ValueObjects;

namespace Nubox.BridgeApp.Application.Services
{
    public sealed class AsistenciaSyncService
    {
        private readonly IPartnerAsistenciaClient _partnerAsistenciaClient;
        private readonly IResumenAsistenciaRepository _resumenAsistenciaRepository;
        private readonly ISincronizacionRepository _sincronizacionRepository;

        public AsistenciaSyncService(IPartnerAsistenciaClient partnerAsistenciaClient, IResumenAsistenciaRepository resumenAsistenciaRepository, ISincronizacionRepository sincronizacionRepository)
        {
            _partnerAsistenciaClient = partnerAsistenciaClient;
            _resumenAsistenciaRepository = resumenAsistenciaRepository;
            _sincronizacionRepository = sincronizacionRepository;
        }

        public async Task<Sincronizacion> SincronizacionAsync(string empresaId, PeriodoClave periodoClave, CancellationToken ct)
        {
            if(string.IsNullOrWhiteSpace(empresaId))
                throw new AppValidationException("empresaId es requerido");

            var correlationId = Guid.NewGuid().ToString("N");

            var ejecucion = await _sincronizacionRepository.IniciarAsync(empresaId, periodoClave.ToString(), correlationId, ct);

            try
            {
                var result = await _partnerAsistenciaClient.GetAsistenciaAsync(empresaId, periodoClave, ct);

                foreach(var x in result)
                {
                    var totalHoras = x.HorasTrabajadas + x.HorasExtras;

                    var resumen = ResumenAsistencia.Create(
                        empresaId: empresaId,
                        trabajadorId: x.TrabajadorID,
                        periodoId: periodoClave.ToString(),
                        rut: x.Rut,
                        horasTrabajadas: x.HorasTrabajadas,
                        horasExtras: x.HorasExtras,
                        ausencias: x.Ausencias,
                        ausenciaLicenciaMedica: x.AusenciaLicenciaMedica,
                        correlationId: correlationId
                    );

                    await _resumenAsistenciaRepository.UpsertAsync(resumen, ct);
                }

                await _sincronizacionRepository.TerminarAsync(ejecucion.Id, exito: true, error: null, ct);
                return ejecucion;
            }
            catch (Exception ex)
            {
                await _sincronizacionRepository.TerminarAsync(ejecucion.Id, exito: false, error: ex.Message, ct);
                throw;
            }
        }
    }
}
