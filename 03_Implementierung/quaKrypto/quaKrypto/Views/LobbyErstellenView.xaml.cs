using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for LobbyErstellenView.xaml
    /// </summary>
    public partial class LobbyErstellenView : UserControl
    {
        public LobbyErstellenView()
        {
            InitializeComponent();
            //Thumb1.DragDelta += Thumb_DragDelta;
            //Thumb2.DragDelta += Thumb_DragDelta;
            
        }
        /*private void Thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Aktualisieren Sie die Position des ersten Daumens
            double newValue = Canvas.GetLeft(Thumb1) + e.HorizontalChange;
            Canvas.SetLeft(Thumb1, newValue);
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Aktualisieren Sie die Position des zweiten Daumens
            double newValue = Canvas.GetLeft(Thumb2) + e.HorizontalChange;
            Canvas.SetLeft(Thumb2, newValue);
        }*/
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;

            // Aktualisieren Sie die Position des Daumens
            double newValue = Canvas.GetLeft(thumb) + e.HorizontalChange;
            Canvas.SetLeft(thumb, newValue);
        }
    }
}
