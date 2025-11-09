using Nubox.BridgeApp.Application.Dto;
using OfficeOpenXml;

namespace Nubox.BridgeApp.Application.Services
{
    public sealed class ExcelUploadService
    {
        public async Task<IReadOnlyList<PartnerAsistenciaDto>> ParseAsync(Stream fileStream, CancellationToken ct)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Test");

            using var package = new ExcelPackage(fileStream);

            var ws = package.Workbook.Worksheets.FirstOrDefault() ?? throw new InvalidOperationException("El archivo Excel no contiene hojas de cálculo.");

            var dtos = new List<PartnerAsistenciaDto>();

            var row = 2;
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var trabajadorId = ws.Cells[row, 2].Text?.Trim();
                if (string.IsNullOrEmpty(trabajadorId)) break;

                var dto = new PartnerAsistenciaDto
                {
                    TrabajadorID = trabajadorId,
                    Rut = ws.Cells[row, 3].Text?.Trim(),
                    HorasTrabajadas = int.TryParse(ws.Cells[row, 4].Text?.Trim(), out var horasTrabajadas) ? horasTrabajadas : 0,
                    HorasExtras = int.TryParse(ws.Cells[row, 5].Text?.Trim(), out var horasExtras) ? horasExtras : 0,
                    Ausencias = int.TryParse(ws.Cells[row, 6].Text?.Trim(), out var ausencias) ? ausencias : 0,
                    AusenciaLicenciaMedica = int.TryParse(ws.Cells[row, 7].Text?.Trim(), out var ausenciaLicenciaMedica) ? ausenciaLicenciaMedica : 0
                };

                dtos.Add(dto);
                row++;
            }
            return dtos;
        }
        private static decimal TryDecimal(string? text) =>
            decimal.TryParse(text, out var value) ? value : 0;

        private static int TryInt(string? text) =>
            int.TryParse(text, out var value) ? value : 0;
    }
}
