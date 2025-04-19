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
        private Stack<CanvasElementState[]> _undoElements = new();
        private Stack<CanvasElementState[]> _redoElements = new();
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

        private double _textSize = 12;
        public double TextSize
        {
            get => _textSize;
            set
            {
                if (double.IsNaN(value) || value <= 0) value = 12;
                var rounded = Math.Round(value);
                SetProperty(ref _textSize, rounded);
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

        private bool _isSelecting;
        private Rectangle? _selectionRect;
        private Point _selectionStart;

        public bool IsSelecting
        {
            get => _isSelecting;
            set => SetProperty(ref _isSelecting, value);
        }

        public class CanvasElementState
        {
            public UIElement Element { get; set; }
            public bool IsHitTestVisible { get; set; }
            public double CanvasWidth { get; set; }
            public double CanvasHeight { get; set; }
            public bool? IsTextEditable { get; set; }
        }

        public bool IsFirstState { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        private Size? _lastSelectionSize = null;
        private bool _clipboardImageIsFromApp = false;

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
        public ICommand CopyLastImageCommand { get; }

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
            CopyLastImageCommand = new RelayCommand(CopyLastImage);
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

        public void CopyLastImage(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null) return;

            var lastImage = canvas.Children
                .OfType<Image>()
                .LastOrDefault();

            if (lastImage == null) return;

            var source = lastImage.Source as BitmapSource;

            if (source == null) return;

            var dataObject = new DataObject();
            dataObject.SetImage(source);
            dataObject.SetData("FromMyApp", true);

            Clipboard.SetDataObject(dataObject);

            if (lastImage == _currentSelectionImage)
            {
                _clipboardImageIsFromApp = true;
            }
            else
            {
                _clipboardImageIsFromApp = false;
                _lastSelectionSize = null;
            }
        }

        private Image? _currentSelectionImage;
        private SelectionBox? _currentSelectionBox;
        private UIElement? _currentEditableElement;
        private Rectangle? _currentSelectionWhiteRect;

        public void StartSelection(Point startPoint, Canvas canvas)
        {
            if (_currentSelectionImage != null || _currentSelectionBox != null || _currentSelectionWhiteRect != null)
            {
                ConfirmChanges(canvas);
            }

            SaveState(canvas);

            _selectionStart = startPoint; 

            _selectionRect = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection { 4, 2 },
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(_selectionRect, _selectionStart.X);
            Canvas.SetTop(_selectionRect, _selectionStart.Y);
            canvas.Children.Add(_selectionRect);

            _isSelecting = true;
            Mouse.Capture(Application.Current.MainWindow, CaptureMode.SubTree);
        }

        public void UpdateSelection(Point currentPoint, Canvas canvas)
        {
            if (!_isSelecting || _selectionRect == null) return;

            double width = currentPoint.X - _selectionStart.X;
            double height = currentPoint.Y - _selectionStart.Y;

            _selectionRect.Width = Math.Abs(width);
            _selectionRect.Height = Math.Abs(height);
            Canvas.SetLeft(_selectionRect, width > 0 ? _selectionStart.X : currentPoint.X);
            Canvas.SetTop(_selectionRect, height > 0 ? _selectionStart.Y : currentPoint.Y);
        }

        public void EndSelection(Canvas canvas, double zoomScale)
        {
            Mouse.Capture(null);

            if (!_isSelecting || _selectionRect == null) return;

            var bounds = new Rect(
                Canvas.GetLeft(_selectionRect),
                Canvas.GetTop(_selectionRect),
                _selectionRect.Width,
                _selectionRect.Height
            );

            canvas.Children.Remove(_selectionRect);
            _selectionRect = null;
            _isSelecting = false;

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var renderWidth = (int)Math.Ceiling(bounds.Width * zoomScale);
            var renderHeight = (int)Math.Ceiling(bounds.Height * zoomScale);

            var rtb = new RenderTargetBitmap(
                renderWidth, renderHeight,
                96 * zoomScale, 96 * zoomScale,
                PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                var brush = new VisualBrush(canvas)
                {
                    Viewbox = new Rect(
                        bounds.X * zoomScale,
                        bounds.Y * zoomScale,
                        bounds.Width * zoomScale,
                        bounds.Height * zoomScale),
                    ViewboxUnits = BrushMappingMode.Absolute
                };

                dc.DrawRectangle(brush, null, new Rect(new Size(bounds.Width, bounds.Height)));
            }

            rtb.Render(dv);

            var whiteRect = new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,
                Fill = Brushes.White,
                IsHitTestVisible = false
            };
            Canvas.SetLeft(whiteRect, bounds.X);
            Canvas.SetTop(whiteRect, bounds.Y);

            var image = new Image
            {
                Source = rtb,
                Width = bounds.Width,
                Height = bounds.Height,
                Stretch = Stretch.Fill,
                IsEnabled = true
            };

            Canvas.SetLeft(image, bounds.X);
            Canvas.SetTop(image, bounds.Y);

            _lastSelectionSize = new Size(bounds.Width, bounds.Height);

            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            image.MouseMove += Image_MouseMove;
            image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
            image.MouseWheel += Image_MouseWheel;

            _currentSelectionWhiteRect = whiteRect;
            _currentSelectionImage = image;

            var selectionBox = new SelectionBox(image);
            _currentSelectionBox = selectionBox;

            if (_currentEditableElement != null)
                SetIsEditable(_currentEditableElement, false);

            _currentEditableElement = image;
            SetIsEditable(image, true);

            int insertIndex = canvas.Children.Count;
            canvas.Children.Insert(insertIndex, whiteRect);
            canvas.Children.Insert(insertIndex + 1, image);
            canvas.Children.Add(selectionBox);

            SaveState(canvas);
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
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

            _lastSelectionSize = null;
            _clipboardImageIsFromApp = false;

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

                double width = bitmap.Width;
                double height = bitmap.Height;

                var data = Clipboard.GetDataObject();
                if (data != null && data.GetDataPresent("FromMyApp") && data.GetData("FromMyApp") is bool fromApp && fromApp)
                {
                    _clipboardImageIsFromApp = true;
                }
                else
                {
                    _clipboardImageIsFromApp = false;
                    _lastSelectionSize = null;
                }

                if (_clipboardImageIsFromApp && _lastSelectionSize.HasValue)
                {
                    width = _lastSelectionSize.Value.Width;
                    height = _lastSelectionSize.Value.Height;
                }

                var image = new Image
                {
                    Source = bitmap,
                    Width = width,
                    Height = height,
                    Stretch = Stretch.Fill
                };

                Canvas.SetLeft(image, (canvas.ActualWidth - width) / 2);
                Canvas.SetTop(image, (canvas.ActualHeight - height) / 2);

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
            if (sender is Image image && GetIsEditable(image))
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
            if (sender is Image image && GetIsEditable(image))
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
            if (_isDragging && _draggedImage != null && GetIsEditable(_draggedImage))
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

            foreach (var element in canvas.Children)
            {
                if (element is Border border && border.Child is System.Windows.Controls.RichTextBox richTextBox && !richTextBox.IsReadOnly)
                    return true;

                if (element is SelectionBox)
                    return true;
            }

            return false;
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
                if (!element.IsHitTestVisible) continue;

                element.MouseWheel -= Image_MouseWheel;
                element.MouseLeftButtonDown -= Image_MouseLeftButtonDown;
                element.MouseMove -= Image_MouseMove;
                element.MouseLeftButtonUp -= Image_MouseLeftButtonUp;

                element.MouseWheel += Image_MouseWheel;
                element.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                element.MouseMove += Image_MouseMove;
                element.MouseLeftButtonUp += Image_MouseLeftButtonUp;
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

        private CanvasElementState[] _lastSavedState;

        public void SaveState(Canvas canvas)
        {
            if (canvas == null) return;

            double width = canvas.Width;
            double height = canvas.Height;

            var currentElements = canvas.Children
                .Cast<UIElement>()
                .Select(el =>
                {
                    bool? isTextEditable = null;

                    if (el is Border border && border.Child is System.Windows.Controls.RichTextBox rtb)
                        isTextEditable = !rtb.IsReadOnly;

                    return new CanvasElementState
                    {
                        Element = el,
                        IsHitTestVisible = el.IsHitTestVisible,
                        CanvasWidth = width,
                        CanvasHeight = height,
                        IsTextEditable = isTextEditable
                    };
                })
                .ToArray();

            if (_lastSavedState != null && AreStatesEqual(_lastSavedState, currentElements))
                return;

            _lastSavedState = currentElements;
            _undoElements.Push(currentElements);
            _redoElements.Clear();

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
        }

        private bool AreStatesEqual(CanvasElementState[] a, CanvasElementState[] b)
        {
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (!ReferenceEquals(a[i].Element, b[i].Element) ||
                    a[i].IsHitTestVisible != b[i].IsHitTestVisible ||
                    a[i].CanvasWidth != b[i].CanvasWidth ||
                    a[i].CanvasHeight != b[i].CanvasHeight ||
                    a[i].IsTextEditable != b[i].IsTextEditable)
                {
                    return false;
                }
            }

            return true;
        }

        private void Undo(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null || _undoElements.Count <= 1) return;

            var current = _undoElements.Pop();
            _redoElements.Push(current);

            var previous = _undoElements.Peek();
            RestoreCanvas(canvas, previous);

            _lastSavedState = previous;

            RestoreImageHandlers(canvas);

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private void Redo(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null || _redoElements.Count == 0) return;

            var next = _redoElements.Pop();
            _undoElements.Push(next);

            RestoreCanvas(canvas, next);

            _lastSavedState = next;

            RestoreImageHandlers(canvas);

            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
        }

        private void RestoreCanvas(Canvas canvas, CanvasElementState[] elements)
        {
            if (elements.Length > 0)
            {
                canvas.Width = elements[0].CanvasWidth;
                canvas.Height = elements[0].CanvasHeight;
            }

            canvas.Children.Clear();
            IsEditingText = false;
            IsEditingImage = false;

            foreach (var state in elements)
            {
                var element = state.Element;
                element.IsHitTestVisible = state.IsHitTestVisible;

                if (element is Border border && border.Child is System.Windows.Controls.RichTextBox rtb)
                {
                    if (state.IsTextEditable == true)
                    {
                        rtb.IsReadOnly = false;
                        rtb.Focusable = true;
                        rtb.Cursor = Cursors.IBeam;

                        rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        rtb.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

                        border.BorderBrush = Brushes.Gray;
                        border.BorderThickness = new Thickness(1);

                        AddBorderEventHandlers(border);
                        AutoResizeRichTextBox(rtb, border); 
                        IsEditingText = true;
                    }
                    else
                    {
                        border.MouseLeftButtonDown -= Border_MouseLeftButtonDown;
                        border.MouseMove -= Border_MouseMove;
                        border.MouseLeftButtonUp -= Border_MouseLeftButtonUp;

                        rtb.IsReadOnly = true;
                        rtb.Focusable = false;
                        rtb.Cursor = Cursors.Arrow;
                        rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        rtb.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                        border.BorderBrush = Brushes.Transparent;
                        border.BorderThickness = new Thickness(0);
                    }
                }

                if (element is Image img && !state.IsHitTestVisible)
                {
                    img.MouseLeftButtonDown -= Image_MouseLeftButtonDown;
                    img.MouseMove -= Image_MouseMove;
                    img.MouseLeftButtonUp -= Image_MouseLeftButtonUp;
                    img.MouseWheel -= Image_MouseWheel;
                }

                canvas.Children.Add(element);
            }

            ((RelayCommand)ConfirmChangesCommand).RaiseCanExecuteChanged();
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

                SaveState(canvas);
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
                var brushCursor = canvas.Children.OfType<Ellipse>().FirstOrDefault(e => e.Name == "BrushCursor");
                var originalVisibility = brushCursor?.Visibility ?? Visibility.Visible;
                if (brushCursor != null)
                    brushCursor.Visibility = Visibility.Hidden;

                try
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
                        System.Windows.MessageBox.Show("Canvas is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                finally
                {
                    if (brushCursor != null)
                        brushCursor.Visibility = originalVisibility;
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

                    paintCanvas.Focus();

                    SaveState(paintCanvas);
                }
            }
        }
    }
}