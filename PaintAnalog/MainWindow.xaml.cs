using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PaintAnalog.ViewModels;
using PaintAnalog.Views;

namespace PaintAnalog
{
    public partial class MainWindow : Window
    {
        private bool _isDrawing;
        private Point _startPoint;
        private Shape _currentShapeElement;

        private UIElement _selectedElement; 
        private Point _elementStartPoint;
        private Point _canvasStartOffset;

        private double _currentThickness = 2.0;
        private string _currentShape = "Polyline";

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

                if (clickedElement != null && (clickedElement is Image || clickedElement is Shape))
                {
                    _selectedElement = clickedElement;
                    _elementStartPoint = position;
                    _canvasStartOffset = new Point(Canvas.GetLeft(clickedElement), Canvas.GetTop(clickedElement));
                }
                else
                {
                    ViewModel?.SaveState(canvas);

                    _isDrawing = true;
                    _startPoint = position;

                    _currentShapeElement = CreateShape(_currentShape, _startPoint, _startPoint);
                    _currentShapeElement.Stroke = ViewModel?.SelectedColor ?? Brushes.Black;
                    _currentShapeElement.StrokeThickness = _currentThickness;

                    PaintCanvas.Children.Add(_currentShapeElement);
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(PaintCanvas);

            if (_selectedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var offsetX = position.X - _elementStartPoint.X;
                var offsetY = position.Y - _elementStartPoint.Y;

                Canvas.SetLeft(_selectedElement, _canvasStartOffset.X + offsetX);
                Canvas.SetTop(_selectedElement, _canvasStartOffset.Y + offsetY);
            }
            else if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateShape(_currentShapeElement, _startPoint, position);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_selectedElement != null)
            {
                ViewModel?.SaveState(PaintCanvas);
                _selectedElement = null;
            }

            if (_isDrawing && e.LeftButton == MouseButtonState.Released)
            {
                _isDrawing = false;
                _currentShapeElement = null;

                ViewModel?.SaveState(PaintCanvas);
            }
        }

        private void OpenPenSettings(object sender, RoutedEventArgs e)
        {
            var penSettingsWindow = new PenSettingsWindow(ViewModel?.SelectedColor ?? Brushes.Black, _currentThickness, _currentShape);
            if (penSettingsWindow.ShowDialog() == true)
            {
                _currentThickness = penSettingsWindow.SelectedThickness;
                _currentShape = penSettingsWindow.SelectedShape;
            }
        }

        private Shape CreateShape(string shapeType, Point startPoint, Point endPoint)
        {
            switch (shapeType)
            {
                case "Rectangle":
                    return new Rectangle
                    {
                        Width = 0,
                        Height = 0,
                        Stroke = Brushes.Black,
                        StrokeThickness = _currentThickness
                    };
                case "Ellipse":
                    return new Ellipse
                    {
                        Width = 0,
                        Height = 0,
                        Stroke = Brushes.Black,
                        StrokeThickness = _currentThickness
                    };
                case "Polyline":
                    return new Polyline
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = _currentThickness,
                        Points = new PointCollection { startPoint }
                    };
                case "Triangle":
                    return new Polygon
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = _currentThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection { startPoint, startPoint, startPoint } 
                    };
                default:
                    return new Rectangle
                    {
                        Width = 0,
                        Height = 0,
                        Stroke = Brushes.Black,
                        StrokeThickness = _currentThickness
                    };
            }
        }

        private void UpdateShape(Shape shape, Point startPoint, Point endPoint)
        {
            if (shape is Rectangle rectangle)
            {
                var x = Math.Min(startPoint.X, endPoint.X);
                var y = Math.Min(startPoint.Y, endPoint.Y);
                var width = Math.Abs(startPoint.X - endPoint.X);
                var height = Math.Abs(startPoint.Y - endPoint.Y);

                rectangle.Width = width;
                rectangle.Height = height;

                Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, y);
            }
            else if (shape is Ellipse ellipse)
            {
                var x = Math.Min(startPoint.X, endPoint.X);
                var y = Math.Min(startPoint.Y, endPoint.Y);
                var width = Math.Abs(startPoint.X - endPoint.X);
                var height = Math.Abs(startPoint.Y - endPoint.Y);

                ellipse.Width = width;
                ellipse.Height = height;

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
            }
            else if (shape is Polyline polyline)
            {
                polyline.Points.Add(endPoint);
            }
            else if (shape is Polygon polygon && _currentShape == "Triangle")
            {
                var baseMidpoint = new Point(
                    (startPoint.X + endPoint.X) / 2, 
                    endPoint.Y 
                );

                var height = Math.Abs(endPoint.X - startPoint.X) / 2; 
                var vertex = new Point(
                    baseMidpoint.X, 
                    startPoint.Y - height 
                );

                polygon.Points.Clear();
                polygon.Points.Add(startPoint);  
                polygon.Points.Add(endPoint);    
                polygon.Points.Add(vertex);      
            }
        }

    }
}
