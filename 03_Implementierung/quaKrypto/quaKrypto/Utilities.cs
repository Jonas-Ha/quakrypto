using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;

namespace quaKrypto
{
    public static class Utilities
    {
        // Die Funktion überprüft, ob das übergebene FrameworkElement einen vertikalen Orientierungstyp aufweist.
        // Sie verwendet die VisualTreeHelper-Klasse, um das übergeordnete Panel des Elements zu ermitteln und dann zu prüfen,
        // ob es sich um ein StackPanel oder ein WrapPanel handelt.
        // Die Ausrichtung wird benötigt, um herauszufinden, wo der Adorner gezeichnet werden soll, der angibt, wo das Element abgelegt wird.
        public static bool HasVerticalOrientation(FrameworkElement itemContainer)
        {
            bool hasVerticalOrientation = true;
            if (itemContainer != null)
            {
                Panel panel = VisualTreeHelper.GetParent(itemContainer) as Panel;
                StackPanel stackPanel;
                WrapPanel wrapPanel;

                if ((stackPanel = panel as StackPanel) != null)
                {
                    hasVerticalOrientation = stackPanel.Orientation == Orientation.Vertical;
                }
                else if ((wrapPanel = panel as WrapPanel) != null)
                {
                    hasVerticalOrientation = wrapPanel.Orientation == Orientation.Vertical;
                }
            }
            return hasVerticalOrientation;
        }

        // Die Funktion fügt ein Element an einer bestimmten Position in den ItemsControl ein.
        // Sie prüft zunächst, ob der ItemsControl eine ItemsSource hat, und fügt das Element entsprechend ein, entweder
        // direkt in die Items-Liste oder in eine IList-Implementierung.
        public static void InsertItemInItemsControl(ItemsControl itemsControl, object itemToInsert, int insertionIndex)
        {
            if (itemToInsert != null)
            {
                IEnumerable itemsSource = itemsControl.ItemsSource;

                if (itemsSource == null)
                {
                    itemsControl.Items.Insert(insertionIndex, itemToInsert);
                }
                else if (itemsSource is IList)
                {
                    ((IList)itemsSource).Insert(insertionIndex, itemToInsert);
                }
                else
                {
                    Type type = itemsSource.GetType();
                    Type genericIListType = type.GetInterface("IList`1");
                    if (genericIListType != null)
                    {
                        type.GetMethod("Insert").Invoke(itemsSource, new object[] { insertionIndex, itemToInsert });
                    }
                }
            }
        }

        // Die Funktion entfernt ein Element aus dem ItemsControl. Ähnlich wie bei "InsertItemInItemsControl"
        // überprüft sie zunächst, ob der ItemsControl eine ItemsSource hat, und entfernt das Element entweder
        // direkt aus der Items-Liste oder aus einer IList-Implementierung.
        public static int RemoveItemFromItemsControl(ItemsControl itemsControl, object itemToRemove)
        {
            int indexToBeRemoved = -1;
            if (itemToRemove != null)
            {
                indexToBeRemoved = itemsControl.Items.IndexOf(itemToRemove);

                if (indexToBeRemoved != -1)
                {
                    IEnumerable itemsSource = itemsControl.ItemsSource;
                    if (itemsSource == null)
                    {
                        itemsControl.Items.RemoveAt(indexToBeRemoved);
                    }
                    else if (itemsSource is IList)
                    {
                        ((IList)itemsSource).RemoveAt(indexToBeRemoved);
                    }
                    else
                    {
                        Type type = itemsSource.GetType();
                        Type genericIListType = type.GetInterface("IList`1");
                        if (genericIListType != null)
                        {
                            type.GetMethod("RemoveAt").Invoke(itemsSource, new object[] { indexToBeRemoved });
                        }
                    }
                }
            }
            return indexToBeRemoved;
        }

        // Die Funktion überprüft, ob der angeklickte Punkt sich in der oberen Hälfte
        // (bei vertikaler Orientierung) oder linken Hälfte (bei horizontaler Orientierung) des
        // Container-FrameworkElements befindet. Die Überprüfung erfolgt anhand der übergebenen Koordinaten
        // des Punktes und der Größe des Containers.
        public static bool IsInFirstHalf(FrameworkElement container, Point clickedPoint, bool hasVerticalOrientation)
        {
            if (hasVerticalOrientation)
            {
                return clickedPoint.Y < container.ActualHeight / 2;
            }
            return clickedPoint.X < container.ActualWidth / 2;
        }

        // Die Funktion überprüft, ob die Bewegung zwischen der anfänglichen Mausposition und der aktuellen Position
        // groß genug ist, um als bedeutende Bewegung betrachtet zu werden. Die Überprüfung erfolgt anhand der
        // Differenz zwischen den X- und Y-Koordinaten der beiden Punkte und den minimalen horizontalen und
        // vertikalen Bewegungsdistanzen, die in den Systemparametern definiert sind.
        public static bool IsMovementBigEnough(Point initialMousePosition, Point currentPosition)
        {
            return Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance;
        }
    }
}
