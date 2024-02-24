using ReDo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.CustomEvents
{
    public class HookEventArgs : EventArgs
    {
        public HookEventArgs(MouseInstruction data)
        {
            Instructions = data;
        }

        public MouseInstruction Instructions { get; set; }
    }

    public delegate void MouseHookEventHandler(object sender, HookEventArgs e);
}
