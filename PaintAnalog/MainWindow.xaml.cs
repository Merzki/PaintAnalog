using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PaintAnalog.ViewModels;

namespace PaintAnalog
{
    public partial class MainWindow : Window
    {
        private bool _isDrawing;
        private Point _startPoint;
        private Polyline _currentPolyline;

        private Image _selectedImage;
        private Point _imageStartPoint;
        private Point _canvasStartOffset;

        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            var position = e.GetPosition(PaintCanvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var clickedElement = e.OriginalSource as UIElement;

                if (clickedElement is Image image)
                {
                    _selectedImage = image;
                    _imageStartPoint = position;
                    _canvasStartOffset = new Point(Canvas.GetLeft(image), Canvas.GetTop(image));
                }
                else
                {
                    ViewModel?.SaveState(canvas);

                    _isDrawing = true;
                    _startPoint = position;

                    _currentPolyline = new Polyline
                    {
                        Stroke = ViewModel?.SelectedColor ?? Brushes.Black,
                        StrokeThickness = 2,
                        Points = new PointCollection { _startPoint }
                    };

                    PaintCanvas.Children.Add(_currentPolyline);
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(PaintCanvas);

            if (_selectedImage != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var offsetX = position.X - _imageStartPoint.X;
                var offsetY = position.Y - _imageStartPoint.Y;

                Canvas.SetLeft(_selectedImage, _canvasStartOffset.X + offsetX);
                Canvas.SetTop(_selectedImage, _canvasStartOffset.Y + offsetY);
            }
            else if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                _currentPolyline?.Points.Add(position);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_selectedImage != null)
            {
                ViewModel?.SaveState(PaintCanvas);
                _selectedImage = null;
            }

            if (_isDrawing && e.LeftButton == MouseButtonState.Released)
            {
                _isDrawing = false;
                _currentPolyline = null;

                ViewModel?.SaveState(PaintCanvas);
            }
        }
    }
}
