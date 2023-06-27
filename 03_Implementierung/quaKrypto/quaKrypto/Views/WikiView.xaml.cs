using System.ComponentModel;
using System.Windows;
using quaKrypto.Models.Classes;

namespace quaKrypto.Views
{
    public partial class WikiView : Window
    {
        public WikiView() { InitializeComponent(); Wiki.SelektiereDieErsteSeite(); }

        private void WikiWirdBeendet(object sender, CancelEventArgs e)
        {
            Wiki.SpeichereBenutzerWikiSeiten();
            Wiki.WikiIstOffen = false;
        }
    }
}
