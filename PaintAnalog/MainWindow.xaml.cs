﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PaintAnalog.ViewModels;
using PaintAnalog.Views;
using System.ComponentModel;

namespace PaintAnalog
{
    public partial class MainWindow : Window
    {
        private bool _isDrawing;
        private Point _startPoint;
        private Shape _currentShapeElement;
        private List<Point> _pointBuffer = new List<Point>();
        private List<Point> _pendingPoints = new List<Point>();
        private const double InterpolationStep = 0.3;
        private const int MaxBufferSize = 1000;
        private const int RenderBatchSize = 50;

        private UIElement _selectedElement;
        private Point _elementStartPoint;
        private Point _canvasStartOffset;

        private double _currentThickness = 2.0;
        private string _currentShape = "Polyline";
        private Ellipse _brushCursor;

        private double _zoomScale = 1.0;
        private const double ZoomFactor = 0.1;

        private MainViewModel ViewModel => DataContext as MainViewModel;
        private DispatcherTimer _renderingTimer;

        public MainWindow()
        {
            InitializeComponent();
            UpdateToolSettingsButton();
            _renderingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; 
            _renderingTimer.Tick += OnRendering;
            _renderingTimer.Start();

            Canvas.SetLeft(PaintCanvas, 0);
            Canvas.SetTop(PaintCanvas, 0);

            _brushCursor = new Ellipse
            {
                Name = "BrushCursor",
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.Transparent,
                Width = _currentThickness,
                Height = _currentThickness,
                IsHitTestVisible = false,
            };

            if (!PaintCanvas.Children.Contains(_brushCursor))
            {
                PaintCanvas.Children.Add(_brushCursor);
                Panel.SetZIndex(_brushCursor, int.MaxValue);
            }

            this.PreviewMouseLeftButtonUp += GlobalMouseLeftButtonUp;
            Mouse.AddLostMouseCaptureHandler(this, OnLostMouseCapture);

            this.Closing += MainWindow_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PaintCanvas.Focus();
            AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.B)
                {
                    BoldButton.IsChecked = !BoldButton.IsChecked;
                    e.Handled = true;
                }
                else if (e.Key == Key.I)
                {
                    ItalicButton.IsChecked = !ItalicButton.IsChecked;
                    e.Handled = true;
                }
                else if (e.Key == Key.U)
                {
                    UnderlineButton.IsChecked = !UnderlineButton.IsChecked;
                    e.Handled = true;
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (PaintCanvas.Children.Count <= 1)
            {
                return;
            }

            var result = MessageBox.Show(
                "Do you want to save your work before exiting?",
                "Save before exit",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var viewModel = DataContext as ViewModels.MainViewModel;

                if (viewModel?.SaveCanvasCommand?.CanExecute(PaintCanvas) == true)
                {
                    viewModel.SaveCanvasCommand.Execute(PaintCanvas);
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void PaletteEllipse_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && DataContext is MainViewModel vm)
            {
                var clickedBrush = ellipse.Fill as SolidColorBrush;
                var index = vm.PaletteColors.IndexOf(clickedBrush);
                if (index >= 0)
                {
                    vm.SelectedPaletteIndex = index;
                }
            }
        }

        private void GlobalMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.IsSelecting ?? false)
            {
                ViewModel.EndSelection(PaintCanvas, _zoomScale);
                e.Handled = true;
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (ViewModel?.IsSelecting ?? false)
            {
                ViewModel.EndSelection(PaintCanvas, _zoomScale);
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (_currentShapeElement is Polyline polyline && _pendingPoints.Count > 0)
            {
                var startTime = DateTime.Now;

                while (_pendingPoints.Count > 0)
                {
                    polyline.Points.Add(_pendingPoints[0]);
                    _pendingPoints.RemoveAt(0);

                    if ((DateTime.Now - startTime).TotalMilliseconds > 10)
                        break;
                }
            }
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double scale = (e.Delta > 0) ? ZoomFactor : -ZoomFactor;
                _zoomScale = Math.Clamp(_zoomScale + scale, 0.1, 5.0);

                CanvasScaleTransform.ScaleX = _zoomScale;
                CanvasScaleTransform.ScaleY = _zoomScale;

                Canvas.SetLeft(PaintCanvas, 0);
                Canvas.SetTop(PaintCanvas, 0);

                e.Handled = true;
            }
        }

