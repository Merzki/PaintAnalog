using System.Windows;

namespace PaintAnalog.Views
{
    public partial class ToolsWindow : Window
    {
        public string SelectedTool { get; private set; }

        public ToolsWindow()
        {
            InitializeComponent();
        }

        private void BrushSelected(object sender, RoutedEventArgs e)
        {
            SelectedTool = "Brush";
            DialogResult = true;
            Close();
        }

        private void EraserSelected(object sender, RoutedEventArgs e)
        {
            SelectedTool = "Eraser";
            DialogResult = true;
            Close();
        }

        private void FillSelected(object sender, RoutedEventArgs e) 
        {
            SelectedTool = "Fill";
            DialogResult = true;
            Close();
        }
    }
}
