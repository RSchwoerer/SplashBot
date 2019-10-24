using System;
using System.Windows.Forms;

namespace SplashBot.Util
{
    public class IconAnimation : IDisposable
    {
        private const int Frames = 1;
        private readonly NotifyIcon _notifyIcon;
        private readonly Timer _timer = new Timer { Interval = 500 };
        private int _currentIteration;

        public IconAnimation(ref NotifyIcon notifyIcon)
        {
            //_notifyIcon = notifyIcon;

            //// Timer Event
            //_timer.Tick += (sender, args) =>
            //{
            //    _currentIteration++;
            //    if (_currentIteration > Frames) _currentIteration = 0;
            //    _notifyIcon.Icon = (_currentIteration == 0) ? Resources.AppIcon : Resources.AppIconAlt;
            //};

            //// Start
            //_notifyIcon.Icon = Resources.AppIconAlt;
            //_currentIteration = 0;
            //_timer.Start();
        }

        public void Dispose()
        {
            //// Stop
            //if (_timer.Enabled)
            //{
            //    _timer.Stop();
            //    _notifyIcon.Icon = Resources.AppIcon;
            //}
        }
    }
}