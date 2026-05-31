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

    /// <summary>
    /// Click step that finds a UI element by matching a captured image template on screen,
    /// then clicks its center (similar to UiPath "Click Image" activity).
    /// </summary>
    public class ImageClickInstruction : IInstructions
    {
        public ImageClickInstruction(
            UtilityType type,
            string imageBase64,
            int templateWidth,
            int templateHeight,
            double matchThreshold = 0.65, //updated threshold to 0.65
            string label = null)
        {
            Type = type;
            ImageBase64 = imageBase64;
            TemplateWidth = templateWidth;
            TemplateHeight = templateHeight;
            MatchThreshold = matchThreshold;
            Label = label;
        }

        /// <summary>PNG template image encoded as base64 for JSON export/import.</summary>
        public string ImageBase64 { get; set; }

        public int TemplateWidth { get; set; }
        public int TemplateHeight { get; set; }

        /// <summary>Minimum normalized cross-correlation score (0–1) to accept a match.</summary>
        public double MatchThreshold { get; set; }

        /// <summary>Optional display name for the captured element.</summary>
        public string Label { get; set; }

        public UtilityType Type { get; set; }
    }
}


