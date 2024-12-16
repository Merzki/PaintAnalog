using PaintAnalog.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

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

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.IsResizing == true) return;
            if (_isDragging) return; 

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

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel?.IsResizing == true) return;
            if (_isDragging) return; 

            if (_currentLine != null && e.LeftButton == MouseButtonState.Pressed)
            {
                _currentLine.Points.Add(e.GetPosition(PaintCanvas));
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel?.IsResizing == true) return;
            if (_isDragging) return;

            if (PaintCanvas != null && _currentLine != null)
            {
                ViewModel?.SaveState(PaintCanvas);
            }
            _currentLine = null;
        }

        private Point _imageStartPosition;
        private UIElement _draggedImage;
        private bool _isDragging = false;

        public void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            if (image != null)
            {
                _draggedImage = image;
                _imageStartPosition = e.GetPosition(PaintCanvas);
                image.CaptureMouse();
                _isDragging = true; 
            }
        }

        public void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedImage != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPos = e.GetPosition(PaintCanvas);
                var offsetX = currentPos.X - _imageStartPosition.X;
                var offsetY = currentPos.Y - _imageStartPosition.Y;

                Canvas.SetLeft(_draggedImage, Canvas.GetLeft(_draggedImage) + offsetX);
                Canvas.SetTop(_draggedImage, Canvas.GetTop(_draggedImage) + offsetY);

                _imageStartPosition = currentPos;
            }
        }

        public void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _draggedImage != null)
            {
                _draggedImage.ReleaseMouseCapture();
                _draggedImage = null;
                _isDragging = false; 
            }
        }
    }
}