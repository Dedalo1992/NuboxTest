namespace Nubox.BridgeApp.Application.Dto
{
    public class ResumenAsistenciaDto
    {
        public string EmpresaId { get; init; } = default!;
        public string TrabajadorId { get; init; } = default!;
        public string PeriodoClave { get; init; } = default!;
        public string? Rut { get; init; }
        public int HorasTrabajadas { get; init; }
        public int HorasExtras { get; init; }
        public int Ausencias { get; init; }
        public int AusenciaLicenciaMedica { get; init; }
        public int TotalHorasRemunerables { get; init; }
        public int Version { get; init; } = 1;
        public string CorrelationId { get; init; } = default!;
        public DateTime CalculatedAtUtc { get; init; } = DateTime.UtcNow;
    }
}
