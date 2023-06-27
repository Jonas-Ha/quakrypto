using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace quaKrypto.Services
{
    class EigenerTextBlock : TextBlock
    {
        public ObservableCollection<Inline> InlineList
        {
            get { return (ObservableCollection<Inline>)GetValue(InlineListProperty); }
            set { SetValue(InlineListProperty, value); }
        }

        public static readonly DependencyProperty InlineListProperty = DependencyProperty.Register("InlineList", typeof(ObservableCollection<Inline>), typeof(EigenerTextBlock), new UIPropertyMetadata(null, OnPropertyChanged));

        private void InlineCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    int indexOfNewItem = e.NewItems.Count - 1;
                    if (e.NewItems[indexOfNewItem] != null)
                    {
                        Application.Current.Dispatcher.Invoke(() => { Inlines.Add(e.NewItems[indexOfNewItem] as Inline); });
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                Inlines.Clear();
            }
        }

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
