using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.Models
{
    public class Instructions
    {
        public UtilityType Type { get; set; }
    }

    public class MouseInstruction : Instructions
    {
        public MouseInstruction(UtilityType type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class KeyboardInstruction : Instructions
    {
        public KeyboardInstruction(UtilityType type, int keyCode = -1, string keyName = null)
        {
            Type = type;
            KeyCode = keyCode;
            KeyName = keyName;
        }

        public int KeyCode { get; set; }
        public string KeyName { get; set; }
    }

    public class DelayInstruction : Instructions
    {
        public DelayInstruction(UtilityType type, TimeSpan delay = default)
        {
            Type = type;
            Delay = delay;
        }

        public TimeSpan Delay { get; set; }
    }
}

