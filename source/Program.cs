using IWshRuntimeLibrary;
using Microsoft.Win32;
using SplashBot.Services;
using SplashBot.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

// ReSharper disable LocalizableElement

/*

    Original source: https://github.com/PaitoAnderson/WallcatWindows
    Additional inspiration: https://github.com/redbaty/Unsplash.Desktop

    Images courtesy of: https://source.unsplash.com/

 */

// TODO:
// animate icon again
// Multi Monitor Support
// configuration to JSON file
// UWP?
// Set lock screen?
//      https://docs.microsoft.com/en-us/uwp/api/windows.system.userprofile.userprofilepersonalizationsettings.trysetlockscreenimageasync#Windows_System_UserProfile_UserProfilePersonalizationSettings_TrySetLockScreenImageAsync_Windows_Storage_StorageFile_
// ...

namespace SplashBot
{
    public class MyCustomApplicationContext : ApplicationContext
    {
        private readonly ContextMenu _contextMenu;
        private readonly UnsplashService _UnsplashService;
        private System.Threading.Timer _timer;
        private NotifyIcon _trayIcon;

        public MyCustomApplicationContext()
        {
            Application.ApplicationExit += OnApplicationExit;
            SystemEvents.PowerModeChanged += OnPowerChange;
            SystemEvents.SessionSwitch += OnSessionSwitch;

            _UnsplashService = new UnsplashService();
            _contextMenu = new ContextMenu();
            _trayIcon = new NotifyIcon
            {
                Icon = Resources.logo,
                ContextMenu = _contextMenu,
                Visible = true
            };

            using (var iconAnimation = new IconAnimation(ref _trayIcon))
            {
                _contextMenu.MenuItems.AddRange(
                                                new[]
                                                {
                                                    new MenuItem("Unsplash.com", (sender, args) => GotoUnsplash()),
                                                    new MenuItem("-") {Enabled = false},
                                                    new MenuItem("Random", SelectChannel)
                                                    {
                                                        Tag = ChannelFactory.RandomChannel(),
                                                        Checked = false
                                                    },
                                                    new MenuItem("-") { Enabled = false },
                                                    new MenuItem("Collections") { Enabled = false }
                                                });

                _contextMenu.MenuItems.AddRange(
                                                ChannelFactory.CollectionChannels()
                                                              .Select(
                                                                      c => new MenuItem(c.Name, SelectChannel)
                                                                      { Tag = c, Checked = false })
                                                              .ToArray());

                _contextMenu.MenuItems.AddRange(new[]
                                                {
                                                    new MenuItem("-") { Enabled = false },
                                                    new MenuItem("Searches") { Enabled = false }
                                                });

                _contextMenu.MenuItems.AddRange(
                                                ChannelFactory.SearchChannels()
                                                    .Select(c => new MenuItem(c.Name, SelectChannel) { Tag = c, Checked = false })
                                                    .ToArray());

                _contextMenu.MenuItems.AddRange(
                                                new[]
                                                {
                                                    new MenuItem("-") {Enabled = false},
                                                    new MenuItem("Start at login", (sender, args) => CreateStartupShortcut())
                                                    {Checked = IsEnabledAtStartup()},
                                                    new MenuItem("-") {Enabled = false},
                                                    new MenuItem("Quit Splashbot", (sender, args) => Application.Exit())
                                                });

                if (Properties.Settings.Default.CurrentChannel != null)
                {
                    UpdateMenuSelection(Properties.Settings.Default.CurrentChannel);
                }

                // Onboarding
                if (Properties.Settings.Default.CurrentChannel == null)
                {
                    var channel = ChannelFactory.CollectionChannels().FirstOrDefault();
                    if (channel != null)
                    {
                        Properties.Settings.Default.CurrentChannel = channel;
                        Properties.Settings.Default.Save();
                    }
                }

                UpdateWallpaper();
                MidnightUpdate();
            }
        }

