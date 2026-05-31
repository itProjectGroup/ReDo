using ReDo.CustomEvents;
using ReDo.Models;
using ReDo.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReDo.Services
{
    public class Recorder
    {
        public static ArrayList instructions;
        private KeyboardHook KeyboardHook;
        private TimerService timerService;

        /// <summary>Raised when recording is stopped (e.g. by Escape or Stop).</summary>
        public event EventHandler RecordingStopped;

        public Recorder()
        {
            timerService = new TimerService();
            instructions = new System.Collections.ArrayList();
            KeyboardHook = new KeyboardHook();
        }

        public void StartRecording()
        {
            try
            {
                MouseHook.Start();
                MouseHook.MouseAction += new MouseHookEventHandler(HandleHookEvent);
                KeyboardHook.KeyIntercepted += KeyboardHook_KeyIntercepted;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed: " + ex);
            }
        }

        private void KeyboardHook_KeyIntercepted(KeyboardHook.KeyboardHookEventArgs e)
        {
            if (e.KeyName == "Escape" && e.KeyCode == 27)
            {
                StopRecording();
            }
            else
            {
                var elapsedDelay = timerService.ToggleTimerState();
                if (elapsedDelay.HasValue)
                    instructions.Add(new DelayInstruction(UtilityType.Delay, elapsedDelay.Value));
                Console.WriteLine($"Received: {e.Instructions.KeyName} and {e.Instructions.KeyCode} ");
                instructions.Add(e.Instructions);
            }
        }

        public void StopRecording()
        {
            MouseHook.stop();
            KeyboardHook.Dispose();
            RecordingStopped?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Recording Stopped by user.");
        }

        private void HandleHookEvent(object sender, HookEventArgs e)
        {
            var elapsedDelay = timerService.ToggleTimerState();
            if (elapsedDelay.HasValue)
                instructions.Add(new DelayInstruction(UtilityType.Delay, elapsedDelay.Value));
            Console.WriteLine($"Received: {e.Instructions.X} and {e.Instructions.Y} ");
            instructions.Add(e.Instructions);
        }

        public void StartPlayBack()
        {
            ClickUtility clickUtility = new ClickUtility();
            foreach (IInstructions instr in instructions)
            {
                if (instr != null && instr is MouseInstruction mInstr)
                {
                    if (mInstr.Type == UtilityType.DoubleClick)
                    {
                        clickUtility.PerformDoubleClick(mInstr.X, mInstr.Y);
                    }
                    else
                    {
                        clickUtility.PerformClick(mInstr.X, mInstr.Y);
                    }
                }
                else if (instr != null && instr is ImageClickInstruction imgInstr)
                {
                    if (imgInstr.Type == UtilityType.ImageClick || imgInstr.Type == UtilityType.ImageDoubleClick)
                    {

                          PerformImageClick(clickUtility, imgInstr);
                    }
                    else
                    {

                    }
                }
                else if (instr != null && instr is KeyboardInstruction kInstr)
                {
                    KeySender keySender = new KeySender();
                    keySender.sendKey((short)kInstr.KeyCode);
                }
                else if (instr != null && instr is DelayInstruction dInstr)
                {
                    Thread.Sleep((int)dInstr.Delay.TotalMilliseconds);
                }
               
            }
            MessageBox.Show("Playback - All Instructions Complete");
        }

        private void PerformImageClick(ClickUtility clickUtility, ImageClickInstruction instruction)
        {
            const int maxAttempts = 5;
            const int retryDelayMs = 500;

            Bitmap template;
            try
            {
                template = ScreenCapture.Base64PngToBitmap(instruction.ImageBase64);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid image template: {ex.Message}", "Image click failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (template)
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    using (var screen = ScreenCapture.CaptureVirtualScreen())
                    {
                        var match = ImageMatcher.FindTemplate(screen, template, instruction.MatchThreshold);
                        if (match != null)
                        {
                            if (instruction.Type == UtilityType.ImageClick)
                            {
                                clickUtility.PerformClick(match.CenterX, match.CenterY);
                            }
                            else
                            {
                                clickUtility.PerformDoubleClick(match.CenterX, match.CenterY);
                            }
                                return;
                        }
                    }

                    if (attempt < maxAttempts)
                        Thread.Sleep(retryDelayMs);
                }
            }

            string label = string.IsNullOrEmpty(instruction.Label) ? "image element" : instruction.Label;
            MessageBox.Show(
                $"Could not find \"{label}\" on screen after {maxAttempts} attempts.\n" +
                $"Try recapturing the image or lowering the match threshold.",
                "Image click failed",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
