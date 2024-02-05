using ReDo.CustomEvents;
using ReDo.Services;
using ReDo.Utility;
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
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var recorder = new Recorder();
            if (isRecording = !isRecording)
            {
                recorder.StartRecording();
            }
            else
            {
                recorder.StopRecording();
            }
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
            }
        }
    }
}
