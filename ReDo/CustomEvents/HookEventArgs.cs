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
        public HookEventArgs(Instructions data)
        {
            Instructions = data;
        }

        public Instructions Instructions { get; set; }
    }

    public delegate void HookEventHandler(object sender, HookEventArgs e);
}
