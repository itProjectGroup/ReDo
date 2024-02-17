using ReDo.CustomEvents;
using ReDo.Models;
using ReDo.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReDo.Services
{
    public class Recorder
    {
        static ArrayList instructions;
        private KeyboardHook KeyboardHook;
        public Recorder()
        {
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
                Console.WriteLine($"Received: {e.Instructions.KeyName} and {e.Instructions.KeyCode} ");
                instructions.Add(e.Instructions);
            }
        }

        public void StopRecording() { MouseHook.stop(); KeyboardHook.Dispose(); MessageBox.Show("Recording Stopped by user."); }

        private void HandleHookEvent(object sender, HookEventArgs e)
        {
            Console.WriteLine($"Received: {e.Instructions.X} and {e.Instructions.Y} ");
            instructions.Add(e.Instructions);
        }

        public void StartPlayBack()
        {
            ClickUtility clickUtility = new ClickUtility();
            foreach (Instructions instr in instructions)
            {
                if (instr != null && instr.Type == UtilityType.Click)
                {
                    clickUtility.PerformClick(instr.X, instr.Y);
                }
                else if (instr != null && instr.Type == UtilityType.Keys)
                {
                    //KeysUtility.SendKey(instr.KeyCode);
                    KeySender keySender = new KeySender();
                    keySender.sendKey(new IntPtr(), (short)instr.KeyCode);
                }

            }
        }
    }
}
