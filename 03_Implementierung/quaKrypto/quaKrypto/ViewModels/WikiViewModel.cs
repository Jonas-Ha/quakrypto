using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using System.Collections.ObjectModel;
using System.Windows;

namespace quaKrypto.ViewModels
{
    public class WikiViewModel : BaseViewModel
    {
        public static ObservableCollection<WikiSeite> WikiSeiten => Wiki.WikiSeiten;
        public WikiSeite SelektierteWikiSeite => Wiki.SelektierteWikiSeite;
        private bool editierModus = false;
        public bool EditierModus { get { return editierModus; } set { editierModus = value; EigenschaftWurdeGeändert(nameof(Cursor)); EigenschaftWurdeGeändert(nameof(EditierModus)); EigenschaftWurdeGeändert(nameof(LabelSichtbar)); EigenschaftWurdeGeändert(nameof(TextBoxSichtbar)); } }
        public Visibility LabelSichtbar => EditierModus ? Visibility.Collapsed : Visibility.Visible;
        public Visibility TextBoxSichtbar => !EditierModus ? Visibility.Collapsed : Visibility.Visible;

        public string Cursor => EditierModus ? "No" : "Hand";

        public DelegateCommand SeitenErweitern { get; set; }
        public DelegateCommand SeiteBearbeiten { get; set; }
        public DelegateCommand SeiteEntfernen { get; set; }
        public DelegateCommand SeiteSelektiert { get; set; }

        public WikiViewModel()
        {
            SeitenErweitern = new((o) =>
            {
                Wiki.SeitenErweitern();
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => !EditierModus);

            SeiteEntfernen = new((o) =>
            {
                Wiki.SeiteEntfernen();
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => !EditierModus);

            SeiteSelektiert = new((o) =>
            {
                Wiki.SeiteSelektieren(o.ToString() ?? "0");
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => !EditierModus);

            SeiteBearbeiten = new((o) =>
            {
                EditierModus = !EditierModus;
                foreach (WikiSeite wikiSeite in WikiSeiten)
                {
                    wikiSeite.SetzeEditierModus(EditierModus);
                }
                EigenschaftWurdeGeändert(nameof(SelektierteWikiSeite));
            }, (o) => true);
        }
    }
}
