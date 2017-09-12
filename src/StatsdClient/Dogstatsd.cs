using System;

namespace StatsdClient
{
    public enum Status
    {
        OK = 0,
        WARNING = 1,
        CRITICAL = 2,
        UNKNOWN = 3
    }

    public static class GlobalStatsd
    {
        private static readonly object Padlock = new object();
        private static IDogStatsd _globalStatsd;

        public static IDogStatsd Instance
        {
            get
            {
                if (GlobalStatsd._globalStatsd != null)
                    return GlobalStatsd._globalStatsd;
                lock (GlobalStatsd.Padlock)
                    return GlobalStatsd._globalStatsd ?? (GlobalStatsd._globalStatsd = GlobalStatsd.CreateGlobalStatsd());
            }
        }

        private static IDogStatsd CreateGlobalStatsd()
        {
            return new DogStatsd();
        }
    }

    public class DogStatsd : IDogStatsd
    {
        private readonly DogStatsdService _dogStatsdService;
        
        public DogStatsd()
        {
            _dogStatsdService = new DogStatsdService();
        }

        public DogStatsd(StatsdConfig config) : this()
        {
            _dogStatsdService.Configure(config);
        }

        public void Configure(StatsdConfig config) => _dogStatsdService.Configure(config);

        public void Shutdown()
        {
            _dogStatsdService.Shutdown();
        }

        public void Event(string title, string text, string alertType = null, string aggregationKey = null,
            string sourceType = null, int? dateHappened = null, string priority = null, string hostname = null, string[] tags = null)
            => _dogStatsdService.Event(title: title, text: text, alertType: alertType, aggregationKey: aggregationKey, sourceType: sourceType, dateHappened: dateHappened, priority: priority, hostname: hostname, tags: tags);

        public void Counter<T>(string statName, T value, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Counter<T>(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public void Increment(string statName, int value = 1, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Increment(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public void Decrement(string statName, int value = 1, double sampleRate = 1.0, params string[] tags) => _dogStatsdService.Decrement(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public void Gauge<T>(string statName, T value, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Gauge<T>(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public void Histogram<T>(string statName, T value, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Histogram<T>(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public void Set<T>(string statName, T value, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Set<T>(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public void Timer<T>(string statName, T value, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Timer<T>(statName: statName, value: value, sampleRate: sampleRate, tags: tags);

        public IDisposable StartTimer(string name, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.StartTimer(name: name, sampleRate: sampleRate, tags: tags);

        public void Time(Action action, string statName, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Time(action: action, statName: statName, sampleRate: sampleRate, tags: tags);

        public T Time<T>(Func<T> func, string statName, double sampleRate = 1.0, string[] tags = null) => _dogStatsdService.Time<T>(func: func, statName: statName, sampleRate: sampleRate, tags: tags);

        public void ServiceCheck(string name, Status status, int? timestamp = null, string hostname = null, string[] tags = null, string message = null) => _dogStatsdService.ServiceCheck(name, status, timestamp, hostname, tags, message);
    }
}
