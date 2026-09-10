using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SplashBot_2.Utility
{
    internal class OperatingSystem
    {
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);

            void GetIDList(out IntPtr ppidl);

            void SetIDList(IntPtr pidl);

            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            void GetHotkey(out short pwHotkey);

            void SetHotkey(short wHotkey);

            void GetShowCmd(out int piShowCmd);

            void SetShowCmd(int iShowCmd);

            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

            void Resolve(IntPtr hwnd, int fFlags);

            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        public static void CreateStartupShortcut()
        {
            RemoveStartupShortcut();

            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetDescription("Unsplash to your desktop.");
            link.SetPath(GetPathToExe());

            // save it
            var file = (System.Runtime.InteropServices.ComTypes.IPersistFile)link;
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(GetPathToShortcut(), false);
        }

        public static void GoToSplashBot()
        {
            Process.Start("https://github.com/RSchwoerer/SplashBot");
        }

        public static bool IsEnabledAtStartup()
        {
            return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "SplashBot.lnk"));
        }

        public static void RemoveStartupShortcut()
        {
            File.Delete(GetPathToShortcut());
        }

        private static string GetPathToExe()
        {
            return Environment.ProcessPath;
        }

        private static string GetPathToShortcut()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "SplashBot.lnk"); ;
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }
    }
}