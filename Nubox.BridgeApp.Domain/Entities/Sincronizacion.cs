namespace Nubox.BridgeApp.Domain.Entities
{
    public enum SyncStatus { Procesando = 0, Completado = 1, Fallido = 2 }

    public sealed class Sincronizacion
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string EmpresaId { get; private set; } = default!;
        public string PeriodoClave { get; set; }
        public SyncStatus Status { get; private set; } = SyncStatus.Procesando;
        public string CorrelationId { get; private set; } = Guid.NewGuid().ToString("N");
        public DateTime InicioUtc { get; private set; } = DateTime.Now;
        public DateTime? FinUtc { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Sincronizacion() //EF
        {
            
        }

        private Sincronizacion(string empresaId, string periodoId, string? correlationId)
        {
            EmpresaId = string.IsNullOrWhiteSpace(empresaId) ? throw new ArgumentException("EmpresaId requerido") : empresaId.Trim();
            PeriodoClave = string.IsNullOrWhiteSpace(periodoId) ? throw new ArgumentException("PeriodoId requerido") : periodoId.Trim();
            if(!string.IsNullOrWhiteSpace(correlationId))
            {
                CorrelationId = correlationId!.Trim();
            }
        }

        public static Sincronizacion Inicio(string empresaId, string periodoId, string? correlationId = null)
        {
            return new Sincronizacion(empresaId, periodoId, correlationId);
        }

        public void TerminadoExito()
        {
            Status = SyncStatus.Completado;
            FinUtc = DateTime.UtcNow;
            ErrorMessage = null;
        }
        public void TerminadoError(string error)
        {
            Status = SyncStatus.Fallido;
            FinUtc = DateTime.UtcNow;
            ErrorMessage = error;
        }
    }
}
