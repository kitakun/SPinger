namespace Kitakun.SPinger.Services
{
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Threading;

    using Kitakun.SPinger.Core.Models;
    using Kitakun.SPinger.Core.Services;

    /// <summary>
    /// Check every interval is server online or not
    /// </summary>
    public class PingService : IPingService
    {
        private readonly Timer _timer;
        private readonly int _timerTicks;
        private Collection<PingerTargetModel> _items;

        public event OnServerStateChanges OnServerChanged;

        public PingService(int checkEveryMinutes = 1)
        {
#if DEBUG_IN_SECCONDS
            _timerTicks = 1000 * checkEveryMinutes;
#else
            _timerTicks = 1000 * 60 * checkEveryMinutes;
#endif
            _timer = new Timer(OnCallBack, null, _timerTicks, Timeout.Infinite);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Watch(Collection<PingerTargetModel> elements)
        {
            _items = elements;
            _timer.Change(_timerTicks, Timeout.Infinite);

            if(_items.Count > 0)
            {
                // Check right now
                OnCallBack(null);
            }
        }

        private void OnCallBack(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (_items != null && _items.Count > 0)
                {
                    using (var reusableHttpClient = new HttpClient())
                    {
                        for (var i = 0; i < _items.Count; i++)
                        {
                            var record = _items[i];
                            var wasSuccess = record.IsAvailable;
                            var isDirty = false;
                            try
                            {
                                using (var request = reusableHttpClient.GetAsync(record.Address))
                                {
                                    using (var response = request.ConfigureAwait(false).GetAwaiter().GetResult())
                                    {
                                        response.EnsureSuccessStatusCode();
                                        if (response.IsSuccessStatusCode && !wasSuccess)
                                        {
                                            record.IsAvailable = true;
                                            isDirty = true;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                if (wasSuccess || state == null)
                                {
                                    record.IsAvailable = false;
                                    isDirty = true;
                                }
                            }

                            if (isDirty && OnServerChanged != null)
                            {
                                OnServerChanged.Invoke(record);
                            }
                        }
                    }
                }
            }
            finally
            {
                _timer.Change(_timerTicks, Timeout.Infinite);
            }
        }
    }
}
