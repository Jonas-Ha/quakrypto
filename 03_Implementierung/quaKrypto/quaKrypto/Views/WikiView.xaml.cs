using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using quaKrypto.Models.Classes;

namespace quaKrypto.Views
{
    public partial class WikiView : Window
    {
        public WikiView()
        {
            InitializeComponent();
        }

        private void WikiWirdBeendet(object sender, CancelEventArgs e)
        {
            Wiki.SpeichereAlleWikiSeiten();
        }
    }
}
