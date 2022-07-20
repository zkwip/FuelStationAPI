using TextScanner;

namespace FuelStationAPI
{
    internal class FuelPriceResultMapper : IScanResultMapper<FuelPriceResult>
    {

        public MappedScanResult<FuelPriceResult> Map(ScanResult result)
        {
            if (!result.Succes) return MappedScanResult<FuelPriceResult>.Fail(result.Message);

            var price = result["price"].ToDouble();
            var type = result["type"].MapWith(new FuelTypeMapper()).Result;

            return new MappedScanResult<FuelPriceResult>(new FuelPriceResult(type, price));
        }
    }

    internal class FuelTypeMapper : ITextSpanMapper<FuelType>
    {
        public MappedScanResult<FuelType> Map(ManagedTextSpan text)
        {
            string t = text.ToString();

            if (t.Contains("95")) 
                return new(FuelType.Euro95_E10);

            if (t.Contains("98")) 
                return new(FuelType.Euro98_E5);

            if (t.Contains("Diesel"))
                return new(FuelType.Diesel);

            if (t.Contains("CNG"))
                return new(FuelType.CNG);

            if (t.Contains("LPG"))
                return new(FuelType.LPG);

            return MappedScanResult<FuelType>.Fail($"unrecognised type: {t}");
        }
    }
}