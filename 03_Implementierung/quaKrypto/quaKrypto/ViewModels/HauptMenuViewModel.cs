using quaKrypto.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using quaKrypto.Commands;
using quaKrypto.Models.Interfaces;
using quaKrypto.Models.Enums;

namespace quaKrypto.ViewModels
{
    public class HauptMenuViewModel : BaseViewModel
    {
        //Lobby Beitritt Command: Zum Wechsel zum Lobby Beitritt ViewModel
        public DelegateCommand LobbyBeitritt { get; set; }
        //Lobby Erstellen Command: Zum Wechsel zum Lobby Erstellen ViewModel
        public DelegateCommand LobbyErstellen { get; set; }

        public HauptMenuViewModel(Navigator navigator, string? errorMessage = null)
        {
            //Reseten aller Einstellungen
            Wiki.Schwierigkeitsgrad = SchwierigkeitsgradEnum.Leicht;
            NetzwerkClient.ResetNetzwerkClient();
            NetzwerkHost.ResetNetzwerkHost();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Application.Current.Dispatcher.Invoke(() => { MessageBox.Show(Application.Current.MainWindow, errorMessage, "Information", MessageBoxButton.OK, MessageBoxImage.Information); });
            }

            LobbyBeitritt = new((o) =>
                {
                    navigator.aktuellesViewModel = new LobbyBeitrittViewModel(navigator);

                }, null);
            LobbyErstellen = new((o) =>
                {
                    navigator.aktuellesViewModel = new LobbyErstellenViewModel(navigator);
                }, null);
        }

    }
}
