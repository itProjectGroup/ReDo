using System.Windows;
using System.Windows.Input;

namespace ReDo.Windows
{
    /// <summary>
    /// Small dialog that captures one key press and returns key code and name.
    /// </summary>
    public partial class KeyCaptureWindow : Window
    {
        public int KeyCode { get; private set; }
        public string KeyName { get; private set; }
        public bool Cancelled { get; private set; } = true;

        public KeyCaptureWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                return;
            }
            KeyCode = (int)KeyInterop.VirtualKeyFromKey(e.Key);
            KeyName = e.Key.ToString();
            Cancelled = false;
            DialogResult = true;
            Close();
        }
    }
}
