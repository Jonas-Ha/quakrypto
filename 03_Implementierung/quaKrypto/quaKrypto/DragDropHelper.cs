using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Documents;
using System.Reflection;
using System.Diagnostics;
using Xceed.Wpf.Toolkit;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;

namespace quaKrypto
{
    // Diese Klasse implementiert eine Hilfsklasse "DragDropHelper",
    // die Funktionen zum Unterstützen von Drag & Drop-Vorgängen in WPF-Anwendungen bereitstellt
    public class DragDropHelper
    {
        // Quelle und Ziel des Drag & Drop Elements
        private DataFormat format = DataFormats.GetDataFormat("DragDropItemsControl");
        private Point initialMousePosition;
        private Vector initialMouseOffset;
        private object draggedData;
        private DraggedAdorner draggedAdorner;
        private InsertionAdorner insertionAdorner;
        private Window topWindow;
        // Quelle des Drag & Drop Elements
        private ItemsControl sourceItemsControl;
        private FrameworkElement sourceItemContainer;
        // Ziel des Drag & Drop Elements
        private ItemsControl targetItemsControl;
        private FrameworkElement targetItemContainer;
        private bool hasVerticalOrientation;
        private int insertionIndex;
        private bool isInFirstHalf;
        // singleton-Patten
        private static DragDropHelper instance;
        private static DragDropHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DragDropHelper();
                }
                return instance;
            }
        }



        // Festlegen und Abrufen einer DataTemplate-Eigenschaft für ein DependencyObject
        public static bool GetIsDragSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragSourceProperty, value);
        }

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));


        public static bool GetIsDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDropTargetProperty);
        }

        public static void SetIsDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDropTargetProperty, value);
        }

        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDropTargetChanged));

        public static DataTemplate GetDragDropTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DragDropTemplateProperty);
        }

        public static void SetDragDropTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DragDropTemplateProperty, value);
        }

        public static readonly DependencyProperty DragDropTemplateProperty =
            DependencyProperty.RegisterAttached("DragDropTemplate", typeof(DataTemplate), typeof(DragDropHelper), new UIPropertyMetadata(null));



        // Diese Methode wird aufgerufen, wenn sich der Wert der angefügten Eigenschaft "IsDragSource" an einem ItemsControl ändert
        // Sie fügt dem ItemsControl entsprechende Event-Handler für die Vorschauereignisse "PreviewMouseLeftButtonDown",
        // "PreviewMouseLeftButtonUp" und "PreviewMouseMove" hinzu oder entfernt sie, basierend auf dem neuen Wert der Eigenschaft
        private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dragSource = obj as ItemsControl;
            if (dragSource != null)
            {
                if (Equals(e.NewValue, true))
                {
                    dragSource.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;
                }
                else
                {
                    dragSource.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
                }
            }
        }

        // Diese Methode wird aufgerufen, wenn sich der Wert der angefügten Eigenschaft "IsDropTarget" an einem ItemsControl ändert.
        // Sie aktiviert bzw. deaktiviert die "AllowDrop"-Eigenschaft des ItemsControl, basierend auf dem neuen Wert der Eigenschaft.
        // Zusätzlich werden entsprechende Event-Handler für die Vorschauereignisse "PreviewDrop", "PreviewDragEnter",
        // "PreviewDragOver" und "PreviewDragLeave" hinzugefügt oder entfernt, um das Ablegen von Elementen auf dem Drop-Ziel zu ermöglichen bzw.zu deaktivieren.
        private static void IsDropTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dropTarget = obj as ItemsControl;
            if (dropTarget != null)
            {
                if (Equals(e.NewValue, true))
                {
                    dropTarget.AllowDrop = true;
                    dropTarget.PreviewDrop += Instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter += Instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver += Instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave += Instance.DropTarget_PreviewDragLeave;
                }
                else
                {
                    dropTarget.AllowDrop = false;
                    dropTarget.PreviewDrop -= Instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter -= Instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver -= Instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave -= Instance.DropTarget_PreviewDragLeave;
                }
            }
        }


        // Diese Methode behandelt das Ereignis "PreviewMouseLeftButtonDown" für eine Drag-Quelle (ItemsControl)
        // Sie speichert das Quell-ItemsControl und die ursprüngliche Mausposition.
        // Dann wird das angeklickte Element anhand des Visual-Elements,
        // das das Ereignis ausgelöst hat, identifiziert und das entsprechende Datenobjekt gespeichert.
        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sourceItemsControl = (ItemsControl)sender;
            Visual visual = e.OriginalSource as Visual;

            if (visual == null)
            {
                Debug.WriteLine("Visual is null.");
                return;
            }

            topWindow = Window.GetWindow(sourceItemsControl);
            initialMousePosition = e.GetPosition(topWindow);

            sourceItemContainer = sourceItemsControl.ContainerFromElement(visual) as FrameworkElement;
            if (sourceItemContainer != null)
            {
                draggedData = sourceItemContainer.DataContext;
            }
        }

        // Diese Methode behandelt das Ereignis "PreviewMouseMove" für eine Drag-Quelle (ItemsControl).
        // Wenn ein Element gezogen wird, wird überprüft, ob die Mausbewegung eine ausreichende Entfernung erreicht hat, um den Drag & Drop-Vorgang zu starten.
        // Wenn dies der Fall ist, wird das draggedData-Objekt in ein DataObject verpackt
        // und die Methode DoDragDrop wird aufgerufen, um den Drag & Drop-Vorgang zu initiieren.
        // Während des Vorgangs werden auch verschiedene Event-Handler auf dem Fenster hinzugefügt
        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (draggedData != null)
            {
                if (Utilities.IsMovementBigEnough(initialMousePosition, e.GetPosition(topWindow)))
                {
                    initialMouseOffset = initialMousePosition - sourceItemContainer.TranslatePoint(new Point(0, 0), topWindow);

                    DataObject data = new DataObject(format.Name, draggedData);

                    bool previousAllowDrop = topWindow.AllowDrop;
                    topWindow.AllowDrop = true;
                    topWindow.DragEnter += TopWindow_DragEnter;
                    topWindow.DragOver += TopWindow_DragOver;
                    topWindow.DragLeave += TopWindow_DragLeave;

                    DragDropEffects effects = DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);

                    RemoveDraggedAdorner();

                    topWindow.AllowDrop = previousAllowDrop;
                    topWindow.DragEnter -= TopWindow_DragEnter;
                    topWindow.DragOver -= TopWindow_DragOver;
                    topWindow.DragLeave -= TopWindow_DragLeave;

                    draggedData = null;
                }
            }
        }

        // Diese Methode behandelt das Ereignis "PreviewMouseLeftButtonUp" für eine Drag-Quelle (ItemsControl).
        // Sie setzt das draggedData-Objekt auf null, um anzuzeigen, dass der Drag & Drop-Vorgang abgeschlossen ist.
        private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            draggedData = null;
        }

        // Diese Methode behandelt das Ereignis "PreviewDragEnter" für ein Drop-Ziel
        // Sie speichert das Ziel-ItemsControl und ruft die Methode DecideDropTarget auf, um das genaue Drop-Ziel zu bestimmen.
        // Wenn das gezogene Element nicht null ist, wird der gezogene Adorner angezeigt und der Einfüge-Adorner erstellt.
        private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            targetItemsControl = (ItemsControl)sender;
            object draggedItem = e.Data.GetData(format.Name);

            DecideDropTarget(e);
            if (draggedItem != null)
            {
                ShowDraggedAdorner(e.GetPosition(topWindow));
                CreateInsertionAdorner();
            }
            e.Handled = true;
        }

        // Diese Methode behandelt das Ereignis "PreviewDragOver" für ein Drop-Ziel (ItemsControl).
        // Sie ruft erneut die Methode DecideDropTarget auf, um das Drop-Ziel zu bestimmen.
        // Wenn das gezogene Element nicht null ist, wird der gezogene Adorner aktualisiert und die Position des Einfüge-Adorners wird aktualisiert.
        private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(format.Name);

            DecideDropTarget(e);
            if (draggedItem != null)
            {
                ShowDraggedAdorner(e.GetPosition(topWindow));
                UpdateInsertionAdornerPosition();
            }
            e.Handled = true;
        }


        // Diese Methode behandelt das Ereignis "PreviewDrop" für ein Drop-Ziel (ItemsControl).
        // Sie überprüft, ob ein gültiges gezogenes Element vorhanden ist,
        // und entfernt es gegebenenfalls aus der Drag-Quelle und fügt es dem Drop-Ziel hinzu.
        // Die Methode entfernt auch den gezogenen Adorner und den Einfüge-Adorner.
        private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(format.Name);
            int indexRemoved = -1;

            if (draggedItem != null)
            {
                if ((e.Effects & DragDropEffects.Move) != 0)
                {
                    indexRemoved = Utilities.RemoveItemFromItemsControl(sourceItemsControl, draggedItem);
                }
                if (indexRemoved != -1 && sourceItemsControl == targetItemsControl && indexRemoved < insertionIndex)
                {
                    insertionIndex--;
                }
                Utilities.InsertItemInItemsControl(targetItemsControl, draggedItem, insertionIndex);

                RemoveDraggedAdorner();
                RemoveInsertionAdorner();
            }
            e.Handled = true;
        }

        // Diese Methode behandelt das Ereignis "PreviewDragLeave" für ein Drop-Ziel (ItemsControl).
        // Wenn das gezogene Element nicht null ist, wird der Einfüge-Adorner entfernt.
        private void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(format.Name);

            if (draggedItem != null)
            {
                RemoveInsertionAdorner();
            }
            e.Handled = true;
        }

        // Diese Methode wird verwendet, um das genaue Drop-Ziel zu bestimmen,
        // basierend auf den Eigenschaften des gezogenen Elements und des Drop-Ziels.
        // Sie verwendet Informationen wie die Mausposition, die Orientierung des Drop-Ziels
        // und die Position des gezogenen Elements innerhalb des Drop-Ziels, um zu bestimmen,
        // ob das Element über oder unter einem vorhandenen Element eingefügt werden soll.
        private void DecideDropTarget(DragEventArgs e)
        {
            int targetItemsControlCount = targetItemsControl.Items.Count;
            object draggedItem = e.Data.GetData(format.Name);

            if (IsDropDataTypeAllowed(draggedItem))
            {
                if (targetItemsControlCount > 0)
                {
                    hasVerticalOrientation = Utilities.HasVerticalOrientation(targetItemsControl.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement);
                    targetItemContainer = targetItemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;

                    if (targetItemContainer != null)
                    {
                        Point positionRelativeToItemContainer = e.GetPosition(targetItemContainer);
                        isInFirstHalf = Utilities.IsInFirstHalf(targetItemContainer, positionRelativeToItemContainer, hasVerticalOrientation);
                        insertionIndex = targetItemsControl.ItemContainerGenerator.IndexFromContainer(targetItemContainer);

                        if (!isInFirstHalf)
                        {
                            insertionIndex++;
                        }
                    }
                    else
                    {
                        targetItemContainer = targetItemsControl.ItemContainerGenerator.ContainerFromIndex(targetItemsControlCount - 1) as FrameworkElement;
                        isInFirstHalf = false;
                        insertionIndex = targetItemsControlCount;
                    }
                }
                else
                {
                    targetItemContainer = null;
                    insertionIndex = 0;
                }
            }
            else
            {
                targetItemContainer = null;
                insertionIndex = -1;
                e.Effects = DragDropEffects.None;
            }
        }

        // Die Methode IsDropDataTypeAllowed(DragEventArgs e) wird in der DragDropHelper-Klasse verwendet,
        // um zu überprüfen, ob der Datentyp des gezogenen Elements für das Drop-Ziel zugelassen ist. 
        private bool IsDropDataTypeAllowed(object draggedItem)
        {
            bool isDropDataTypeAllowed;
            IEnumerable collectionSource = targetItemsControl.ItemsSource;
            if (draggedItem != null)
            {
                if (collectionSource != null)
                {
                    Type draggedType = draggedItem.GetType();
                    Type collectionType = collectionSource.GetType();

                    Type genericIListType = collectionType.GetInterface("IList`1");
                    if (genericIListType != null)
                    {
                        Type[] genericArguments = genericIListType.GetGenericArguments();
                        isDropDataTypeAllowed = genericArguments[0].IsAssignableFrom(draggedType);
                    }
                    else if (typeof(IList).IsAssignableFrom(collectionType))
                    {
                        isDropDataTypeAllowed = true;
                    }
                    else
                    {
                        isDropDataTypeAllowed = false;
                    }
                }
                else
                {
                    isDropDataTypeAllowed = true;
                }
            }
            else
            {
                isDropDataTypeAllowed = false;
            }
            return isDropDataTypeAllowed;
        }



        // Window
        // Folgende Methoden reagieren auf die Ereignisse des übergeordeten Fensters

        // Diese Methode behandelt das Ereignis DragEnter des übergeordneten Fensters,
        // wenn sich der Mauszeiger während des Drag & Drop-Vorgangs in das Fenster bewegt.
        private void TopWindow_DragEnter(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        // Diese Methode behandelt das Ereignis DragOver des übergeordneten Fensters,
        // wenn sich der Mauszeiger während des Drag & Drop-Vorgangs über dem Fenster befindet.
        private void TopWindow_DragOver(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        // Diese Methode behandelt das Ereignis DragLeave des übergeordneten Fensters,
        // wenn sich der Mauszeiger während des Drag & Drop-Vorgangs aus dem Fenster bewegt.
        private void TopWindow_DragLeave(object sender, DragEventArgs e)
        {
            RemoveDraggedAdorner();
            e.Handled = true;
        }





        // Adorners
        // Folgende Methoden sind für die visuelle Darstellung von Adornern während des Drag & Drop-Prozesses verantwortlich

        // Diese Methode zeigt den gezogenen Adorner an, der das gezogene Element visuell repräsentiert.
        private void ShowDraggedAdorner(Point currentPosition)
        {
            if (draggedAdorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(sourceItemsControl);
                draggedAdorner = new DraggedAdorner(draggedData, GetDragDropTemplate(sourceItemsControl), sourceItemContainer, adornerLayer);
            }
            draggedAdorner.SetPosition(currentPosition.X - initialMousePosition.X + initialMouseOffset.X, currentPosition.Y - initialMousePosition.Y + initialMouseOffset.Y);
        }

        // Diese Methode entfernt den gezogenen Adorner aus dem übergeordneten Fenster.
        private void RemoveDraggedAdorner()
        {
            if (draggedAdorner != null)
            {
                draggedAdorner.Detach();
                draggedAdorner = null;
            }
        }

        // Diese Methode erstellt einen Einfüge-Adorner, der anzeigt,
        // an welcher Stelle das gezogene Element in das Drop-Ziel eingefügt wird.
        private void CreateInsertionAdorner()
        {
            if (targetItemContainer != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(targetItemContainer);
                insertionAdorner = new InsertionAdorner(hasVerticalOrientation, isInFirstHalf, targetItemContainer, adornerLayer);
            }
        }

        // Diese Methode aktualisiert die Position des Einfüge-Adorners,
        // basierend auf der aktuellen Mausposition während des Drag & Drop-Vorgangs.
        private void UpdateInsertionAdornerPosition()
        {
            if (insertionAdorner != null)
            {
                insertionAdorner.IsInFirstHalf = isInFirstHalf;
                insertionAdorner.InvalidateVisual();
            }
        }

        // Diese Methode entfernt den Einfüge-Adorner aus dem übergeordneten Fenster.
        private void RemoveInsertionAdorner()
        {
            if (insertionAdorner != null)
            {
                insertionAdorner.Detach();
                insertionAdorner = null;
            }
        }
    }
}
