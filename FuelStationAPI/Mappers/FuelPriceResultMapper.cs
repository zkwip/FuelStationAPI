using FuelStationAPI.Domain;
using TextScanner;

namespace FuelStationAPI.Mappers
{
    internal class FuelPriceResultMapper : IScanResultMapper<FuelPriceResult>
    {
        private readonly bool _useDecimalCommaInPrice;

        public FuelPriceResultMapper(bool useDecimalCommaPrice)
        {
            _useDecimalCommaInPrice = useDecimalCommaPrice;
        }

        public MappedScanResult<FuelPriceResult> Map(ScanResult result)
        {
            if (!result.Succes)
                return MappedScanResult<FuelPriceResult>.Fail(result.Message);

            try
            {
                var price = result["price"].ToDouble(_useDecimalCommaInPrice ? "fr-FR" : "en-US");
                var type = result["type"].MapWith(new FuelTypeMapper()).Result;

                return new MappedScanResult<FuelPriceResult>(new FuelPriceResult(type, price));
            }
            catch (Exception ex)
            {
                return MappedScanResult<FuelPriceResult>.Fail(ex.Message);
            }
        }
    }
}