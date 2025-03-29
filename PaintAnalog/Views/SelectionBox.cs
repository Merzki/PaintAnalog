using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PaintAnalog.Views
{
    public class SelectionBox : Canvas
    {
        private readonly Rectangle _border;
        private readonly Thumb[] _resizeThumbs = new Thumb[8];
        private UIElement _targetElement;

        public UIElement TargetElement => _targetElement;

        public SelectionBox(UIElement targetElement)
        {
            _targetElement = targetElement;

            _border = new Rectangle
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 4, 2 },
                Width = ((FrameworkElement)targetElement).Width,
                Height = ((FrameworkElement)targetElement).Height
            };

            Children.Add(_border);
            CreateResizeThumbs();
            UpdatePosition();
        }

        private void CreateResizeThumbs()
        {
            Cursor[] cursors =
            {
                Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW, 
                Cursors.SizeWE,                                     
                Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE, 
                Cursors.SizeWE                                     
            };

            for (int i = 0; i < _resizeThumbs.Length; i++)
            {
                var thumb = new Thumb
                {
                    Width = 10,
                    Height = 10,
                    Background = Brushes.White,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Cursor = cursors[i]
                };

                thumb.DragDelta += ResizeThumb_DragDelta;
                _resizeThumbs[i] = thumb;
                Children.Add(thumb);
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_targetElement is not FrameworkElement element) return;
            var thumb = sender as Thumb;
            if (thumb == null) return;

            double newWidth = element.Width;
            double newHeight = element.Height;
            double newLeft = Canvas.GetLeft(element);
            double newTop = Canvas.GetTop(element);

            int index = Array.IndexOf(_resizeThumbs, thumb);
            if (index == -1) return;

            switch (index)
            {
                case 0:
                    newWidth -= e.HorizontalChange;
                    newHeight -= e.VerticalChange;
                    newLeft += e.HorizontalChange;
                    newTop += e.VerticalChange;
                    break;
                case 1:
                    newHeight -= e.VerticalChange;
                    newTop += e.VerticalChange;
                    break;
                case 2: 
                    newWidth += e.HorizontalChange;
                    newHeight -= e.VerticalChange;
                    newTop += e.VerticalChange;
                    break;
                case 3: 
                    newWidth += e.HorizontalChange;
                    break;
                case 4: 
                    newWidth += e.HorizontalChange;
                    newHeight += e.VerticalChange;
                    break;
                case 5: 
                    newHeight += e.VerticalChange;
                    break;
                case 6:
                    newWidth -= e.HorizontalChange;
                    newHeight += e.VerticalChange;
                    newLeft += e.HorizontalChange;
                    break;
                case 7: 
                    newWidth -= e.HorizontalChange;
                    newLeft += e.HorizontalChange;
                    break;
            }

            newWidth = Math.Max(10, newWidth);
            newHeight = Math.Max(10, newHeight);

            element.Width = newWidth;
            element.Height = newHeight;
            Canvas.SetLeft(element, newLeft);
            Canvas.SetTop(element, newTop);

            _border.Width = newWidth;
            _border.Height = newHeight;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            if (_targetElement is not FrameworkElement element) return;

            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            double width = element.Width;
            double height = element.Height;

            Canvas.SetLeft(_border, left);
            Canvas.SetTop(_border, top);

            (double, double)[] positions =
            {
                (left - 5, top - 5),                      
                (left + width / 2 - 5, top - 5),          
                (left + width - 5, top - 5),              
                (left + width - 5, top + height / 2 - 5), 
                (left + width - 5, top + height - 5),     
                (left + width / 2 - 5, top + height - 5), 
                (left - 5, top + height - 5),             
                (left - 5, top + height / 2 - 5)          
            };

            for (int i = 0; i < _resizeThumbs.Length; i++)
            {
                Canvas.SetLeft(_resizeThumbs[i], positions[i].Item1);
                Canvas.SetTop(_resizeThumbs[i], positions[i].Item2);
            }
        }

    }
}
