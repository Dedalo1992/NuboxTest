namespace Nubox.BridgeApp.Application.Dto
{
    public class PartnerAsistenciaDto
    {
        public string TrabajadorID { get; init; } = default!;
        public string? Rut { get; init; }
        public int HorasTrabajadas { get; init; }
        public int HorasExtras { get; init; }
        public int Ausencias { get; init; }
        public int AusenciaLicenciaMedica { get; init; }
    }
}
