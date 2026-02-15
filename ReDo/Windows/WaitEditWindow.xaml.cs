using System;
using System.Windows;

namespace ReDo.Windows
{
    public partial class WaitEditWindow : Window
    {
        public double ResultMilliseconds { get; private set; }
        public bool Confirmed { get; private set; }

        public WaitEditWindow(double currentMs)
        {
            InitializeComponent();
            TbMilliseconds.Text = currentMs.ToString("F0", System.Globalization.CultureInfo.InvariantCulture);
            TbMilliseconds.SelectAll();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(TbMilliseconds.Text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double ms) && ms >= 0)
            {
                ResultMilliseconds = ms;
                Confirmed = true;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a valid number (≥ 0).", "Invalid value", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
