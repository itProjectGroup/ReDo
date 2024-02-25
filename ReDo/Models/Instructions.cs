using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.Models
{
    public interface IInstructions
    {
        UtilityType Type { get; set; }
    }

    public class MouseInstruction : IInstructions
    {
        public MouseInstruction(UtilityType type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public UtilityType Type { get; set; }
}

    public class KeyboardInstruction : IInstructions
    {
        public KeyboardInstruction(UtilityType type, int keyCode = -1, string keyName = null)
        {
            Type = type;
            KeyCode = keyCode;
            KeyName = keyName;
        }

        public int KeyCode { get; set; }
        public string KeyName { get; set; }
        public UtilityType Type { get; set; }
    }

    public class DelayInstruction : IInstructions
    {
        public DelayInstruction(UtilityType type, TimeSpan delay = default)
        {
            Type = type;
            Delay = delay;
        }

        public TimeSpan Delay { get; set; }
        public UtilityType Type { get; set; }
    }
}

