using System.Windows.Controls;
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
        private UIElementCollection _canvasChildren;
        private double _canvasWidth;
        private double _canvasHeight;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public UIElementCollection CanvasChildren
        {
            get => _canvasChildren;
            set
            {
                _canvasChildren = value;
                OnPropertyChanged();
            }
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                _canvasWidth = value;
                OnPropertyChanged();
            }
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                _canvasHeight = value;
                OnPropertyChanged();
            }
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
        public ICommand OpenImageCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand InsertImageCommand { get; }
        public ICommand ConfirmChangesCommand { get; }
        public ICommand PasteImageCommand { get; }

        public MainViewModel()
        {
            ClearCanvasCommand = new RelayCommand(ClearCanvas);
            ChooseColorCommand = new RelayCommand(ChooseColor);
            SaveCanvasCommand = new RelayCommand(SaveCanvas);
            OpenImageCommand = new RelayCommand(OpenImage);
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
            PasteImageCommand = new RelayCommand(PasteImage);
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
            if (parameter is not Canvas canvas) return;

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
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Document = new FlowDocument(new Paragraph())
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
                if (rtb == null || string.IsNullOrEmpty(e.Text)) return;

                var caretPosition = rtb.CaretPosition;

                if (!rtb.Selection.IsEmpty)
                {
                    rtb.Selection.Text = e.Text;
                    ApplyCurrentStyleToSelection(rtb);
                }
                else
                {
                    var textRange = new TextRange(caretPosition, caretPosition) { Text = e.Text };
                    ApplyCurrentStyleToTextRange(textRange);

                    rtb.CaretPosition = textRange.End;
                }

                e.Handled = true;
                rtb.Dispatcher.InvokeAsync(() => AutoResizeRichTextBox(rtb, border),
                    System.Windows.Threading.DispatcherPriority.Background);
            };

            richTextBox.PreviewKeyDown += (s, e) =>
            {
                var rtb = s as WpfRichTextBox;
                if (rtb == null) return;

                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    if (!rtb.Selection.IsEmpty)
                    {
                        rtb.Selection.Text = "";
                        e.Handled = true;
                    }
                }
            };

            richTextBox.TextChanged += (s, e) =>
            {
                var rtb = s as WpfRichTextBox;
                if (rtb == null) return;

                rtb.Dispatcher.InvokeAsync(() => AutoResizeRichTextBox(rtb, border),
                    System.Windows.Threading.DispatcherPriority.Background);
            };

            this.PropertyChanged += (sender, e) =>
            {
                if (richTextBox.IsKeyboardFocusWithin &&
                    (e.PropertyName == nameof(SelectedColor) ||
                     e.PropertyName == nameof(TextSize) ||
                     e.PropertyName == nameof(SelectedFontFamily) ||
                     e.PropertyName == nameof(IsBold) ||
                     e.PropertyName == nameof(IsItalic) ||
                     e.PropertyName == nameof(IsUnderline)))
                {
                    ApplyCurrentStyleToSelection(richTextBox);
                }
            };

            richTextBox.Focus();
            IsEditingText = true;

            SaveState(canvas);
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private void ApplyCurrentStyleToSelection(WpfRichTextBox rtb)
        {
            if (rtb == null || rtb.Selection.IsEmpty) return;

            var textRange = new TextRange(rtb.Selection.Start, rtb.Selection.End);
            ApplyCurrentStyleToTextRange(textRange);
        }

        private void ApplyCurrentStyleToTextRange(TextRange textRange)
        {
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, SelectedColor);
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, TextSize);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, SelectedFontFamily);
            textRange.ApplyPropertyValue(TextElement.FontWeightProperty,
                IsBold ? FontWeights.Bold : FontWeights.Normal);
            textRange.ApplyPropertyValue(TextElement.FontStyleProperty,
                IsItalic ? FontStyles.Italic : FontStyles.Normal);
            textRange.ApplyPropertyValue(Inline.TextDecorationsProperty,
                IsUnderline ? TextDecorations.Underline : null);
        }

        private void AutoResizeRichTextBox(WpfRichTextBox rtb, Border border)
        {
            rtb.Width = double.NaN;
            rtb.Height = double.NaN;
            rtb.Document.PageWidth = 10000;
            rtb.UpdateLayout();

            double maxWidth = 0, maxHeight = 0;

            for (var pointer = rtb.Document.ContentStart;
                 pointer != null && pointer.CompareTo(rtb.Document.ContentEnd) < 0;
                 pointer = pointer.GetNextContextPosition(LogicalDirection.Forward))
            {
                try
                {
                    var rect = pointer.GetCharacterRect(LogicalDirection.Forward);
                    maxWidth = Math.Max(maxWidth, rect.Right);
                    maxHeight = Math.Max(maxHeight, rect.Bottom);
                }
                catch { }
            }

            border.Width = Math.Max(rtb.MinWidth, maxWidth + 20);
            border.Height = Math.Max(rtb.MinHeight, maxHeight + 10);
            rtb.Width = border.Width;
            rtb.Height = border.Height;
        }

        private bool _isDragging;
        private Image _draggedImage;
        private Point _startPoint;

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
                SaveState(canvas);

                var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                var image = new Image
                {
                    Source = bitmap,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    IsEnabled = true,
                    Stretch = Stretch.Fill
                };

                Canvas.SetLeft(image, (canvas.ActualWidth - bitmap.Width) / 2);
                Canvas.SetTop(image, (canvas.ActualHeight - bitmap.Height) / 2);

                image.MouseWheel += Image_MouseWheel;

                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                image.MouseMove += Image_MouseMove;
                image.MouseLeftButtonUp += Image_MouseLeftButtonUp;

                var selectionBox = new SelectionBox(image);
                canvas.Children.Add(image);
                canvas.Children.Add(selectionBox);
                SetIsEditable(image, true);

                SaveState(canvas);
                ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
            }
        }

        private void PasteImage(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            if (Clipboard.ContainsImage())
            {
                SaveState(canvas);

                var bitmap = Clipboard.GetImage();
                var image = new Image
                {
                    Source = bitmap,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    Stretch = Stretch.Fill
                };

                Canvas.SetLeft(image, (canvas.ActualWidth - bitmap.Width) / 2);
                Canvas.SetTop(image, (canvas.ActualHeight - bitmap.Height) / 2);

                image.MouseWheel += Image_MouseWheel;
                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                image.MouseMove += Image_MouseMove;
                image.MouseLeftButtonUp += Image_MouseLeftButtonUp;

                var selectionBox = new SelectionBox(image);
                canvas.Children.Add(image);
                canvas.Children.Add(selectionBox);
                SetIsEditable(image, true);

                SaveState(canvas);
                ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
            }
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is Image image)
            {
                var transform = image.RenderTransform as RotateTransform;
                if (transform == null)
                {
                    transform = new RotateTransform(0);
                    image.RenderTransform = transform;
                    image.RenderTransformOrigin = new Point(0.5, 0.5);
                }

                double angle = transform.Angle + (e.Delta > 0 ? 10 : -10); 
                transform.Angle = angle;

                e.Handled = true;
            }
        }

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

                    foreach (var child in canvas.Children)
                    {
                        if (child is SelectionBox selectionBox && selectionBox.TargetElement == _draggedImage)
                        {
                            selectionBox.UpdatePosition();
                            break;
                        }
                    }
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _draggedImage?.ReleaseMouseCapture();
                _draggedImage = null;
                IsEditingImage = false;
            }
        }

        private bool CanConfirmChanges(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return false;

            bool hasEditableImage = false;

            foreach (var element in canvas.Children)
            {
                if (element is Border border && border.Child is System.Windows.Controls.RichTextBox richTextBox && !richTextBox.IsReadOnly)
                    return true;

                if (element is Image image && image.IsHitTestVisible)
                    hasEditableImage = true; 

                if (element is SelectionBox)
                    return true; 
            }

            return hasEditableImage; 
        }

        private void ConfirmChanges(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            var focusedElement = Keyboard.FocusedElement as TextBox;
            focusedElement?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            List<UIElement> toRemove = new List<UIElement>();

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
                    SelectionBox selectionBox = canvas.Children
                        .OfType<SelectionBox>()
                        .FirstOrDefault(sb => sb.TargetElement == image);

                    if (selectionBox != null)
                        toRemove.Add(selectionBox);

                    image.MouseLeftButtonDown -= Image_MouseLeftButtonDown;
                    image.MouseMove -= Image_MouseMove;
                    image.MouseLeftButtonUp -= Image_MouseLeftButtonUp;
                    image.MouseWheel -= Image_MouseWheel;

                    image.IsHitTestVisible = false; 
                }
            }

            foreach (var element in toRemove)
            {
                canvas.Children.Remove(element);
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

        private void RestoreImageHandlers(Canvas canvas)
        {
            foreach (var element in canvas.Children.OfType<Image>())
            {
                if (GetIsEditable(element))  
                {
                    element.MouseWheel += Image_MouseWheel;
                    element.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                    element.MouseMove += Image_MouseMove;
                    element.MouseLeftButtonUp += Image_MouseLeftButtonUp;
                    element.IsHitTestVisible = true;
                }
            }
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

            RestoreImageHandlers(canvas);

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

                var brushCursor = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "BrushCursor");

                canvas.Children.Clear(); 

                if (brushCursor != null)
                {
                    canvas.Children.Add(brushCursor);
                    Panel.SetZIndex(brushCursor, int.MaxValue);
                }
            }

            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        public double ImageDpiX { get; set; } = 96;
        public double ImageDpiY { get; set; } = 96;

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
                    ImageDpiX, ImageDpiY, PixelFormats.Pbgra32);

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

        public static readonly DependencyProperty IsEditableProperty =
           DependencyProperty.RegisterAttached("IsEditable", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static void SetIsEditable(UIElement element, bool value)
        {
            element.SetValue(IsEditableProperty, value);
        }

        public static bool GetIsEditable(UIElement element)
        {
            return (bool)element.GetValue(IsEditableProperty);
        }

        private void OpenImage(object parameter)
        {
            if (parameter is Canvas paintCanvas)
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    SaveState(paintCanvas);

                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                    ImageDpiX = bitmap.DpiX;
                    ImageDpiY = bitmap.DpiY;

                    var brushCursor = paintCanvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "BrushCursor");
                    var elementsToKeep = brushCursor != null ? new List<UIElement> { brushCursor } : new List<UIElement>();

                    paintCanvas.Children.Clear();
                    foreach (var element in elementsToKeep)
                    {
                        paintCanvas.Children.Add(element);
                    }

                    paintCanvas.Width = bitmap.PixelWidth;
                    paintCanvas.Height = bitmap.PixelHeight;
                    CanvasWidth = bitmap.PixelWidth;
                    CanvasHeight = bitmap.PixelHeight;

                    var image = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.None,
                        Width = bitmap.PixelWidth,
                        Height = bitmap.PixelHeight
                    };

                    Canvas.SetLeft(image, 0);
                    Canvas.SetTop(image, 0);
                    paintCanvas.Children.Add(image);
                    SetIsEditable(image, false);

                    SaveState(paintCanvas);
                }
            }
        }
    }
}