        private void ResizeCanvas(object sender, RoutedEventArgs e)
        {
            var dialog = new ResizeCanvasDialog(PaintCanvas.Width, PaintCanvas.Height);
            if (dialog.ShowDialog() == true)
            {

                double newWidth = dialog.NewWidth;
                double newHeight = dialog.NewHeight;

                PaintCanvas.Width = newWidth;
                PaintCanvas.Height = newHeight;

                Canvas.SetLeft(PaintCanvas, 0);
                Canvas.SetTop(PaintCanvas, 0);

                CanvasScrollViewer.ScrollToHorizontalOffset(0);
                CanvasScrollViewer.ScrollToVerticalOffset(0);

                RemoveOutOfBoundsElements();

                ViewModel.SaveState(PaintCanvas); 
            }
        }

        private void RemoveOutOfBoundsElements()
        {
            for (int i = PaintCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (PaintCanvas.Children[i] is Shape shape)
                {
                    double left = Canvas.GetLeft(shape);
                    double top = Canvas.GetTop(shape);

                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    double right = left + shape.RenderSize.Width;
                    double bottom = top + shape.RenderSize.Height;

                    if (right > PaintCanvas.Width || bottom > PaintCanvas.Height)
                    {
                        PaintCanvas.Children.RemoveAt(i);
                    }
                }
            }
        }

        private DateTime _lastMouseMoveTime = DateTime.MinValue;
        private const double MouseMoveInterval = 16;
        private DispatcherTimer _timer;

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var now = DateTime.Now;
            var timeSinceLastMove = (now - _lastMouseMoveTime).TotalMilliseconds;

            if (timeSinceLastMove < 5) 
                return;

            _lastMouseMoveTime = now;

            var position = e.GetPosition(PaintCanvas);
            UpdateBrushCursor(position);

            if (ViewModel?.IsSelecting ?? false)
            {
                ViewModel?.UpdateSelection(position, PaintCanvas);
                return;
            }

