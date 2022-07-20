using TextScanner;

namespace FuelStationAPI
{
    internal class FuelStationIdentifierMapper : IScanResultMapper<FuelStationIdentifier>
    {
        private readonly string _providerName;
        private readonly string _namePrefix;

        public FuelStationIdentifierMapper(string providerName, string namePrefix)
        {
            _providerName = providerName;
            _namePrefix = namePrefix;
        }

        public MappedScanResult<FuelStationIdentifier> Map(ScanResult result)
        {
            if (!result.Succes) 
                return MappedScanResult<FuelStationIdentifier>.Fail(result.Message);

            try
            {
                var lat = result["lat"].ToDouble();
                var lng = result["lng"].ToDouble();
                var name = result["name"].ToString();
                var identifier = result["identifier"].ToString();

                return new(new FuelStationIdentifier(_providerName, identifier, _namePrefix + name, new Geolocation(lat, lng)));
            }
            catch (Exception ex)
            {
                return MappedScanResult<FuelStationIdentifier>.Fail(ex.Message);
            }
        }
    }
}