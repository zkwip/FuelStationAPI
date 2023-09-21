using FuelStationAPI.Domain;
using TextScanner;

namespace FuelStationAPI.Mappers
{
    internal class FuelTypeMapper : ITextSpanMapper<FuelType>
    {
        public MappedScanResult<FuelType> Map(ManagedTextSpan text)
        {
            string t = text.ToString();

            if (t.Contains("95") && t.Contains("E5"))
                return new(FuelType.Euro95_E5);

            if (t.Contains("95"))
                return new(FuelType.Euro95_E10);

            if (t.Contains("98"))
                return new(FuelType.Euro98_E5);

            if (t.Contains("Diesel"))
                return new(FuelType.Diesel);

            if (t.Contains("AdBlue"))
                return new(FuelType.AdBlue);

            if (t.Contains("CNG"))
                return new(FuelType.CNG);

            if (t.Contains("LPG"))
                return new(FuelType.LPG);

            return MappedScanResult<FuelType>.Fail($"unrecognised type: {t}");
        }
    }
}