            if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                if (_currentTool == "Eraser" && _currentShapeElement is Polyline eraserPolyline)
                {
                    HandleDrawing(eraserPolyline, position, Brushes.White, InterpolationStep);
                }
                else if (_currentTool == "Brush" && _currentShapeElement is Polyline brushPolyline)
                {
                    double adaptiveStep = _currentThickness > 20 ? InterpolationStep * 2 : InterpolationStep;
                    HandleDrawing(brushPolyline, position, brushPolyline.Stroke, adaptiveStep);
                }
                else
                {
                    UpdateShape(_currentShapeElement, _startPoint, position);
                }
            }
        }

        private void HandleDrawing(Polyline polyline, Point position, Brush brush, double interpolationStep)
        {
            if (_pointBuffer.Count == 0 || GetDistance(_pointBuffer.Last(), position) >= interpolationStep)
            {
                _pointBuffer.Add(position);
            }

            if (_pointBuffer.Count > MaxBufferSize)
            {
                _pointBuffer.RemoveRange(0, _pointBuffer.Count - MaxBufferSize);
            }

            if (_pointBuffer.Count >= 2)
            {
                var smoothPoints = InterpolatePoints(_pointBuffer, interpolationStep);
                
                for (int i = 0; i < smoothPoints.Count; i += RenderBatchSize)
                {
                    int batchSize = Math.Min(RenderBatchSize, smoothPoints.Count - i);
                    var batch = smoothPoints.GetRange(i, batchSize);
                    _pendingPoints.AddRange(batch);
                }

                _pointBuffer.Clear();
                _pointBuffer.Add(position);
            }
        }

        private void UpdateBrushCursor(Point? position = null)
        {
            if (_brushCursor == null)
                return;

            if (_currentTool == "Fill" || _currentTool == "Selection")
            {
                _brushCursor.Visibility = Visibility.Hidden;
            }
            else
            {
                _brushCursor.Visibility = Visibility.Visible;

                var brushColor = (ViewModel?.SelectedColor as SolidColorBrush)?.Color ?? Colors.Black;

                if (_currentThickness > 20)
                {
                    _brushCursor.StrokeThickness = 1;
                    _brushCursor.Width = _currentThickness;
                    _brushCursor.Height = _currentThickness;
                }
                else
                {
                    _brushCursor.StrokeThickness = 1;
                    _brushCursor.Width = _currentThickness;
                    _brushCursor.Height = _currentThickness;
                }

                _brushCursor.Stroke = _currentTool == "Eraser" ? Brushes.Black : new SolidColorBrush(brushColor);
                _brushCursor.Fill = _currentTool == "Eraser" ? Brushes.White : new SolidColorBrush(brushColor);

                if (position.HasValue)
                {
                    Canvas.SetLeft(_brushCursor, position.Value.X - _currentThickness / 2);
                    Canvas.SetTop(_brushCursor, position.Value.Y - _currentThickness / 2);
                }
            }
        }

        private double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(PaintCanvas);

            if (ViewModel?.IsEditingImage == true || ViewModel?.IsEditingText == true)
            {
                return;
            }

            if (_currentTool == "Selection")
            {
                ViewModel?.StartSelection(position, PaintCanvas);
                return;
            }

            double adjustedX = Math.Max(0, Math.Min(PaintCanvas.ActualWidth - _currentThickness / 2, position.X));
            double adjustedY = Math.Max(0, Math.Min(PaintCanvas.ActualHeight - _currentThickness / 2, position.Y));
            position = new Point(adjustedX, adjustedY);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _pointBuffer.Clear();
                _pendingPoints.Clear();

                if (_currentTool == "Eraser")
                {
                    _isDrawing = true;
                    _startPoint = position;

                    var eraserPolyline = new Polyline
                    {
                        Stroke = Brushes.White,
                        StrokeThickness = _currentThickness
                    };

                    eraserPolyline.Points.Add(position);
                    _currentShapeElement = eraserPolyline;

                    PaintCanvas.Children.Add(eraserPolyline);
                }
                else if (_currentTool == "Fill")
                {
                    PerformFill(position);
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

                    if (_currentShape == "Polyline")
                    {
                        AddEndPointDot(position, ViewModel?.SelectedColor ?? Brushes.Black, _currentThickness);
                    }
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.IsSelecting ?? false)
            {
                ViewModel.EndSelection(PaintCanvas, _zoomScale);
                return;
            }

            if (_isDrawing && e.LeftButton == MouseButtonState.Released)
            {
                _isDrawing = false;

                if (_currentShapeElement is Polyline polyline && polyline.Points.Count > 0)
                {
                    Point endPoint = polyline.Points.Last();
                    AddEndPointDot(endPoint, polyline.Stroke, polyline.StrokeThickness);
                }

                _currentShapeElement = null;
                _pointBuffer.Clear();

                ViewModel?.SaveState(PaintCanvas);
            }
        }

        private void AddEndPointDot(Point position, Brush color, double thickness)
        {
            var dot = new Ellipse
            {
                Width = thickness,
                Height = thickness,
                Fill = color,
                Stroke = color,
                StrokeThickness = 1
            };

            Canvas.SetLeft(dot, position.X - thickness / 2);
            Canvas.SetTop(dot, position.Y - thickness / 2);

            PaintCanvas.Children.Add(dot);
        }

        private List<Point> InterpolatePoints(List<Point> points, double step)
        {
            var interpolated = new List<Point>();
            if (points.Count < 2) return interpolated;

            interpolated.Add(points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                var p1 = points[i - 1];
                var p2 = points[i];
                double distance = GetDistance(p1, p2);

                if (distance < step * 0.5)
                    continue;

                double adjustedStep = step;
                if (distance > step * 3)
                {
                    adjustedStep = distance / 3;
                }

                int numSteps = (int)(distance / adjustedStep);
                for (int j = 1; j <= numSteps; j++)
                {
                    double t = j / (double)numSteps;
                    double x = p1.X + t * (p2.X - p1.X);
                    double y = p1.Y + t * (p2.Y - p1.Y);
                    interpolated.Add(new Point(x, y));
                }
            }

            interpolated.Add(points[points.Count - 1]);
            return interpolated;
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
            UpdateBrushCursor();
        }

        private string _currentTool = "Brush";

        private void UpdateToolSettingsButton()
        {
            SettingsButton.Click -= OpenPenSettings;
            SettingsButton.Click -= OpenEraserSettings;

            if (_currentTool == "Brush")
            {
                SettingsButton.Content = "Brush Settings";
                SettingsButton.IsEnabled = true;
                SettingsButton.Click += OpenPenSettings;
            }
            else if (_currentTool == "Eraser")
            {
                SettingsButton.Content = "Eraser Settings";
                SettingsButton.IsEnabled = true;
                SettingsButton.Click += OpenEraserSettings;
            }
            else if (_currentTool == "Fill" || _currentTool == "Selection")
            {
                SettingsButton.Content = "No Settings";
                SettingsButton.IsEnabled = false;
            }
            else
            {
                SettingsButton.IsEnabled = true;
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
            UpdateBrushCursor();
        }

        private void OpenEraserSettings(object sender, RoutedEventArgs e)
        {
            var eraserSettingsWindow = new EraserSettingsWindow(_currentThickness);
            if (eraserSettingsWindow.ShowDialog() == true)
            {
                _currentThickness = eraserSettingsWindow.SelectedThickness;
            }
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void PerformFill(Point position)
        {
            var fillColor = (ViewModel?.SelectedColor as SolidColorBrush)?.Color ?? Colors.Black;

            int canvasWidth = (int)(PaintCanvas.ActualWidth * _zoomScale);
            int canvasHeight = (int)(PaintCanvas.ActualHeight * _zoomScale);

            if (ViewModel != null && ViewModel.IsFirstState)
            {
                ViewModel.SaveState(PaintCanvas);
                ViewModel.IsFirstState = false;
            }

            var renderTarget = new RenderTargetBitmap(canvasWidth, canvasHeight, 96, 96, PixelFormats.Pbgra32);

            RenderOptions.SetBitmapScalingMode(PaintCanvas, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(PaintCanvas, EdgeMode.Aliased);

            renderTarget.Render(PaintCanvas);

            var pixels = new byte[canvasWidth * canvasHeight * 4];
            renderTarget.CopyPixels(pixels, canvasWidth * 4, 0);

            int x = (int)(position.X * _zoomScale);
            int y = (int)(position.Y * _zoomScale);

            if (x < 0 || x >= canvasWidth || y < 0 || y >= canvasHeight)
            {
                MessageBox.Show("Click was made outside of canvas");
                return;
            }

            int index = (y * canvasWidth + x) * 4;
            var targetColor = Color.FromArgb(
                pixels[index + 3],
                pixels[index + 2],
                pixels[index + 1],
                pixels[index]
            );

            if (targetColor == fillColor)
            {
                MessageBox.Show("It's already the same color");
                return;
            }

            FloodFill(pixels, x, y, targetColor, fillColor, canvasWidth, canvasHeight);

            var writeableBitmap = new WriteableBitmap(canvasWidth, canvasHeight, 96, 96, PixelFormats.Pbgra32, null);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, canvasWidth, canvasHeight), pixels, canvasWidth * 4, 0);

            var image = new Image
            {
                Source = writeableBitmap,
                Width = PaintCanvas.ActualWidth,
                Height = PaintCanvas.ActualHeight
            };

            PaintCanvas.Children.Clear();
            PaintCanvas.Children.Add(image);
            PaintCanvas.Children.Add(_brushCursor);
            ViewModel?.SaveState(PaintCanvas);
        }

        private void FloodFill(byte[] pixels, int x, int y, Color targetColor, Color fillColor, int width, int height)
        {
            var stack = new Stack<Point>();
            stack.Push(new Point(x, y));

            byte fillR = fillColor.R, fillG = fillColor.G, fillB = fillColor.B, fillA = fillColor.A;

            while (stack.Count > 0)
            {
                var point = stack.Pop();
                int px = (int)point.X, py = (int)point.Y;

                if (px < 0 || px >= width || py < 0 || py >= height)
                    continue;

                int index = (py * width + px) * 4;

                var currentColor = Color.FromArgb(
                    pixels[index + 3],
                    pixels[index + 2],
                    pixels[index + 1],
                    pixels[index]
                );

                if (AreColorsSimilar(currentColor, targetColor))
                {
                    pixels[index] = fillB;
                    pixels[index + 1] = fillG;
                    pixels[index + 2] = fillR;
                    pixels[index + 3] = fillA;

                    stack.Push(new Point(px + 1, py));
                    stack.Push(new Point(px - 1, py));
                    stack.Push(new Point(px, py + 1));
                    stack.Push(new Point(px, py - 1));
                }
            }
        }

        private bool AreColorsSimilar(Color c1, Color c2, int tolerance = 1)
        {
            return Math.Abs(c1.R - c2.R) <= tolerance &&
                   Math.Abs(c1.G - c2.G) <= tolerance &&
                   Math.Abs(c1.B - c2.B) <= tolerance &&
                   Math.Abs(c1.A - c2.A) <= tolerance;
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
                _pointBuffer.Add(endPoint);
                if (_pointBuffer.Count >= 2)
                {
                    var smoothPoints = InterpolatePoints(_pointBuffer, InterpolationStep);
                    foreach (var point in smoothPoints)
                    {
                        polyline.Points.Add(point);
                    }
                    _pointBuffer.Clear();
                    _pointBuffer.Add(endPoint);
                }
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

        private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void OnlyNumbers_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextNumeric(string text)
        {
            return text.All(char.IsDigit);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text) || !double.TryParse(textBox.Text, out _))
                {
                    textBox.Text = "12"; 
                    var binding = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
                    binding?.UpdateSource(); 
                }
            }
        }

    }
}