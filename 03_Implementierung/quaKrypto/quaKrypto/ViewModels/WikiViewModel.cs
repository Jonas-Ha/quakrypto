using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace quaKrypto.ViewModels
{
    public class WikiViewModel : BaseViewModel
    {
        public static ObservableCollection<WikiSeite> WikiSeiten { get { return Wiki.WikiSeiten; } }
        public WikiSeite SelektierteWikiSeite { get { return Wiki.SelektierteWikiSeite; } }
        private bool editierModus = false;
        public bool EditierModus { get { return editierModus; } set { editierModus = value; EigenschaftWurdeGeändert(nameof(EditierModus)); EigenschaftWurdeGeändert(nameof(LabelSichtbar)); EigenschaftWurdeGeändert(nameof(TextBoxSichtbar)); } }
        public Visibility LabelSichtbar { get { return EditierModus ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility TextBoxSichtbar { get { return !EditierModus ? Visibility.Collapsed : Visibility.Visible; } }

        public DelegateCommand SeitenErweitern { get; set; }
        public DelegateCommand SeiteBearbeiten { get; set; }
        public DelegateCommand SeiteEntfernen { get; set; }
        public DelegateCommand SeiteSelektiert { get; set; }


        public WikiViewModel()
        {
            SeitenErweitern = new((o) =>
            {
                Wiki.SeitenErweitern();
                EigenschaftWurdeGeändert(nameof(ViewModels.WikiViewModel.SelektierteWikiSeite));
            }, (o) => !EditierModus);

            SeiteBearbeiten = new((o) =>
            {
                EditierModus = !EditierModus;
                foreach (WikiSeite wikiSeite in WikiSeiten)
                {
                    wikiSeite.SetzeEditierModus(EditierModus);
                }
                EigenschaftWurdeGeändert(nameof(ViewModels.WikiViewModel.SelektierteWikiSeite));
            }, (o) => true);

            SeiteEntfernen = new((o) =>
            {
                Wiki.SeiteEntfernen();
                EigenschaftWurdeGeändert(nameof(ViewModels.WikiViewModel.SelektierteWikiSeite));
            }, (o) => !EditierModus);

            SeiteSelektiert = new((o) =>
            {
                Wiki.SeiteSelektieren(o.ToString() ?? "0");
                EigenschaftWurdeGeändert(nameof(ViewModels.WikiViewModel.SelektierteWikiSeite));
            }, (o) => !EditierModus);
        }
    }
}
