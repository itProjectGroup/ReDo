using ReDo.CustomEvents;
using ReDo.Models;
using ReDo.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
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
                if (instr != null && instr is MouseInstruction mInstr )
                {
                    clickUtility.PerformClick(mInstr.X, mInstr.Y);
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
    }
}
