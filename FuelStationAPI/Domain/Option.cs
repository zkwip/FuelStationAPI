using TextScanner;

namespace FuelStationAPI.Domain
{
    public class Option<T> where T : class
    {
        private T? _value = null;

        public bool Succes => _value is not null;

        public static Option<T> Some(T value) => new() { _value = value };
        public static Option<T> None() => new();
        public Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class =>
            _value is null ? Option<TResult>.None() : Option<TResult>.Some(map(_value));

        public T Reduce(T def) => _value is null ? def : _value;

        public static implicit operator Option<T>(MappedScanResult<T> msr) => msr.Succes ? Some(msr.Result) : None();

        public static implicit operator Option<T>(T value) => Some(value);
    }
}
