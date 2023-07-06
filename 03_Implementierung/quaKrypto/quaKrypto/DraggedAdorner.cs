using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace quaKrypto
{
    // Es wird eine Klasse namens "DraggedAdorner" definiert, die von der Klasse "Adorner" erbt.
    // Die Klasse repräsentiert einen Adorner, der verwendet wird, um ein visuelles Element anzuzeigen,
    // das während eines Drag & Drop-Vorgangs gezogen wird
    public class DraggedAdorner : Adorner
    {
        private ContentPresenter contentPresenter; // Content Presenter
        private double left; // linke Position des Adorners
        private double top; // obere Position des Adorners
        private AdornerLayer adornerLayer; // Referenz auf den AdornerLayer

        // Der Konstruktor der Klasse nimmt verschiedene Parameter entgegen, darunter Daten für den Drag & Drop-Vorgang,
        // eine DataTemplate für die Darstellung des Inhalts und eine Referenz auf das Element, über dem der Adorner angezeigt werden soll.
        // Der Konstruktor initialisiert den ContentPresenter mit den übergebenen Daten und Vorlagen
        // und setzt die Opazität auf 0,7. Anschließend wird der Adorner zur AdornerLayer hinzugefügt.
        public DraggedAdorner(object dragDropData, DataTemplate dragDropTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            this.adornerLayer = adornerLayer;

            contentPresenter = new ContentPresenter();
            contentPresenter.Content = dragDropData;
            contentPresenter.ContentTemplate = dragDropTemplate;
            contentPresenter.Opacity = 0.7;

            this.adornerLayer.Add(this);
        }

        // SetPosition wird verwendet, um die Position des Adorners festzulegen.
        // Sie aktualisiert die Werte für "left" und "top"
        // und ruft dann die Methode "Update" der AdornerLayer auf, um die Position des Adorners zu aktualisieren.
        public void SetPosition(double left, double top)
        {
            this.left = left - 1;
            this.top = top + 13;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }

        // MeasureOverride überschreibt die entsprechende Methode der Basisklasse,
        // um die Größe des ContentPresenters zu messen und zurückzugeben.
        protected override Size MeasureOverride(Size constraint)
        {
            contentPresenter.Measure(constraint);
            return contentPresenter.DesiredSize;
        }

        // ArrangeOverride überschreibt die entsprechende Methode der Basisklasse,
        // um den ContentPresenter innerhalb der angegebenen Größe anzuordnen.
        protected override Size ArrangeOverride(Size finalSize)
        {
            contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        // GetVisualChild gibt die Anzahl der visuellen Kinder zurück
        protected override Visual GetVisualChild(int index)
        {
            return contentPresenter;
        }

        // In diesem Fall gibt GetVisualChild 1 zurück, da nur der ContentPresenter vorhanden ist.
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        // GetDesiredTransform überschreibt die entsprechende Methode der Basisklasse, um eine Transformationsgruppe
        // zu erstellen, die die gewünschte Transformation enthält.
        // Die Basistransformation wird beibehalten, und eine zusätzliche TranslateTransform
        // wird hinzugefügt, um die linke und obere Position des Adorners zu berücksichtigen.
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(left, top));

            return result;
        }

        // Detach wird verwendet, um den Adorner von der AdornerLayer zu entfernen
        public void Detach()
        {
            adornerLayer.Remove(this);
        }

    }
}
