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

        private double _zoomScale = 1.0;
        private const double ZoomFactor = 0.1;

        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            UpdateToolSettingsButton();
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _zoomScale += (e.Delta > 0) ? ZoomFactor : -ZoomFactor;

                _zoomScale = Math.Clamp(_zoomScale, 0.1, 5.0);

                CanvasScaleTransform.ScaleX = _zoomScale;
                CanvasScaleTransform.ScaleY = _zoomScale;

                e.Handled = true;
            }
        }

        private void ResizeCanvas(object sender, RoutedEventArgs e)
        {
            var dialog = new ResizeCanvasDialog(PaintCanvas.Width, PaintCanvas.Height);
            if (dialog.ShowDialog() == true)
            {
                PaintCanvas.Width = dialog.NewWidth;
                PaintCanvas.Height = dialog.NewHeight;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(PaintCanvas);

            if (position.X - _currentThickness / 2 < 0 || position.X + _currentThickness / 2 > PaintCanvas.ActualWidth ||
                position.Y - _currentThickness / 2 < 0 || position.Y + _currentThickness / 2 > PaintCanvas.ActualHeight)
            {
                if (_eraserCursor != null)
                {
                    _eraserCursor.Visibility = Visibility.Collapsed;
                }
                return;
            }

            if (_eraserCursor == null)
            {
                _eraserCursor = new Rectangle
                {
                    Width = _currentThickness,
                    Height = _currentThickness,
                    Fill = Brushes.Transparent,
                    Visibility = Visibility.Visible
                };

                PaintCanvas.Children.Add(_eraserCursor);
            }

            Canvas.SetLeft(_eraserCursor, position.X - _currentThickness / 2);
            Canvas.SetTop(_eraserCursor, position.Y - _currentThickness / 2);

            foreach (UIElement child in PaintCanvas.Children)
            {
                Panel.SetZIndex(child, 0);
            }
            Panel.SetZIndex(_eraserCursor, 1);

            if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                if (_currentTool == "Eraser")
                {
                    var eraserElement = new Rectangle
                    {
                        Fill = Brushes.White,
                        Width = _currentThickness,
                        Height = _currentThickness
                    };

                    Canvas.SetLeft(eraserElement, position.X - _currentThickness / 2);
                    Canvas.SetTop(eraserElement, position.Y - _currentThickness / 2);

                    PaintCanvas.Children.Add(eraserElement);
                }
                else
                {
                    UpdateShape(_currentShapeElement, _startPoint, position);
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(PaintCanvas);

            if (position.X - _currentThickness / 2 < 0 || position.X + _currentThickness / 2 > PaintCanvas.ActualWidth ||
                position.Y - _currentThickness / 2 < 0 || position.Y + _currentThickness / 2 > PaintCanvas.ActualHeight)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_currentTool == "Eraser")
                {
                    _isDrawing = true;
                    _startPoint = position;

                    var eraserElement = new Rectangle
                    {
                        Fill = Brushes.White,
                        Width = _currentThickness,
                        Height = _currentThickness
                    };

                    Canvas.SetLeft(eraserElement, position.X - _currentThickness / 2);
                    Canvas.SetTop(eraserElement, position.Y - _currentThickness / 2);

                    PaintCanvas.Children.Add(eraserElement);
                }
                else
                {
                    ViewModel?.SaveState(PaintCanvas);

                    _isDrawing = true;
                    _startPoint = position;

                    _currentShapeElement = CreateShape(_currentShape, _startPoint, _startPoint);
                    _currentShapeElement.Stroke = ViewModel?.SelectedColor ?? Brushes.Black;
                    _currentShapeElement.StrokeThickness = _currentThickness;

                    PaintCanvas.Children.Add(_currentShapeElement);
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing && e.LeftButton == MouseButtonState.Released)
            {
                _isDrawing = false;
                _currentShapeElement = null;

                if (_currentTool != "Eraser")
                {
                    ViewModel?.SaveState(PaintCanvas);
                }
            }
        }

        private Rectangle _eraserCursor;

        private void OpenToolsWindow(object sender, RoutedEventArgs e)
        {
            var toolsWindow = new ToolsWindow();
            if (toolsWindow.ShowDialog() == true)
            {
                _currentTool = toolsWindow.SelectedTool; 
                UpdateToolSettingsButton(); 
            }
        }

        private string _currentTool = "Brush";

        private void UpdateToolSettingsButton()
        {
            SettingsButton.Click -= OpenPenSettings;
            SettingsButton.Click -= OpenEraserSettings;

            if (_currentTool == "Brush")
            {
                SettingsButton.Content = "Brush Settings";
                SettingsButton.Click += OpenPenSettings; 
            }
            else if (_currentTool == "Eraser")
            {
                SettingsButton.Content = "Eraser Settings";
                SettingsButton.Click += OpenEraserSettings; 
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

        private void OpenEraserSettings(object sender, RoutedEventArgs e)
        {
            var eraserSettingsWindow = new EraserSettingsWindow(_currentThickness);
            if (eraserSettingsWindow.ShowDialog() == true)
            {
                _currentThickness = eraserSettingsWindow.SelectedThickness;
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
