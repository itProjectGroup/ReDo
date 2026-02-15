using System.Windows;

namespace ReDo.Windows
{
    /// <summary>
    /// Full-screen overlay that draws a red border to indicate recording is active.
    /// IsHitTestVisible=false so all mouse/keyboard input passes through to the desktop.
    /// </summary>
    public partial class RecordingOverlayWindow : Window
    {
        public RecordingOverlayWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => CoverAllScreens();
        }

        private void CoverAllScreens()
        {
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }
    }
}
