using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.Models
{
    public class Instructions
    {
        public Instructions(UtilityType type, int x, int y, string information = null) {
            X = x; Y = y;Type = type; Information = information;
        }
        public UtilityType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Information { get; set; }
    }
}
