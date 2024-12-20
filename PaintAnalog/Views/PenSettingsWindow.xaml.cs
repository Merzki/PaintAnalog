using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PaintAnalog.Views
{
    public partial class PenSettingsWindow : Window
    {
        public string SelectedShape { get; set; } = "Polyline";
        public Brush SelectedBrush { get; private set; } = Brushes.Black;
        public double SelectedThickness { get; private set; } = 2.0;

        public PenSettingsWindow(Brush currentBrush, double currentThickness)
        {
            InitializeComponent();

            SelectedBrush = currentBrush;
            SelectedThickness = currentThickness;

            ThicknessSlider.Value = currentThickness;

            DataContext = this;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedThickness = ThicknessSlider.Value;
            SelectedShape = (ShapeComboBox.SelectedItem as ComboBoxItem)?.Tag as string ?? "Polyline";

            DialogResult = true;
            Close();
        }
    }
}
