using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using quaKrypto.Models.Classes;
using Xceed.Wpf.AvalonDock.Controls;
using System.Windows.Threading;

namespace quaKrypto.Views
{
    public partial class WikiButton : UserControl
    {
        //Wiki-Button Zeug Anfang
        private readonly int zeitInMsBisWikiButtonBewegtWird = 100;
        private long zeitStempelLetztesMalWikiButtonAngeklickt = 0;
        private bool letzterClickWarNachUnten = false;
        private Point positionDesWikiButton = new(25, 25);
        private Button myButton = new();
        //Wiki-Button Zeug Ende

        public WikiButton()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
            {
                ((Window)Parent.FindVisualTreeRoot()).SizeChanged += WikiButton_SizeChanged;
            });
        }

        private void WikiButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (positionDesWikiButton.X == 25 && positionDesWikiButton.Y == 25) return;
            myButton.Margin = new Thickness(0, 0, 0, 0);
            positionDesWikiButton = new(25, 25);
        }

        //Das sind die Wiki-Button Funktionen. Minimierbar mit dem Minuszeichen vor #region
        #region Wiki-Button Funktionen
        private void WikiButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            letzterClickWarNachUnten = true;
            zeitStempelLetztesMalWikiButtonAngeklickt = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }
        private void WikiButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (letzterClickWarNachUnten == false) return;
            letzterClickWarNachUnten = false;
            if (new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - zeitStempelLetztesMalWikiButtonAngeklickt < zeitInMsBisWikiButtonBewegtWird)
            {
                if (!Wiki.WikiIstOffen)
                {
                    new WikiView().Show();
                    Wiki.WikiIstOffen = true;
                }
            }
        }
        private void WikiButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (letzterClickWarNachUnten)
            {
                Point mousePosition = e.GetPosition(this);
                Point neuerPunkt = new(Math.Min(Math.Max(mousePosition.X - 25, 0), ((Grid)Parent).ActualWidth - 50), Math.Min(Math.Max(mousePosition.Y - 25, 0), ((Grid)Parent).ActualHeight - 50));
                if (new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - zeitStempelLetztesMalWikiButtonAngeklickt >= zeitInMsBisWikiButtonBewegtWird)
                {
                    myButton = (Button)sender;
                    myButton.Margin = new Thickness(neuerPunkt.X, neuerPunkt.Y, 0, 0);
                    positionDesWikiButton = neuerPunkt;
                }
                else
                {
                    if (!IstMausAufWikiButton(neuerPunkt)) letzterClickWarNachUnten = false;
                }
            }
        }
        private bool IstMausAufWikiButton(Point mousePosition)
        {
            double deltaX = mousePosition.X - positionDesWikiButton.X; double deltaY = mousePosition.Y - positionDesWikiButton.Y;
            return !(Math.Abs(deltaX) >= 26 || Math.Abs(deltaY) >= 26);
        }
        #endregion
    }
}
