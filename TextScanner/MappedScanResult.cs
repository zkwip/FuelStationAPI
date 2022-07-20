namespace TextScanner
{
    public class MappedScanResult<T>
    {
        private readonly T? _resultData;
        public readonly Boolean Succes;
        public readonly string Message;

        public MappedScanResult(T resultData)
        {
            _resultData = resultData;
            Succes = true;
            Message = "Succes";
        }

        private MappedScanResult(bool succes, string msg)
        {
            Succes = succes;
            Message = msg;
        }

        public static MappedScanResult<T> Fail(string msg) => new MappedScanResult<T>(false, msg);

        public T Result => _resultData!;
    }
}