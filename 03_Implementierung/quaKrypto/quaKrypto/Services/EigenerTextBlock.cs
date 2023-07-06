// **********************************************************
// File: EigenerTextBlock.cs
// Autor: Daniel Hannes
// erstellt am: 28.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace quaKrypto.Services
{
    //Diese Klasse wird benötigt, um im Wiki Hyperlink-Support zu realisieren.
    class EigenerTextBlock : TextBlock
    {
        //Das ist die öffentliche Property, auf welche gebunden wird. Sie wird benutzt, um einen TextBlock mit einer InlineCollection zu haben,
        //welche benutzt wird, um Hyperlinks hinzufügen zu können.
        public ObservableCollection<Inline> InlineList { get => (ObservableCollection<Inline>)GetValue(InlineListProperty); set => SetValue(InlineListProperty, value); }

        //Hier wird die Property registiert.
        public static readonly DependencyProperty InlineListProperty = DependencyProperty.Register("InlineList", typeof(ObservableCollection<Inline>), typeof(EigenerTextBlock), new UIPropertyMetadata(null, OnPropertyChanged));

        //Diese Methode wird aufgerufen, wenn sich etwas an der Collection verändert.
        private void InlineCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Wenn ein neues Inline-Element hinzugefügt wird, so wird dieses auch in die Inline-Collection hinzugefügt.
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems == null) return;
                int indexOfNewItem = e.NewItems.Count - 1;
                if (e.NewItems[indexOfNewItem] == null) return;
                Application.Current.Dispatcher.Invoke(() => { Inlines.Add(e.NewItems[indexOfNewItem] as Inline); });
            }
            //Und wenn die InlineCollection Resetet wird, so wird die ObservableCollection geleert.
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset) Inlines.Clear();
        }

        //Diese Methode wird aufgerufen, wenn sich etwas an der Property ändert.
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not EigenerTextBlock textBlock) return;
            if (e.NewValue is ObservableCollection<Inline> list)
            {
                list.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(textBlock.InlineCollectionChanged);
                textBlock.InlineCollectionChanged(sender, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
                foreach (Inline inline in list)
                {
                    textBlock.InlineCollectionChanged(sender, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, inline));
                }
            }
        }
    }
}
