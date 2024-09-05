using SplashBot_2.Utility;

namespace SplashBot_2.Service
{
    internal class ScheduleService
    {
        private Timer _timer;
        private Action updateWallpaper;

        public void Resume()
        {
            ArgumentNullException.ThrowIfNull(updateWallpaper);

            Retry.Do(updateWallpaper, TimeSpan.FromSeconds(15));
            MidnightUpdate();
        }

        public void Start(Action updateWallpaper)
        {
            this.updateWallpaper = updateWallpaper;

            // TODO : do not set at start. let main handle initial loading of image.
            Retry.Do(updateWallpaper, TimeSpan.FromSeconds(15));
            MidnightUpdate();
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        private void MidnightUpdate()
        {
            var updateTime = new TimeSpan(24, 1, 0) - DateTime.Now.TimeOfDay;
            _timer = new Timer(
                                                x =>
                                                {
                                                    updateWallpaper();
                                                    MidnightUpdate();
                                                },
                                                null,
                                                updateTime,
                                                Timeout.InfiniteTimeSpan);
        }
    }
}