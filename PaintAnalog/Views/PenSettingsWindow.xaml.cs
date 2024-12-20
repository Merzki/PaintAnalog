using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PaintAnalog.Views
{
    public partial class PenSettingsWindow : Window
    {
        public string SelectedShape { get; set; }
        public Brush SelectedBrush { get; private set; }
        public double SelectedThickness { get; set; }

        public PenSettingsWindow(Brush currentBrush, double currentThickness, string currentShape)
        {
            InitializeComponent();

            SelectedBrush = currentBrush;
            SelectedThickness = currentThickness;
            SelectedShape = currentShape;

            ThicknessSlider.Value = currentThickness;

            foreach (ComboBoxItem item in ShapeComboBox.Items)
            {
                if ((string)item.Tag == currentShape)
                {
                    ShapeComboBox.SelectedItem = item;
                    break;
                }
            }

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
