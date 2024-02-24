using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace ReDo.Utility
{
    class KeySender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetMessageExtraInfo();

        private enum InputType
        {
            INPUT_MOUSE,
            INPUT_KEYBOARD,
            INPUT_HARDWARE
        }
        [Flags]
        private enum MOUSEEVENTF
        {
            MOVE = 1,
            LEFTDOWN = 2,
            LEFTUP = 4,
            RIGHTDOWN = 8,
            RIGHTUP = 16,
            MIDDLEDOWN = 32,
            MIDDLEUP = 64,
            XDOWN = 128,
            XUP = 256,
            WHEEL = 2048,
            VIRTUALDESK = 16384,
            ABSOLUTE = 32768
        }
        [Flags]
        private enum KEYEVENTF
        {
            EXTENDEDKEY = 1,
            KEYUP = 2,
            UNICODE = 4,
            SCANCODE = 8
        }
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        public static uint KeyPress(short key)
        {
            INPUT iNPUT = default(INPUT);
            iNPUT.type = 1;
            iNPUT.ki.wScan = (short)MapVirtualKey((uint)key, 0u);
            iNPUT.ki.time = 0;
            iNPUT.ki.dwFlags = 8;
            INPUT[] pInputs = new INPUT[]
				{
					iNPUT
				};
            return SendInput(1u, pInputs, Marshal.SizeOf(iNPUT));
        }

        public static uint KeyRelease(short key)
        {
            if(key != 20)
            {
                INPUT iNPUT = default(INPUT);
                iNPUT.type = 1;
                iNPUT.ki.wScan = (short)MapVirtualKey((uint)key, 0u);
                iNPUT.ki.time = 0;
                iNPUT.ki.dwFlags = 8;
                iNPUT.ki.dwFlags = 2;
                INPUT[] pInputs = new INPUT[]
                    {
                    iNPUT
                    };
                return SendInput(1u, pInputs, Marshal.SizeOf(iNPUT));
            }
            return 0;
        }

        public bool sendKey(short key)
        {
            //string keyChar = key;//((char)key).ToString().ToUpper();
            short vKey = key;

            //switch (keyChar)
            //{
            //    case "1":
            //        vKey = (short)VirtualKeyCode.VK_1;
            //        break;
            //    case "2":
            //        vKey = (short)VirtualKeyCode.VK_2;
            //        break;
            //    case "3":
            //        vKey = (short)VirtualKeyCode.VK_3;
            //        break;
            //    case "4":
            //        vKey = (short)VirtualKeyCode.VK_4;
            //        break;
            //    case "5":
            //        vKey = (short)VirtualKeyCode.VK_5;
            //        break;
            //    case "6":
            //        vKey = (short)VirtualKeyCode.VK_6;
            //        break;
            //    case "7":
            //        vKey = (short)VirtualKeyCode.VK_7;
            //        break;
            //    case "8":
            //        vKey = (short)VirtualKeyCode.VK_8;
            //        break;
            //    case "9":
            //        vKey = (short)VirtualKeyCode.VK_9;
            //        break;
            //    case "0":
            //        vKey = (short)VirtualKeyCode.VK_0;
            //        break;
            //    case "A":
            //        vKey = (short)VirtualKeyCode.VK_A;
            //        break;
            //    case "B":
            //        vKey = (short)VirtualKeyCode.VK_B;
            //        break;
            //    case "C":
            //        vKey = (short)VirtualKeyCode.VK_C;
            //        break;
            //    case "D":
            //        vKey = (short)VirtualKeyCode.VK_D;
            //        break;
            //    case "E":
            //        vKey = (short)VirtualKeyCode.VK_E;
            //        break;
            //    case "F":
            //        vKey = (short)VirtualKeyCode.VK_F;
            //        break;
            //    case "G":
            //        vKey = (short)VirtualKeyCode.VK_G;
            //        break;
            //    case "H":
            //        vKey = (short)VirtualKeyCode.VK_H;
            //        break;
            //    case "I":
            //        vKey = (short)VirtualKeyCode.VK_I;
            //        break;
            //    case "J":
            //        vKey = (short)VirtualKeyCode.VK_J;
            //        break;
            //    case "K":
            //        vKey = (short)VirtualKeyCode.VK_K;
            //        break;
            //    case "L":
            //        vKey = (short)VirtualKeyCode.VK_L;
            //        break;
            //    case "M":
            //        vKey = (short)VirtualKeyCode.VK_M;
            //        break;
            //    case "N":
            //        vKey = (short)VirtualKeyCode.VK_N;
            //        break;
            //    case "O":
            //        vKey = (short)VirtualKeyCode.VK_O;
            //        break;
            //    case "P":
            //        vKey = (short)VirtualKeyCode.VK_P;
            //        break;
            //    case "Q":
            //        vKey = (short)VirtualKeyCode.VK_Q;
            //        break;
            //    case "R":
            //        vKey = (short)VirtualKeyCode.VK_R;
            //        break;
            //    case "S":
            //        vKey = (short)VirtualKeyCode.VK_S;
            //        break;
            //    case "T":
            //        vKey = (short)VirtualKeyCode.VK_T;
            //        break;
            //    case "U":
            //        vKey = (short)VirtualKeyCode.VK_U;
            //        break;
            //    case "V":
            //        vKey = (short)VirtualKeyCode.VK_V;
            //        break;
            //    case "W":
            //        vKey = (short)VirtualKeyCode.VK_W;
            //        break;
            //    case "X":
            //        vKey = (short)VirtualKeyCode.VK_X;
            //        break;
            //    case "Y":
            //        vKey = (short)VirtualKeyCode.VK_Y;
            //        break;
            //    case "Z":
            //        vKey = (short)VirtualKeyCode.VK_Z;
            //        break;
            //    default: return false;
            //}

            //vKey = 0x51;

            KeyPress(vKey);
            Thread.Sleep(50);
            KeyRelease(vKey);
            Thread.Sleep(50);

            return true;
        }
    }
}
