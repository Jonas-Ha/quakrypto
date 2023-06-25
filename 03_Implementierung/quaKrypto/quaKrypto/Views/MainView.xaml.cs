using quaKrypto.Models.Classes;
using System.Windows;

namespace quaKrypto.Views
{
    public partial class MainView : Window
    {
        public MainView() => InitializeComponent();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetzwerkClient.BeendeSucheNachLobbys();
            NetzwerkClient.TrenneVerbindungMitUebungsszenario();
            NetzwerkHost.BeendeZyklischesSendenVonLobbyinformation();
            NetzwerkHost.BeendeTCPLobby();
        }
    }
}
