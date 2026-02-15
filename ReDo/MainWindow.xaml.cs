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
        RecordingOverlayWindow recordingOverlay;

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
            recorder.RecordingStopped += Recorder_RecordingStopped;
        }

        private void Recorder_RecordingStopped(object sender, EventArgs e)
        {
            isRecording = false;
            HideRecordingOverlay();
            WindowState = WindowState.Normal;
            mainViewModel.RefreshRecordedSteps(Recorder.instructions);
        }

        private void ShowRecordingOverlay()
        {
            if (recordingOverlay == null)
                recordingOverlay = new RecordingOverlayWindow();
            recordingOverlay.Show();
        }

        private void HideRecordingOverlay()
        {
            recordingOverlay?.Close();
            recordingOverlay = null;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (isRecording = !isRecording)
            {
                ShowRecordingOverlay();
                this.WindowState = WindowState.Minimized;
                this.recorder.StartRecording();
            }
            else
            {
                this.recorder.StopRecording();
                HideRecordingOverlay();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            isRecording = !isRecording;
            this.recorder.StopRecording();
            HideRecordingOverlay();
            this.WindowState = WindowState.Normal;
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
                    Recorder.instructions = instructionsList ?? new ArrayList();
                    mainViewModel.RefreshRecordedSteps(Recorder.instructions);
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
            if (e.Key == Key.Escape && isRecording)
            {
                isRecording = false;
                MouseHook.stop();
                MessageBox.Show("Recording Stopped by user.");
                HideRecordingOverlay();
                this.WindowState = WindowState.Normal;
            }
        }

        private void ReselectClick_OnClick(object sender, RoutedEventArgs e)
        {
            var step = (sender as System.Windows.Controls.Button)?.Tag as RecordedStepViewModel;
            if (step == null) return;
            var win = new Windows.ClickCaptureWindow();
            win.Owner = this;
            if (win.ShowDialog() == true && !win.Cancelled)
            {
                var instr = Recorder.instructions[step.InstructionIndex] as MouseInstruction;
                if (instr != null)
                {
                    instr.X = win.ResultX;
                    instr.Y = win.ResultY;
                    mainViewModel.RefreshRecordedSteps(Recorder.instructions);
                }
            }
        }

        private void WaitEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var step = (sender as System.Windows.Controls.Button)?.Tag as RecordedStepViewModel;
            if (step == null) return;
            var instr = Recorder.instructions[step.InstructionIndex] as DelayInstruction;
            if (instr == null) return;
            var win = new Windows.WaitEditWindow(step.DelayMilliseconds) { Owner = this };
            if (win.ShowDialog() == true && win.Confirmed)
            {
                instr.Delay = TimeSpan.FromMilliseconds(win.ResultMilliseconds);
                mainViewModel.RefreshRecordedSteps(Recorder.instructions);
            }
        }

        private void ChangeKey_OnClick(object sender, RoutedEventArgs e)
        {
            var step = (sender as System.Windows.Controls.Button)?.Tag as RecordedStepViewModel;
            if (step == null) return;
            var win = new Windows.KeyCaptureWindow { Owner = this };
            if (win.ShowDialog() == true && !win.Cancelled)
            {
                var instr = Recorder.instructions[step.InstructionIndex] as KeyboardInstruction;
                if (instr != null)
                {
                    instr.KeyCode = win.KeyCode;
                    instr.KeyName = win.KeyName;
                    mainViewModel.RefreshRecordedSteps(Recorder.instructions);
                }
            }
        }

        private void DeleteStep_OnClick(object sender, RoutedEventArgs e)
        {
            var step = (sender as System.Windows.Controls.Button)?.Tag as RecordedStepViewModel;
            if (step == null) return;
            if (step.InstructionIndex < 0 || step.InstructionIndex >= Recorder.instructions.Count) return;
            Recorder.instructions.RemoveAt(step.InstructionIndex);
            mainViewModel.RefreshRecordedSteps(Recorder.instructions);
        }
    }
}
