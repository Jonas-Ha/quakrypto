using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using quaKrypto.ViewModels;
using quaKrypto.Views;

namespace quaKrypto
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Navigator navigator = new Navigator();

            navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);
            MainWindow = new MainView()
            {
                DataContext = new MainViewModel(navigator)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
