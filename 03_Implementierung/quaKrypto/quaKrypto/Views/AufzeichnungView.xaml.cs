﻿using System;
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
    /// Interaction logic for AufzeichnungView.xaml
    /// </summary>
    public partial class AufzeichnungView : UserControl
    {
        public AufzeichnungView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dg.SelectedIndex = -1;

        }
    }
}
