using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace quaKrypto
{
    public class InsertionAdorner : Adorner
    {
        // Informationen über die Ausrichtung des Adorners und die Position innerhalb des Elements
        private bool isSeparatorHorizontal; // gibt an, ob der Adorner horizontal oder vertikal ausgerichtet ist.
        public bool IsInFirstHalf { get; set; } // gibt an, in welcher Hälfte sich der Adorner befindet
        private AdornerLayer adornerLayer; // Ebene, auf der der Adorner gerendert wird
        private static Pen pen; // zeichnet die Linie des Adorners
        private static PathGeometry triangle; // enthält die Dreiecksform, die an den Enden der Linie gezeichnet wird

        // Der Konstruktor nimmt verschiedene Parameter entgegen, darunter Informationen zur Ausrichtung,
        // zur Position im Element und eine Referenz auf das Element, über dem der Adorner angezeigt werden soll.
        // Der Konstruktor initialisiert die Felder und fügt den Adorner zur AdornerLayer hinzu
        static InsertionAdorner()
        {
            pen = new Pen { Brush = Brushes.Gray, Thickness = 2 };
            pen.Freeze();

            LineSegment firstLine = new LineSegment(new Point(0, -5), false);
            firstLine.Freeze();
            LineSegment secondLine = new LineSegment(new Point(0, 5), false);
            secondLine.Freeze();

            PathFigure figure = new PathFigure { StartPoint = new Point(5, 0) };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            triangle = new PathGeometry();
            triangle.Figures.Add(figure);
            triangle.Freeze();
        }

        public InsertionAdorner(bool isSeparatorHorizontal, bool isInFirstHalf, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            this.isSeparatorHorizontal = isSeparatorHorizontal;
            IsInFirstHalf = isInFirstHalf;
            this.adornerLayer = adornerLayer;
            IsHitTestVisible = false;

            this.adornerLayer.Add(this);
        }

        // Die Methode "OnRender" wird überschrieben, um das visuelle Aussehen des Adorners zu definieren.
        // In dieser Methode wird die Linie zwischen einem Startpunkt und einem Endpunkt gezeichnet,
        // wobei der Pen verwendet wird. Je nach Ausrichtung des Adorners werden zwei Dreiecke
        // an den Enden der Linie gezeichnet, um den Einfügepunkt visuell zu markieren.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Point startPoint;
            Point endPoint;

            CalculateStartAndEndPoint(out startPoint, out endPoint);
            drawingContext.DrawLine(pen, startPoint, endPoint);

            if (isSeparatorHorizontal)
            {
                DrawTriangle(drawingContext, startPoint, 0);
                DrawTriangle(drawingContext, endPoint, 180);
            }
            else
            {
                DrawTriangle(drawingContext, startPoint, 90);
                DrawTriangle(drawingContext, endPoint, -90);
            }
        }

        // Die Methode "DrawTriangle" wird verwendet, um ein Dreieck an einem bestimmten Punkt
        // mit einem bestimmten Winkel zu zeichnen. Dabei wird eine Translationstransformation
        // und eine Rotationsstransformation auf den Zeichenkontext angewendet.
        private void DrawTriangle(DrawingContext drawingContext, Point origin, double angle)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(angle));

            drawingContext.DrawGeometry(pen.Brush, null, triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        // Die Methode "CalculateStartAndEndPoint" berechnet den Startpunkt
        // und Endpunkt der Linie basierend auf der Größe des Elements und der Ausrichtung des Adorners.
        private void CalculateStartAndEndPoint(out Point startPoint, out Point endPoint)
        {
            startPoint = new Point();
            endPoint = new Point();

            double width = AdornedElement.RenderSize.Width;
            double height = AdornedElement.RenderSize.Height;

            if (isSeparatorHorizontal)
            {
                endPoint.X = width;
                if (!IsInFirstHalf)
                {
                    startPoint.Y = height;
                    endPoint.Y = height;
                }
            }
            else
            {
                endPoint.Y = height;
                if (!IsInFirstHalf)
                {
                    startPoint.X = width;
                    endPoint.X = width;
                }
            }
        }

        // Die Methode "Detach" wird verwendet, um den Adorner von der AdornerLayer zu entfernen.
        public void Detach()
        {
            adornerLayer.Remove(this);
        }

    }
}
