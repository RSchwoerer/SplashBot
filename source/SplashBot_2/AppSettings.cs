using SplashBot_2.Service;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SplashBot_2
{
    internal class AppSettings
    {
        private readonly DataService ds;
        private string searchText;

        public AppSettings(DataService ds)
        {
            this.ds = ds;
            ds.InitializeAppSettings(this);
        }

        public string Foo { get; set; }

        public string SearchText
        {
            get => searchText;
            set
            {
                _ = SetProperty(ref searchText, value);
            }
        }

        protected bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            ds.UpdateAppSetting(propertyName, newValue as string);
            field = newValue;
            return true;
        }
    }
}