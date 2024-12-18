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

        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var canvas = sender as Canvas;

                ViewModel?.SaveState(canvas);

                _isDrawing = true;
                _startPoint = e.GetPosition(PaintCanvas);

                _currentPolyline = new Polyline
                {
                    Stroke = ViewModel?.SelectedColor ?? Brushes.Black,
                    StrokeThickness = 2,
                    Points = new PointCollection { _startPoint }
                };

                PaintCanvas.Children.Add(_currentPolyline);
            }
        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(PaintCanvas);
                _currentPolyline?.Points.Add(position);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing && e.LeftButton == MouseButtonState.Released)
            {
                _isDrawing = false;
                _currentPolyline = null;

                ViewModel?.SaveState(PaintCanvas);
            }
        }
    }
}
