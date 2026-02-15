using System.Windows;
using System.Windows.Input;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Windows.Media.Animation;

namespace ReDo.Windows
{
    /// <summary>
    /// Full-screen overlay that captures one mouse click and returns screen coordinates.
    /// </summary>
    public partial class ClickCaptureWindow : Window
    {
        public int ResultX { get; private set; }
        public int ResultY { get; private set; }
        public bool Cancelled { get; private set; } = true;

        public ClickCaptureWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Left = SystemParameters.VirtualScreenLeft;
                Top = SystemParameters.VirtualScreenTop;
                Width = SystemParameters.VirtualScreenWidth;
                Height = SystemParameters.VirtualScreenHeight;
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var screenPos = PointToScreen(e.GetPosition(this));
            ResultX = (int)screenPos.X;
            ResultY = (int)screenPos.Y;
            Cancelled = false;
            DialogResult = true;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                return;
            }
        }
    }
}
