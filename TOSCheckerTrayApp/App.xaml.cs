using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using TOSCheckerTrayApp.ViewModel;
using System.ComponentModel;

namespace TOSCheckerTrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Forms.NotifyIcon trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var icon = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/tos-dr.ico"));
            trayIcon = TOSTrayIcon.createTrayIcon(this, icon.Stream);
        }

        public void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = ((INotifyPropertyChanged)e.NewValue);
            context.PropertyChanged += context_PropertyChanged;
        }

        void context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = ((MainWindowViewModel)this.MainWindow.DataContext);
            if (e.PropertyName == "Rating")
            {
                TOSTrayIcon.adaptRatingIcon(trayIcon, context.Rating);
            }
        }
    }
}
