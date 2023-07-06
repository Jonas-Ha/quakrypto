using quaKrypto.Models.Classes;
using System.Windows;

namespace quaKrypto.Views
{
    public partial class MainView : Window
    {
        public MainView() => InitializeComponent();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Beim Schließen der Anwendung werden alle Verbindungen getrennt und geschlossen
            NetzwerkClient.BeendeSucheNachLobbys();
            NetzwerkClient.TrenneVerbindungMitUebungsszenario();
            NetzwerkHost.BeendeZyklischesSendenVonLobbyinformation();
            NetzwerkHost.BeendeTCPLobby();
        }
    }
}