        private static bool IsEnabledAtStartup()
        {
            return System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "SplashBot.lnk"));
        }

        private void CreateStartupShortcut()
        {
            string pathToExe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string pathToShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "SplashBot.lnk");

            if (IsEnabledAtStartup())
            {
                System.IO.File.Delete(pathToShortcut);
            }
            else
            {
                var shortcut = (IWshShortcut)new WshShell().CreateShortcut(pathToShortcut);

                shortcut.Description = "Unsplash to your desktop.";
                shortcut.TargetPath = pathToExe;
                shortcut.Save();
            }

            foreach (MenuItem menuItem in _contextMenu.MenuItems)
            {
                if (menuItem.Text == "Start at login")
                {
                    menuItem.Checked = IsEnabledAtStartup();
                    break;
                }
            }
        }

        private void GotoUnsplash()
        {
            Process.Start("https://unsplash.com/");
        }

        private void MidnightUpdate()
        {
            var updateTime = new TimeSpan(24, 1, 0) - DateTime.Now.TimeOfDay;
            _timer = new System.Threading.Timer(
                                                x =>
                                                {
                                                    UpdateWallpaper();
                                                    MidnightUpdate();
                                                },
                                                null,
                                                updateTime,
                                                Timeout.InfiniteTimeSpan);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            SystemEvents.PowerModeChanged -= OnPowerChange;
            SystemEvents.SessionSwitch -= OnSessionSwitch;

            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;
        }

        private void OnPowerChange(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    ResumeSession();
                    break;

                case PowerModes.Suspend:
                    SuspendSession();
                    break;
            }
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    // Going into lock/standby screen
                    SuspendSession();
                    break;

                case SessionSwitchReason.SessionUnlock:
                    // Back from lock/standby
                    ResumeSession();
                    break;
            }
        }

        private void ResumeSession()
        {
            Retry.Do(UpdateWallpaper, TimeSpan.FromSeconds(15));
            MidnightUpdate();
        }

        private async void SelectChannel(object sender, EventArgs e)
        {
            using (var iconAnimation = new IconAnimation(ref _trayIcon))
            {
                var channel = (UnsplashChannel)((MenuItem)sender).Tag;

                // Update Settings
                Properties.Settings.Default.CurrentChannel = channel;
                Properties.Settings.Default.LastChecked = DateTime.Now.Date;
                Properties.Settings.Default.Save();
                UpdateMenuSelection(channel);

                var wallpaper = await _UnsplashService.GetWallpaper(channel);
                var filePath = await DownloadFile.Get(wallpaper.Url);

                if (Environment.OSVersion.Version.Major >= 8)
                {
                    // Windows 10
                    SetWallpaper.Apply(null, filePath, DesktopWallpaperPosition.Fill);
                }
                else
                {
                    SetWallpaperLegacy.Apply(filePath, DesktopWallpaperPosition.Fill);
                }
            }
        }

        private void SuspendSession()
        {
            _timer.Dispose();
        }

        private void UpdateMenuSelection(UnsplashChannel channel)
        {
            Console.WriteLine("UpdateMneuSelection");
            for (var i = _contextMenu.MenuItems.Count - 1; i >= 0; i--)
            {
                // Update Checkmark
                if (_contextMenu.MenuItems[i].Tag is UnsplashChannel c)
                {
                    _contextMenu.MenuItems[i].Checked = c.Name == channel.Name;
                }
            }
        }

        private void UpdateWallpaper()
        {
            if (Properties.Settings.Default.LastChecked != DateTime.Now.Date)
            {
                var channel = Properties.Settings.Default.CurrentChannel;
                if (channel != null)
                    SelectChannel(new MenuItem { Tag = channel }, null);
            }
        }
    }

    internal static class Program
    {
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Thread Exception!");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception)?.ToString(), "Unhandled Exception!");
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Debug.WriteLine($@"Storage: {System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath}");
            Debug.WriteLine($@"Current Channel: {Properties.Settings.Default.CurrentChannel}");
            Debug.WriteLine($@"Last Checked: {Properties.Settings.Default.LastChecked}");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new MyCustomApplicationContext());
        }
    }
}