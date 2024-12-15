using PaintAnalog.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PaintAnalog
{
    public partial class MainWindow : Window
    {
        private Polyline _currentLine;
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel?.SaveState(PaintCanvas);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentLine != null && e.LeftButton == MouseButtonState.Pressed)
            {
                _currentLine.Points.Add(e.GetPosition(PaintCanvas));
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (ViewModel?.SelectedColor != null)
                {
                    _currentLine = new Polyline
                    {
                        Stroke = ViewModel.SelectedColor,
                        StrokeThickness = 2
                    };

                    _currentLine.Points.Add(e.GetPosition(PaintCanvas));
                    PaintCanvas.Children.Add(_currentLine);
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PaintCanvas != null && _currentLine != null)
            {
                ViewModel?.SaveState(PaintCanvas); 
            }
            _currentLine = null;
        }
    }
}
