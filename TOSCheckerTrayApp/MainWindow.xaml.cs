using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TOSCheckerTrayApp.ViewModel;

namespace TOSCheckerTrayApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).HideWindow(this);
        }

        private void Recenter(object sender, SizeChangedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).Recenter(this);
        }

        private void Recenter(object sender, EventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).Recenter(this);
        }

        private void Window_DataContextChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((TOSCheckerTrayApp.App)App.Current).MainWindow_DataContextChanged(sender, e);
        }
    }
}

