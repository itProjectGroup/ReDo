using ReDo.CustomEvents;
using ReDo.Services;
using ReDo.Utility;
using ReDo.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace ReDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool isRecording = false;
        MainWindowViewModel mainViewModel;
        public MainWindow()
        {
            mainViewModel = new MainWindowViewModel();
            mainViewModel.BtnData = new Models.UIElementBtnState { btnName = "Record", btnPath = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,9A3,3 0 0,1 15,12A3,3 0 0,1 12,15A3,3 0 0,1 9,12A3,3 0 0,1 12,9Z" };
            DataContext = mainViewModel;

            InitializeComponent();

            /*
             * Background Handlers
             */
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            var recorder = new Recorder();
            if (isRecording = !isRecording)
            {
                this.WindowState = WindowState.Minimized;
                recorder.StartRecording();
            }
            else
            {
                recorder.StopRecording();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            var recorder = new Recorder();
            isRecording = !isRecording;
            MessageBox.Show("Ending Recording.");
            recorder.StopRecording();
        }

        private void PlaybackButton_Click(object sender, EventArgs e)
        {
            var recorder = new Recorder();
            recorder.StartPlayBack();
        }

        void MyButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked");
        }

        private void MainWindow_KeyDown(Object sender, KeyEventArgs e)  //Escape to stop recording.
        {
            if (e.Key == Key.Escape)
            {
                MouseHook.stop();
                MessageBox.Show("Recording Stopped by user.");
                this.WindowState = WindowState.Minimized;
            }
        }
    }
}
