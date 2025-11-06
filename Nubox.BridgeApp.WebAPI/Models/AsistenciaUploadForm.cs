namespace Nubox.BridgeApp.WebAPI.Models
{
    public class AsistenciaUploadForm
    {
        public string EmpresaId { get; set; } = default!;
        public string PeriodoClave { get; set; } = default!;
        public IFormFile File { get; set; } = default!;
    }
}
