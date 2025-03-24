using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PaintAnalog.Views
{
    public class ResizeAdorner : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly Thumb _topLeft, _bottomRight, _topRight, _bottomLeft;
        private FrameworkElement _adornedElement => AdornedElement as FrameworkElement;

        private double _initialAngle;
        private Point _rotationCenter;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);

            _topLeft = CreateResizeThumb(Cursors.SizeNWSE);
            _bottomRight = CreateResizeThumb(Cursors.SizeNWSE);
            _topRight = CreateRotationThumb(Cursors.ScrollAll);
            _bottomLeft = CreateRotationThumb(Cursors.ScrollAll); 

            _visuals.Add(_topLeft);
            _visuals.Add(_bottomRight);
            _visuals.Add(_topRight);
            _visuals.Add(_bottomLeft);

            _topLeft.DragDelta += TopLeft_DragDelta;
            _bottomRight.DragDelta += BottomRight_DragDelta;

            _topRight.DragStarted += RotationThumb_DragStarted;
            _topRight.DragDelta += RotationThumb_DragDelta;

            _bottomLeft.DragStarted += RotationThumb_DragStarted;
            _bottomLeft.DragDelta += RotationThumb_DragDelta;
        }

        private void RotationThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (_adornedElement == null) return;

            double left = Canvas.GetLeft(_adornedElement);
            double top = Canvas.GetTop(_adornedElement);
            _rotationCenter = new Point(left + _adornedElement.Width / 2, top + _adornedElement.Height / 2);

            var rotateTransform = _adornedElement.RenderTransform as RotateTransform;
            _initialAngle = rotateTransform?.Angle ?? 0;
        }

        private void RotationThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_adornedElement == null) return;

            var parentCanvas = VisualTreeHelper.GetParent(_adornedElement) as UIElement;
            if (parentCanvas == null) return;

            Point mousePos = Mouse.GetPosition(parentCanvas);
            Vector delta = Point.Subtract(mousePos, _rotationCenter);

            double currentAngle = Math.Atan2(delta.Y, delta.X) * 180 / Math.PI;
            double angleToApply = NormalizeAngle(_initialAngle + currentAngle);
            angleToApply = GetSnappedAngle(angleToApply);

            var rotateTransform = _adornedElement.RenderTransform as RotateTransform;
            if (rotateTransform == null)
            {
                rotateTransform = new RotateTransform(angleToApply, _adornedElement.Width / 2, _adornedElement.Height / 2);
                _adornedElement.RenderTransform = rotateTransform;
            }
            else
            {
                rotateTransform.Angle = angleToApply;
                rotateTransform.CenterX = _adornedElement.Width / 2;
                rotateTransform.CenterY = _adornedElement.Height / 2;
            }

            InvalidateArrange();
        }

        private double NormalizeAngle(double angle)
        {
            angle %= 360;
            if (angle < 0)
                angle += 360;
            return angle;
        }

        private double GetSnappedAngle(double angle)
        {
            double[] snapPoints = { 0, 90, 180, 270, 360 };
            foreach (var point in snapPoints)
            {
                if (Math.Abs(angle - point) <= 5)
                {
                    return point;
                }
            }
            return angle;
        }

        private void TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_adornedElement != null)
            {
                var newWidth = _adornedElement.Width - e.HorizontalChange;
                var newHeight = _adornedElement.Height - e.VerticalChange;

                if (newWidth > 0 && newHeight > 0)
                {
                    _adornedElement.Width = newWidth;
                    _adornedElement.Height = newHeight;

                    Canvas.SetLeft(_adornedElement, Canvas.GetLeft(_adornedElement) + e.HorizontalChange);
                    Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) + e.VerticalChange);

                    InvalidateArrange();
                }
            }
        }

        private void BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_adornedElement != null)
            {
                var newWidth = _adornedElement.Width + e.HorizontalChange;
                var newHeight = _adornedElement.Height + e.VerticalChange;

                if (newWidth > 0 && newHeight > 0)
                {
                    _adornedElement.Width = newWidth;
                    _adornedElement.Height = newHeight;

                    InvalidateArrange();
                }
            }
        }

        private Thumb CreateResizeThumb(Cursor cursor)
        {
            return new Thumb
            {
                Width = 12,
                Height = 12,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Cursor = cursor
            };
        }

        private Thumb CreateRotationThumb(Cursor cursor)
        {
            return new Thumb
            {
                Width = 14,
                Height = 14,
                Background = Brushes.Black,
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(2),
                Cursor = cursor
            };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_adornedElement == null)
                return base.ArrangeOverride(finalSize);

            var adornedRect = new Rect(AdornedElement.RenderSize);

            _topLeft.Arrange(new Rect(adornedRect.TopLeft, new Size(_topLeft.Width, _topLeft.Height)));
            _bottomRight.Arrange(new Rect(adornedRect.BottomRight - new Vector(_bottomRight.Width, _bottomRight.Height),
                new Size(_bottomRight.Width, _bottomRight.Height)));

            _topRight.Arrange(new Rect(new Point(adornedRect.TopRight.X - _topRight.Width, adornedRect.TopRight.Y),
                new Size(_topRight.Width, _topRight.Height)));

            _bottomLeft.Arrange(new Rect(new Point(adornedRect.BottomLeft.X, adornedRect.BottomLeft.Y - _bottomLeft.Height),
                new Size(_bottomLeft.Width, _bottomLeft.Height)));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => _visuals[index];

        protected override int VisualChildrenCount => _visuals.Count;
    }
}
