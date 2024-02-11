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
        ArrayList instructions;
        public Recorder()
        {
            instructions = new System.Collections.ArrayList();
        }

        public void StartRecording()
        {
            try
            {
                MouseHook.Start();
                MouseHook.MouseAction += new HookEventHandler(HandleHookEvent);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed: " + ex);
            }
        }

        public void StopRecording() { MouseHook.stop(); MessageBox.Show("Recording Stopped by user."); }

        //private void Event(object sender, EventArgs e) => Console.WriteLine("Left mouse click!");

        private void HandleHookEvent(object sender, HookEventArgs e)
        {
            Console.WriteLine($"Received: {e.Instructions.X} and {e.Instructions.Y} ");
            instructions.Add(e.Instructions);
        }

        public void StartPlayBack()
        {
            foreach (Instructions instr in instructions)
            {
                if (instr != null && instr.Type == UtilityType.Click)
                {
                    ClickUtility clickUtility = new ClickUtility();
                    clickUtility.PerformClick(instr.X, instr.Y);
                }

                //TODO: Add KeyBoard Interaction.
            }
        }
    }
}
