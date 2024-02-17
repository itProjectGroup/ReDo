using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.Models
{
    public class Instructions
    {
        public Instructions(UtilityType type, int x = -1, int y = -1, string metData = null, int keyCode = -1, string keyName = null)
        {
            X = x; Y = y; Type = type; MetaData = metData; KeyCode = keyCode; KeyName = keyName;
        }
        public UtilityType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string MetaData { get; set; }
        public int KeyCode { get; set; }
        public string KeyName { get; set; }
    }
}
