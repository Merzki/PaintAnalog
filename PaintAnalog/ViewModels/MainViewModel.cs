﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Documents;
using PaintAnalog.Views;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using WpfRichTextBox = System.Windows.Controls.RichTextBox;
using XceedRichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace PaintAnalog.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _title = "Paint Analog";
        private SolidColorBrush _selectedColor = new SolidColorBrush(Colors.Black);
        private readonly Stack<UIElement[]> _undoElements = new();
        private readonly Stack<UIElement[]> _redoElements = new();
        private double _textSize = 12;
        private FontFamily _selectedFontFamily = new FontFamily("Segoe UI");

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

        private bool _isEditingImage;
        public bool IsEditingImage
        {
            get => _isEditingImage;
            set => SetProperty(ref _isEditingImage, value);
        }

        private bool _isEditingText;
        public bool IsEditingText
        {
            get => _isEditingText;
            set => SetProperty(ref _isEditingText, value);
        }
        public double TextSize
        {
            get => _textSize;
            set
            {
                var roundedValue = Math.Round(value); 
                if (_textSize != roundedValue)
                {
                    SetProperty(ref _textSize, roundedValue);
                }
            }
        }
        public FontFamily SelectedFontFamily
        {
            get => _selectedFontFamily;
            set => SetProperty(ref _selectedFontFamily, value);
        }

        private bool _isBold;
        public bool IsBold
        {
            get => _isBold;
            set
            {
                _isBold = value;
                OnPropertyChanged();
            }
        }

        private bool _isItalic;
        public bool IsItalic
        {
            get => _isItalic;
            set
            {
                _isItalic = value;
                OnPropertyChanged();
            }
        }

        private bool _isUnderline;
        public bool IsUnderline
        {
            get => _isUnderline;
            set
            {
                _isUnderline = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand InsertTextCommand { get; }
        public ICommand ClearCanvasCommand { get; }
        public ICommand ChooseColorCommand { get; }
        public ICommand SaveCanvasCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand InsertImageCommand { get; }
        public ICommand ConfirmChangesCommand { get; }

        public MainViewModel()
        {
            ClearCanvasCommand = new RelayCommand(ClearCanvas);
            ChooseColorCommand = new RelayCommand(ChooseColor);
            SaveCanvasCommand = new RelayCommand(SaveCanvas);
            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);
            InsertImageCommand = new RelayCommand(InsertImage);
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                var canvas = mainWindow?.FindName("PaintCanvas") as Canvas;
                SaveState(canvas);
            });
            ConfirmChangesCommand = new RelayCommand(ConfirmChanges, CanConfirmChanges);
            InsertTextCommand = new RelayCommand(InsertRichText);
        }

        private void ChooseColor(object parameter)
        {
            var colorDialog = new Xceed.Wpf.Toolkit.ColorPicker
            {
                SelectedColor = SelectedColor.Color,
                UsingAlphaChannel = true
            };

            var popup = new System.Windows.Controls.Primitives.Popup
            {
                Placement = System.Windows.Controls.Primitives.PlacementMode.Center,
                PlacementTarget = parameter as System.Windows.UIElement,
                StaysOpen = false,
                Child = colorDialog,
                IsOpen = true
            };

            colorDialog.MouseLeftButtonUp += (s, e) =>
            {
                if (colorDialog.SelectedColor.HasValue)
                {
                    SelectedColor = new SolidColorBrush(colorDialog.SelectedColor.Value);
                }
                popup.IsOpen = false;
            };
        }

        private void InsertRichText(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            var richTextBox = new WpfRichTextBox
            {
                FontSize = TextSize,
                FontFamily = SelectedFontFamily,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = SelectedColor,
                AcceptsReturn = true,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                MinWidth = 100,
                MinHeight = 50,
                Padding = new Thickness(2),
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Background = Brushes.Transparent,
                Child = richTextBox
            };

            Canvas.SetLeft(border, (canvas.ActualWidth - 200) / 2);
            Canvas.SetTop(border, (canvas.ActualHeight - 50) / 2);

            SaveState(canvas);
            canvas.Children.Add(border);

            border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            border.MouseMove += Border_MouseMove;
            border.MouseLeftButtonUp += Border_MouseLeftButtonUp;

            richTextBox.PreviewTextInput += (s, e) =>
            {
                var rtb = s as WpfRichTextBox;
                if (rtb == null) return;

                var caretPos = rtb.CaretPosition;
                if (caretPos.Paragraph == null)
                {
                    caretPos = rtb.Document.Blocks.FirstBlock.ContentEnd;
                }

                var run = new Run(e.Text)
                {
                    Foreground = SelectedColor,
                    FontSize = TextSize,
                    FontFamily = SelectedFontFamily,
                    FontWeight = IsBold ? FontWeights.Bold : FontWeights.Normal,
                    FontStyle = IsItalic ? FontStyles.Italic : FontStyles.Normal,
                    TextDecorations = IsUnderline ? TextDecorations.Underline : null
                };

                caretPos.InsertTextInRun(""); 
                caretPos.Paragraph.Inlines.Add(run);
                rtb.CaretPosition = run.ContentEnd;
                e.Handled = true;

                AutoResizeRichTextBox(rtb, border);
            };

            richTextBox.TextChanged += (s, e) =>
            {
                AutoResizeRichTextBox(richTextBox, border);
            };

            richTextBox.Focus();
            IsEditingText = true;

            SaveState(canvas);
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private void AutoResizeRichTextBox(WpfRichTextBox rtb, Border border)
        {
            rtb.Width = double.NaN;
            rtb.Height = double.NaN;
            rtb.Document.PageWidth = 10000;
            rtb.UpdateLayout();

            rtb.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

            double maxWidth = 0;
            double maxHeight = 0;

            var pointer = rtb.Document.ContentStart;

            while (pointer != null && pointer.CompareTo(rtb.Document.ContentEnd) < 0)
            {
                var next = pointer.GetNextContextPosition(LogicalDirection.Forward);
                if (next == null) break;

                try
                {
                    var rect = pointer.GetCharacterRect(LogicalDirection.Forward);
                    if (rect.Right > maxWidth) maxWidth = rect.Right;
                    if (rect.Bottom > maxHeight) maxHeight = rect.Bottom;
                }
                catch
                {

                }

                pointer = next;
            }

            double newWidth = Math.Max(rtb.MinWidth, maxWidth + 20);
            double newHeight = Math.Max(rtb.MinHeight, maxHeight + 10);

            border.Width = newWidth;
            border.Height = newHeight;
            rtb.Width = newWidth;
            rtb.Height = newHeight;
        }

        public void InsertImage(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Insert Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                var image = new Image
                {
                    Source = bitmap,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    IsEnabled = true
                };

                Canvas.SetLeft(image, (canvas.ActualWidth - bitmap.Width) / 2);
                Canvas.SetTop(image, (canvas.ActualHeight - bitmap.Height) / 2);

                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                image.MouseMove += Image_MouseMove;
                image.MouseLeftButtonUp += Image_MouseLeftButtonUp;

                canvas.Children.Add(image);

                var adornerLayer = AdornerLayer.GetAdornerLayer(image);
                if (adornerLayer != null)
                {
                    var resizeAdorner = new ResizeAdorner(image);
                    adornerLayer.Add(resizeAdorner);
                }

                SaveState(canvas);

                ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
            }
        }

        private Point _startPoint;
        private bool _isDragging;
        private Image _draggedImage;

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                _isDragging = true;
                _draggedImage = image;
                _startPoint = e.GetPosition(image);

                IsEditingImage = true;

                image.CaptureMouse();
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedImage != null)
            {
                var canvas = VisualTreeHelper.GetParent(_draggedImage) as Canvas;
                if (canvas != null)
                {
                    var currentPoint = e.GetPosition(canvas);

                    Canvas.SetLeft(_draggedImage, currentPoint.X - _startPoint.X);
                    Canvas.SetTop(_draggedImage, currentPoint.Y - _startPoint.Y);
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;

                if (_draggedImage != null)
                {
                    _draggedImage.ReleaseMouseCapture();
                    _draggedImage = null;

                    IsEditingImage = false;

                    var adornerLayer = AdornerLayer.GetAdornerLayer(sender as UIElement);
                    if (adornerLayer != null)
                    {
                        var resizeAdorner = new ResizeAdorner(sender as UIElement);
                        adornerLayer.Add(resizeAdorner);
                    }
                }
            }
        }

        private bool CanConfirmChanges(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return false;

            foreach (var element in canvas.Children)
            {
                if (element is Border border && border.Child is System.Windows.Controls.RichTextBox richTextBox && !richTextBox.IsReadOnly)
                    return true;

                if (element is Image image)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(image);
                    if (adornerLayer != null)
                    {
                        var adorners = adornerLayer.GetAdorners(image);
                        if (adorners != null && adorners.Length > 0)
                            return true;
                    }
                }
            }

            return false;
        }

        private void ConfirmChanges(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            var focusedElement = Keyboard.FocusedElement as TextBox;
            focusedElement?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            foreach (var element in canvas.Children)
            {
                if (element is Border border && border.Child is System.Windows.Controls.RichTextBox richTextBox)
                {
                    richTextBox.IsReadOnly = true;
                    richTextBox.Background = Brushes.Transparent;
                    richTextBox.BorderThickness = new Thickness(0);
                    richTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    richTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    richTextBox.Focusable = false;
                    richTextBox.Cursor = Cursors.Arrow;

                    border.MouseLeftButtonDown -= Border_MouseLeftButtonDown;
                    border.MouseMove -= Border_MouseMove;
                    border.MouseLeftButtonUp -= Border_MouseLeftButtonUp;

                    border.BorderBrush = Brushes.Transparent;
                    border.BorderThickness = new Thickness(0);
                }

                if (element is Image image)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(image);
                    if (adornerLayer != null)
                    {
                        var adorners = adornerLayer.GetAdorners(image);
                        if (adorners != null)
                        {
                            foreach (var adorner in adorners)
                            {
                                adornerLayer.Remove(adorner);
                            }
                        }
                    }

                    image.MouseLeftButtonDown -= Image_MouseLeftButtonDown;
                    image.MouseMove -= Image_MouseMove;
                    image.MouseLeftButtonUp -= Image_MouseLeftButtonUp;
                }
            }

            IsEditingText = false;
            IsEditingImage = false;

            SaveState(canvas);

            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private bool isDragging;
        private Point startPoint;
        private Border border;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var canvas = VisualTreeHelper.GetParent(border) as Canvas;

            if (border != null && canvas != null)
            {
                isDragging = true;
                startPoint = e.GetPosition(canvas);

                border.BorderThickness = new Thickness(3);
                border.CaptureMouse();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && sender is Border border)
            {
                var canvas = VisualTreeHelper.GetParent(border) as Canvas;
                if (canvas != null)
                {
                    var currentPoint = e.GetPosition(canvas);
                    var offsetX = currentPoint.X - startPoint.X;
                    var offsetY = currentPoint.Y - startPoint.Y;

                    Canvas.SetLeft(border, Canvas.GetLeft(border) + offsetX);
                    Canvas.SetTop(border, Canvas.GetTop(border) + offsetY);

                    startPoint = currentPoint; 
                }
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && sender is Border border)
            {
                isDragging = false;
                border.ReleaseMouseCapture();
            }
        }

        private void AddBorderEventHandlers(Border border)
        {
            border.MouseEnter += (s, e) =>
            {
                border.BorderBrush = Brushes.Blue; 
            };

            border.MouseLeave += (s, e) =>
            {
                border.BorderBrush = Brushes.Gray; 
            };

            border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            border.MouseMove += Border_MouseMove;
            border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
        }

        private bool _isResizing;
        public bool IsResizing
        {
            get => _isResizing;
            set => SetProperty(ref _isResizing, value);
        }

        private void AddResizeHandles(Canvas canvas, Image image)
        {
            var topLeftHandle = CreateResizeHandle();
            var bottomRightHandle = CreateResizeHandle();

            UpdateHandlePositions(image, topLeftHandle, bottomRightHandle);

            canvas.Children.Add(topLeftHandle);
            canvas.Children.Add(bottomRightHandle);

            topLeftHandle.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _isResizing = true;
                    var position = e.GetPosition(canvas);

                    var newWidth = image.Width - (position.X - Canvas.GetLeft(image));
                    var newHeight = image.Height - (position.Y - Canvas.GetTop(image));

                    if (newWidth > 0 && newHeight > 0)
                    {
                        image.Width = newWidth;
                        image.Height = newHeight;

                        Canvas.SetLeft(image, position.X);
                        Canvas.SetTop(image, position.Y);

                        UpdateHandlePositions(image, topLeftHandle, bottomRightHandle);
                    }
                }
            };

            bottomRightHandle.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _isResizing = true;
                    var position = e.GetPosition(canvas);

                    var newWidth = position.X - Canvas.GetLeft(image);
                    var newHeight = position.Y - Canvas.GetTop(image);

                    if (newWidth > 0 && newHeight > 0)
                    {
                        image.Width = newWidth;
                        image.Height = newHeight;

                        UpdateHandlePositions(image, topLeftHandle, bottomRightHandle);
                    }
                }
            };

            topLeftHandle.MouseUp += (s, e) => _isResizing = false;
            bottomRightHandle.MouseUp += (s, e) => _isResizing = false;
        }

        private void UpdateHandlePositions(Image image, Rectangle topLeftHandle, Rectangle bottomRightHandle)
        {
            Canvas.SetLeft(topLeftHandle, Canvas.GetLeft(image) - topLeftHandle.Width / 2);
            Canvas.SetTop(topLeftHandle, Canvas.GetTop(image) - topLeftHandle.Height / 2);

            Canvas.SetLeft(bottomRightHandle, Canvas.GetLeft(image) + image.Width - bottomRightHandle.Width / 2);
            Canvas.SetTop(bottomRightHandle, Canvas.GetTop(image) + image.Height - bottomRightHandle.Height / 2);
        }

        private Rectangle CreateResizeHandle()
        {
            return new Rectangle
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Transparent,
                StrokeThickness = 0,
                Cursor = Cursors.SizeNWSE
            };
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
            if (canvas == null || _undoElements.Count <= 1) return;

            var currentElements = _undoElements.Pop();
            _redoElements.Push(currentElements);

            var previousElements = _undoElements.Peek();
            RestoreCanvas(canvas, previousElements);

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private void Redo(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null || _redoElements.Count == 0) return;

            var nextElements = _redoElements.Pop();
            _undoElements.Push(nextElements);
            RestoreCanvas(canvas, nextElements);

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private void RestoreCanvas(Canvas canvas, UIElement[] elements)
        {
            canvas.Children.Clear();
            foreach (var element in elements)
            {
                canvas.Children.Add(element);
            }
        }

        private bool CanUndo(object parameter)
        {
            return _undoElements.Count > 1;
        }

        private bool CanRedo(object parameter)
        {
            return _redoElements.Count > 0;
        }

        private void ClearCanvas(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas != null)
            {
                SaveState(canvas);
                canvas.Children.Clear();
            }

            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
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
                Rect bounds = new Rect();
                foreach (UIElement element in canvas.Children)
                {
                    if (element.Visibility == Visibility.Visible)
                    {
                        Rect elementBounds = VisualTreeHelper.GetDescendantBounds(element);
                        if (elementBounds.IsEmpty) continue;

                        Point elementPosition = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
                        if (double.IsNaN(elementPosition.X)) elementPosition.X = 0;
                        if (double.IsNaN(elementPosition.Y)) elementPosition.Y = 0;

                        GeneralTransform transform = element.TransformToAncestor(canvas);
                        elementPosition = transform.Transform(elementPosition);

                        bounds.Union(new Rect(elementPosition, elementBounds.Size));
                    }
                }

                if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
                {
                    System.Windows.MessageBox.Show("Canva is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var renderBitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(canvas.ActualWidth),
                (int)Math.Ceiling(canvas.ActualHeight),
                96, 96, PixelFormats.Pbgra32);

                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    context.DrawRectangle(new VisualBrush(canvas), null, new Rect(new Point(), new Size(canvas.ActualWidth, canvas.ActualHeight)));
                }
                renderBitmap.Render(visual);


                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                {
                    encoder.Save(stream);
                }
            }
        }
    }
}