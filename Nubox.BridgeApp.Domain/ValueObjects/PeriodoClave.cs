using System.Text.RegularExpressions;

namespace Nubox.BridgeApp.Domain.ValueObjects
{
    public readonly record struct PeriodoClave(string Valor)
    {
        private static readonly Regex ValidacionRegex = new(@"^(20\d{2})-(0[1-9]|1[0-2])-(M1|Q1|Q2|W[1-5])$", 
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static PeriodoClave Parse(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("PeriodoClave requerido", nameof(input));
            }

            var inputNormalizado = input.Trim().ToUpperInvariant();
            if (!ValidacionRegex.IsMatch(inputNormalizado)) throw new FormatException("Formato de clave invalido");
            return new PeriodoClave(inputNormalizado);
        }

        public static bool TryParse(string? input, out PeriodoClave value)
        {
            try { value = Parse(input!); return true; }
            catch { value = default; return false; }
        }

        public override string ToString() => Valor;
    }
}
