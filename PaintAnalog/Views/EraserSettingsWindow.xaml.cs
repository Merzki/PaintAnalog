using System.Windows;

namespace PaintAnalog.Views
{
    public partial class EraserSettingsWindow : Window
    {
        public double SelectedThickness { get; private set; }

        public EraserSettingsWindow(double currentThickness)
        {
            InitializeComponent();
            ThicknessSlider.Value = currentThickness;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            SelectedThickness = ThicknessSlider.Value;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
