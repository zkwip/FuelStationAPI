using TextScanner;

namespace FuelStationAPI
{
    internal class FuelPriceResultMapper : IScanResultMapper<FuelPriceResult>
    {
        private bool _useDecimalCommaInPrice;

        public FuelPriceResultMapper(bool useDecimalCommaPrice)
        {
            _useDecimalCommaInPrice = useDecimalCommaPrice;
        }

        public MappedScanResult<FuelPriceResult> Map(ScanResult result)
        {
            if (!result.Succes) return MappedScanResult<FuelPriceResult>.Fail(result.Message);

            try
            {
                var price = result["price"].ToDouble(_useDecimalCommaInPrice ? "fr-FR" : "en-US");
                var type = result["type"].MapWith(new FuelTypeMapper()).Result;

                return new MappedScanResult<FuelPriceResult>(new FuelPriceResult(type, price));
            }
            catch(Exception ex)
            {
                return MappedScanResult<FuelPriceResult>.Fail(ex.Message);
            }
        }
    }

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