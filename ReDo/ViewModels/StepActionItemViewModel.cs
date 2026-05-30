using FontAwesome.WPF;

namespace ReDo.ViewModels
{
    /// <summary>Display model for one entry in the add-step action menu.</summary>
    public class StepActionItemViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public FontAwesomeIcon Icon { get; set; }
        public string AccentColorHex { get; set; }
    }
}
