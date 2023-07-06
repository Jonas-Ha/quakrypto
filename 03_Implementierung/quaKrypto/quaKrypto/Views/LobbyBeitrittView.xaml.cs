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

namespace quaKrypto.Views
{
    /// <summary>
    /// Interaction logic for LobbyBeitrittView.xaml
    /// </summary>
    public partial class LobbyBeitrittView : UserControl
    {
        //Variablen für die Bewegungen des Wikibuttons
        private readonly int zeitInMsBisWikiButtonBewegtWird = 100;
        private long zeitStempelLetztesMalWikiButtonAngeklickt = 0;
        private bool letzterClickWarNachUnten = false;
        private Point positionDesWikiButton = new(25, 25);
        public LobbyBeitrittView()
        {
            InitializeComponent();
        }
    }
}
