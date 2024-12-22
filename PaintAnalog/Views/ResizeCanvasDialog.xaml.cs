using System.Windows;

namespace PaintAnalog
{
    public partial class ResizeCanvasDialog : Window
    {
        public double NewWidth { get; private set; }
        public double NewHeight { get; private set; }

        public ResizeCanvasDialog(double currentWidth, double currentHeight)
        {
            InitializeComponent();

            WidthBox.Text = currentWidth.ToString();
            HeightBox.Text = currentHeight.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(WidthBox.Text, out double width) &&
                double.TryParse(HeightBox.Text, out double height))
            {
                NewWidth = width;
                NewHeight = height;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid dimensions! Please enter valid numbers");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
