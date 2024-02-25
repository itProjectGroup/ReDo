using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReDo.CustomEvents;
using ReDo.Models;
using ReDo.Services;
using ReDo.Utility;
using ReDo.ViewModels;
using ReDo.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
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
        Recorder recorder;
        public MainWindow()
        {
            mainViewModel = new MainWindowViewModel();
            mainViewModel.BtnData = new Models.UIElementBtnState { btnName = "Record", btnPath = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,9A3,3 0 0,1 15,12A3,3 0 0,1 12,15A3,3 0 0,1 9,12A3,3 0 0,1 12,9Z" };
            DataContext = mainViewModel;

            /*
             * Placement of the window
             */
            SizeChanged += (o, e) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };

            InitializeComponent();

            /*
             * Background Handlers
             */
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            recorder = new Recorder();

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (isRecording = !isRecording)
            {
                this.WindowState = WindowState.Minimized;
                this.recorder.StartRecording();
            }
            else
            {
                this.recorder.StopRecording();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            isRecording = !isRecording;
            this.recorder.StopRecording();
            this.WindowState = WindowState.Maximized;
        }

        private void PlaybackButton_Click(object sender, EventArgs e)
        {
            this.recorder.StartPlayBack();
        }

        void Import_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    ArrayList instructionsList = JsonConvert.DeserializeObject<ArrayList>(jsonContent, jsonSerializerSettings);
                    MessageBox.Show("JSON data loaded successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading JSON data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private T GetMethod<T>(IInstructions model) where T : IInstructions
        {
            var json = JsonConvert.SerializeObject(model);
            return JsonConvert.DeserializeObject<T>(json);
        }

        void Export_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    string jsonContent = JsonConvert.SerializeObject(Recorder.instructions, jsonSerializerSettings);
                    File.WriteAllText(filePath, jsonContent);

                    MessageBox.Show("Automation saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving JSON data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void OnExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
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
