using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using System.Collections.Generic;
using System.Windows;

namespace PaintAnalog.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _title = "Paint Analog";
        private SolidColorBrush _selectedColor = new SolidColorBrush(Colors.Black);
        private readonly Stack<UIElement[]> _undoElements = new();
        private readonly Stack<UIElement[]> _redoElements = new();

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public SolidColorBrush SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        public ICommand ClearCanvasCommand { get; }
        public ICommand ChooseColorCommand { get; }
        public ICommand SaveCanvasCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public MainViewModel()
        {
            ClearCanvasCommand = new RelayCommand(ClearCanvas);
            ChooseColorCommand = new RelayCommand(ChooseColor);
            SaveCanvasCommand = new RelayCommand(SaveCanvas);
            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);
        }

        private void ChooseColor(object parameter)
        {
            var colorDialog = new Xceed.Wpf.Toolkit.ColorPicker
            {
                SelectedColor = SelectedColor.Color
            };

            var colorPickerPopup = new System.Windows.Controls.Primitives.Popup
            {
                Placement = System.Windows.Controls.Primitives.PlacementMode.Center,
                PlacementTarget = parameter as System.Windows.UIElement,
                StaysOpen = false,
                Child = colorDialog,
                IsOpen = true
            };

            colorDialog.SelectedColorChanged += (s, e) =>
            {
                if (colorDialog.SelectedColor.HasValue)
                {
                    SelectedColor = new SolidColorBrush(colorDialog.SelectedColor.Value);
                }
                else
                {
                    SelectedColor = new SolidColorBrush(Colors.Black);
                }
                colorPickerPopup.IsOpen = false;
            };
        }


        private void ClearCanvas(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas != null)
            {
                SaveState(canvas);
                canvas.Children.Clear();
            }
        }

        private void SaveCanvas(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save Canvas As Image"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var renderBitmap = new RenderTargetBitmap(
                    (int)canvas.ActualWidth,
                    (int)canvas.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);

                renderBitmap.Render(canvas);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                {
                    encoder.Save(stream);
                }
            }
        }

        public void SaveState(Canvas canvas)
        {
            if (canvas == null) return;

            var currentElements = canvas.Children.Cast<UIElement>().ToArray();

            if (_undoElements.Count == 0 || !currentElements.SequenceEqual(_undoElements.Peek()))
            {
                _undoElements.Push(currentElements);
                _redoElements.Clear();

                ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
            }
        }


        private void Undo(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null || _undoElements.Count == 0) return;

            var currentElements = canvas.Children.Cast<UIElement>().ToArray();
            _redoElements.Push(currentElements);

            var previousElements = _undoElements.Pop(); 
            RestoreCanvas(canvas, previousElements);

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
        }

        private void Redo(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null || _redoElements.Count == 0) return;

            var currentElements = canvas.Children.Cast<UIElement>().ToArray();
            _undoElements.Push(currentElements); 

            var nextElements = _redoElements.Pop(); 
            RestoreCanvas(canvas, nextElements);

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
        }


        private void RestoreCanvas(Canvas canvas, UIElement[] elements)
        {
            canvas.Children.Clear(); 
            foreach (var element in elements)
            {
                canvas.Children.Add(element);
            }
        }

        private bool CanUndo(object parameter) => _undoElements.Count > 0;

        private bool CanRedo(object parameter) => _redoElements.Count > 0;
    }
}