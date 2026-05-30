using ReDo.Utility;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReDo.Windows
{
    /// <summary>
    /// Full-screen overlay where the user drags a rectangle to capture a template image
    /// for image-based click targeting (UiPath-style "Indicate on screen").
    /// </summary>
    public partial class ImageCaptureWindow : Window
    {
        private const int MinSelectionSize = 8;

        private bool _isSelecting;
        private System.Windows.Point _startPoint;
        private System.Windows.Point _currentPoint;

        public string ResultImageBase64 { get; private set; }
        public int ResultWidth { get; private set; }
        public int ResultHeight { get; private set; }
        public bool Cancelled { get; private set; } = true;

        public ImageCaptureWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isSelecting = true;
            _startPoint = e.GetPosition(SelectionCanvas);
            _currentPoint = _startPoint;
            UpdateSelectionRect();
            SelectionRect.Visibility = Visibility.Visible;
            CaptureMouse();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSelecting)
                return;

            _currentPoint = e.GetPosition(SelectionCanvas);
            UpdateSelectionRect();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isSelecting)
                return;

            _isSelecting = false;
            ReleaseMouseCapture();

            var topLeft = PointToScreen(new System.Windows.Point(
                Math.Min(_startPoint.X, _currentPoint.X),
                Math.Min(_startPoint.Y, _currentPoint.Y)));
            var bottomRight = PointToScreen(new System.Windows.Point(
                Math.Max(_startPoint.X, _currentPoint.X),
                Math.Max(_startPoint.Y, _currentPoint.Y)));

            int width = (int)Math.Round(bottomRight.X - topLeft.X);
            int height = (int)Math.Round(bottomRight.Y - topLeft.Y);

            if (width < MinSelectionSize || height < MinSelectionSize)
            {
                HintText.Text = "Selection too small — drag a larger area (Esc to cancel)";
                SelectionRect.Visibility = Visibility.Collapsed;
                return;
            }

            try
            {
                using (var bitmap = ScreenCapture.CaptureRegion((int)topLeft.X, (int)topLeft.Y, width, height))
                {
                    ResultImageBase64 = ScreenCapture.BitmapToBase64Png(bitmap);
                    ResultWidth = width;
                    ResultHeight = height;
                }

                Cancelled = false;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to capture image: {ex.Message}", "Capture error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                SelectionRect.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateSelectionRect()
        {
            double x = Math.Min(_startPoint.X, _currentPoint.X);
            double y = Math.Min(_startPoint.Y, _currentPoint.Y);
            double w = Math.Abs(_currentPoint.X - _startPoint.X);
            double h = Math.Abs(_currentPoint.Y - _startPoint.Y);

            Canvas.SetLeft(SelectionRect, x);
            Canvas.SetTop(SelectionRect, y);
            SelectionRect.Width = w;
            SelectionRect.Height = h;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
