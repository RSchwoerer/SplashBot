using SplashBot_2.Utility;

namespace SplashBot_2
{
    internal class ScheduleService
    {
        private readonly Action updateWallpaper;
        private System.Threading.Timer _timer;

        public ScheduleService(Action updateWallpaper)
        {
            this.updateWallpaper = updateWallpaper;
        }

        public void Resume()
        {
            Retry.Do(updateWallpaper, TimeSpan.FromSeconds(15));
            MidnightUpdate();
        }

        public void Start()
        {
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
            _timer = new System.Threading.Timer(
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