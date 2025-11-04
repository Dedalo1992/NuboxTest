namespace Nubox.BridgeApp.Domain.Services
{
    public static class CalculadorAsistencia
    {
        public static int TotalHorasRemunerables(int horasTrabajadas, int horasExtras)
        {
            if(horasTrabajadas < 0 || horasExtras < 0)
            {
                throw new ArgumentOutOfRangeException("Las horas no pueden ser negativas");
            }
            return horasTrabajadas + horasExtras;
        }
    }
}
