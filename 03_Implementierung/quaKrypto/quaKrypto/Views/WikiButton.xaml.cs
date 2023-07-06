// **********************************************************
// File: WikiButton.xaml.cs
// Autor: Daniel Hannes
// erstellt am: 18.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using quaKrypto.Models.Classes;
using Xceed.Wpf.AvalonDock.Controls;
using System.Windows.Threading;

namespace quaKrypto.Views
{
    //Diese Klasse repräsentiert den Button des Wikis, welcher verschiebbar ist.
    public partial class WikiButton
    {
        //Hier werden die wichtigen Variablen für den Wiki-Button angelegt.
        private readonly int zeitInMsBisWikiButtonBewegtWird = 250;
        private long zeitStempelLetztesMalWikiButtonAngeklickt = 0;
        private bool letzterClickWarNachUnten = false;
        private Point positionDesWikiButton = new(25, 25);
        private Button myButton = new();

        //Beim Erstellen des Wiki-Buttons wird zuerst die Komponente initialisiert und eine Funktion wird hinzugefügt, wenn das Fenster fertig initialisiert ist.
        public WikiButton() { InitializeComponent(); Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () => ((Window)Parent.FindVisualTreeRoot()).SizeChanged += WikiButton_SizeChanged); }

        //Das ist die Methode, welche aufgerufen wird, wenn sich die Größe des Fensters verändert wird. Sie setzt den Wiki-Button wieder in die obere, linke Ecke.
        private void WikiButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (positionDesWikiButton.X == 25 && positionDesWikiButton.Y == 25) return;
            myButton.Margin = new Thickness(0, 0, 0, 0);
            positionDesWikiButton = new(25, 25);
        }

        //Das sind die Wiki-Button Funktionen. Minimierbar mit dem Minuszeichen vor #region
        #region Wiki-Button Funktionen

        //Diese Methode wird aufgerufen, wenn mit der linken Maustaste auf den Wiki-Button gedrückt wird. Es wird sich die Zeit gespeichert.
        private void WikiButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            letzterClickWarNachUnten = true;
            zeitStempelLetztesMalWikiButtonAngeklickt = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }

        //Diese Methode wird aufgerufen, wenn die linke Maustaste wieder losgelassen wird.
        private void WikiButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (letzterClickWarNachUnten == false) return;
            letzterClickWarNachUnten = false;
            //Hier wird die Zeit seit dem letzen Klick nach unten überprüft und wenn es unter dem Limit ist, so wird ein Linksklick ausgeführt.
            if (new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - zeitStempelLetztesMalWikiButtonAngeklickt < zeitInMsBisWikiButtonBewegtWird)
            {
                if (Wiki.WikiIstOffen) return;
                new WikiView().Show();
                Wiki.WikiIstOffen = true;
            }
        }

        //Diese Methode wird aufgerufen, wenn die Maus über den Wiki-Button bewegt wird. Hier wird überprüft, ob die Zeit seit dem letzen Klick nach unten lang genug war
        //und wenn dies der Fall ist, so wird der Knopf bewegt.
        private void WikiButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!letzterClickWarNachUnten) return;
            Point mousePosition = e.GetPosition(this);
            Point neuerPunkt = new(Math.Min(Math.Max(mousePosition.X - 25, 0), ((Grid)Parent).ActualWidth - 50), Math.Min(Math.Max(mousePosition.Y - 25, 0), ((Grid)Parent).ActualHeight - 50));
            if (new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - zeitStempelLetztesMalWikiButtonAngeklickt >= zeitInMsBisWikiButtonBewegtWird)
            {
                //Hier wird die Position des Wiki-Buttons geändert, indem der Margin verändert wird.
                myButton = (Button)sender;
                myButton.Margin = new Thickness(neuerPunkt.X, neuerPunkt.Y, 0, 0);
                positionDesWikiButton = neuerPunkt;
            }
            //Hier wurde die Maus vom Wiki-Button weg bewegt.
            else if (!IstMausAufWikiButton(neuerPunkt)) letzterClickWarNachUnten = false;
        }

        //Das ist eine Hilfsfunktion, welche überprüft, ob sich der Mauszeiger auf dem Wiki-Button befindet.
        private bool IstMausAufWikiButton(Point mousePosition)
        {
            double deltaX = mousePosition.X - positionDesWikiButton.X; double deltaY = mousePosition.Y - positionDesWikiButton.Y;
            return !(Math.Abs(deltaX) >= 26 || Math.Abs(deltaY) >= 26);
        }
        #endregion
    }
}
