using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Wpf.Ui.Tray.Controls;

namespace SplashBot_2
{
    /// <summary>
    /// Override Wpf.Ui's NotifyIcon to fix hooking up the DataContext to the ContextMenu.
    /// Doing this here provides simpler integration in xaml.
    /// </summary>
    internal class NotifyIconPlus : NotifyIcon
    {
        public NotifyIconPlus()
        {
            DataContextProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(DataContextPropertyChanged));
        }

        protected override void OnMenuChanged(ContextMenu contextMenu)
        {
            base.OnMenuChanged(contextMenu);
            UpdateDataContext(contextMenu, null, DataContext);
        }

        private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                    ((NotifyIconPlus)d).OnDataContextPropertyChanged(e);

        private void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateDataContext(Menu, e.OldValue, e.NewValue);
        }

        private void UpdateDataContext(FrameworkElement target, object oldValue, object newValue)
        {
            if (target == null || BindingOperations.GetBindingExpression(target, DataContextProperty) != null) return;
            if (ReferenceEquals(this, target.DataContext) || Equals(oldValue, target.DataContext))
            {
                target.DataContext = newValue ?? this;
            }
        }
    }
}