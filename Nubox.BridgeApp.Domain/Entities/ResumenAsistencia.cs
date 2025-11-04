namespace Nubox.BridgeApp.Domain.Entities
{
    public class ResumenAsistencia
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string EmpresaId { get; private set; } = default!;
        public string TrabajadorId { get; private set; } = default!;
        public string PeriodoClave { get; private set; } = default!;
        public string? Rut { get; private set; }
        public int HorasTrabajadas { get; private set; }
        public int HorasExtras { get; private set; }
        public int Ausencias { get; private set; }
        public int AusenciaLicenciaMedica { get; private set; }
        public int TotalHorasRemunerables { get; private set; }
        public int Version { get; private set; } = 1;
        public string CorrelationId { get; private set; } = Guid.NewGuid().ToString("N");
        public DateTime CalculatedAtUtc { get; private set; } = DateTime.UtcNow;

        private ResumenAsistencia() { } // EF

        private ResumenAsistencia(
            string companyId,
            string employeeExternalId,
            string periodoClave,
            string? rut)
        {
            EmpresaId = string.IsNullOrWhiteSpace(companyId) ? throw new ArgumentException("CompanyId requerido") : companyId.Trim();
            TrabajadorId = string.IsNullOrWhiteSpace(employeeExternalId) ? throw new ArgumentException("EmployeeExternalId requerido") : employeeExternalId.Trim();
            PeriodoClave = string.IsNullOrWhiteSpace(periodoClave) ? throw new ArgumentException("PeriodKey requerido") : periodoClave.Trim();
            Rut = string.IsNullOrWhiteSpace(rut) ? null : rut.Trim();
        }

        public static ResumenAsistencia Create(
            string empresaId,
            string trabajadorId,
            string periodoId,
            string? rut,
            int horasTrabajadas,
            int horasExtras,
            int ausencias,
            int ausenciaLicenciaMedica,
            string correlationId)
        {
            var s = new ResumenAsistencia(empresaId, trabajadorId, periodoId, rut);
            s.Apply(horasTrabajadas, horasExtras, ausencias, ausenciaLicenciaMedica, correlationId);
            return s;
        }

        public void ActualizarTotales(
            int horasTrabajadas,
            int horasExtras,
            int ausencias,
            int ausenciaLicenciaMedica,
            string correlationId)
            => Apply(horasTrabajadas, horasExtras, ausencias, ausenciaLicenciaMedica, correlationId, isUpdate: true);

        private void Apply(
            int horasTrabajadas,
            int horasExtras,
            int ausencias,
            int ausenciaLicenciaMedica,
            string correlationId,
            bool isUpdate = false)
        {
            if (horasTrabajadas < 0 || horasExtras < 0 || ausencias < 0 || ausenciaLicenciaMedica < 0)
                throw new ArgumentOutOfRangeException("No se aceptan valores negativos.");

            HorasTrabajadas = horasTrabajadas;
            HorasExtras = horasExtras;
            Ausencias = ausencias;
            AusenciaLicenciaMedica = ausenciaLicenciaMedica;
            TotalHorasRemunerables = horasTrabajadas + horasExtras;

            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString("N") : correlationId.Trim();
            CalculatedAtUtc = DateTime.UtcNow;

            if (isUpdate) Version++;
        }
    }
}
