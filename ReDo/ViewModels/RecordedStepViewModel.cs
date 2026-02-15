using System;

namespace ReDo.ViewModels
{
    /// <summary>
    /// Display model for a single recorded step in the UI.
    /// </summary>
    public class RecordedStepViewModel
    {
        public int Index { get; set; }
        public string TypeLabel { get; set; }
        public string Description { get; set; }
        /// <summary>For styling: "Click", "Key", "Delay".</summary>
        public string StepType { get; set; }
    }
}
