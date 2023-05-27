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
        private readonly int zeitInMsBisWikiButtonBewegtWird = 100;
        private long zeitStempelLetztesMalWikiButtonAngeklickt = 0;
        private bool letzterClickWarNachUnten = false;
        private Point positionDesWikiButton = new(25, 25);
        public LobbyBeitrittView()
        {
            InitializeComponent();
        }
        #region Wiki-Button Funktionen
        private void WikiButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            letzterClickWarNachUnten = true;
            zeitStempelLetztesMalWikiButtonAngeklickt = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }
        private void WikiButton_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            letzterClickWarNachUnten = false;
            if (new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - zeitStempelLetztesMalWikiButtonAngeklickt < zeitInMsBisWikiButtonBewegtWird && IstMausAufWikiButton(e.GetPosition(this)))
            {
                WikiView wiki = new();
                wiki.Show();
            }
        }
        private void WikiButton_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (letzterClickWarNachUnten)
            {
                Point mousePosition = e.GetPosition(this);
                if (new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - zeitStempelLetztesMalWikiButtonAngeklickt >= zeitInMsBisWikiButtonBewegtWird)
                {
                    ((Button)sender).Margin = new Thickness(mousePosition.X - 25, mousePosition.Y - 25, 0, 0);
                    positionDesWikiButton = mousePosition;
                }
                else
                {
                    if (!IstMausAufWikiButton(mousePosition)) letzterClickWarNachUnten = false;
                }
            }
        }
        private bool IstMausAufWikiButton(Point mousePosition)
        {
            double realXX = mousePosition.X - positionDesWikiButton.X; double realYY = mousePosition.Y - positionDesWikiButton.Y;
            return !(realXX >= 26 || realXX <= -26 || realYY >= 26 || realYY <= -26);
        }
        #endregion
    }
}
