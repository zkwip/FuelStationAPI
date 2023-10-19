using FuelStationAPI.Domain;
using TextScanner;

namespace FuelStationAPI.Mappers
{
    internal class FuelStationIdentifierMapper : IScanResultMapper<Station>
    {
        private readonly string _providerName;
        private readonly string _namePrefix;

        public FuelStationIdentifierMapper(string providerName, string namePrefix)
        {
            _providerName = providerName;
            _namePrefix = namePrefix;
        }

        public MappedScanResult<Station> Map(ScanResult result)
        {
            if (!result.Succes)
                return MappedScanResult<Station>.Fail(result.Message);

            try
            {
                var lat = result["lat"].ToDouble();
                var lng = result["lng"].ToDouble();
                var name = result["name"].ToString();
                var identifier = result["identifier"].ToString();

                return new(new Station(_providerName, identifier, _namePrefix + name, new Geolocation(lat, lng)));
            }
            catch (Exception ex)
            {
                return MappedScanResult<Station>.Fail(ex.Message);
            }
        }
    }
}