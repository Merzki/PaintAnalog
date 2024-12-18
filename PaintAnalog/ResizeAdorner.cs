using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PaintAnalog
{
    public class ResizeAdorner : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly Thumb _topLeft, _bottomRight;

        private FrameworkElement _adornedElement => AdornedElement as FrameworkElement;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);

            _topLeft = CreateThumb(Cursors.SizeNWSE);
            _bottomRight = CreateThumb(Cursors.SizeNWSE);

            _visuals.Add(_topLeft);
            _visuals.Add(_bottomRight);

            _topLeft.DragDelta += TopLeft_DragDelta;
            _bottomRight.DragDelta += BottomRight_DragDelta;
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

                    InvalidateVisual();
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

                    InvalidateVisual();
                }
            }
        }

        private Thumb CreateThumb(Cursor cursor)
        {
            var thumb = new Thumb
            {
                Width = 10,
                Height = 10,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Cursor = cursor
            };
            return thumb;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_adornedElement == null)
                return base.ArrangeOverride(finalSize);

            var adornedElementRect = new Rect(AdornedElement.RenderSize);

            _topLeft.Arrange(new Rect(adornedElementRect.TopLeft, new Size(_topLeft.Width, _topLeft.Height)));
            _bottomRight.Arrange(new Rect(adornedElementRect.BottomRight - new Vector(_bottomRight.Width, _bottomRight.Height),
            new Size(_bottomRight.Width, _bottomRight.Height)));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => _visuals[index];

        protected override int VisualChildrenCount => _visuals.Count;
    }
}
