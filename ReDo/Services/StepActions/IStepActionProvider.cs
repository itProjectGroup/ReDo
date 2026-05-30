using FontAwesome.WPF;

namespace ReDo.Services.StepActions
{
    /// <summary>
    /// Describes one action that can be appended to the automation from the add-step menu.
    /// Register new implementations in <see cref="StepActionRegistry"/> to extend the menu.
    /// </summary>
    public interface IStepActionProvider
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        FontAwesomeIcon Icon { get; }
        /// <summary>Hex color for the action badge (e.g. #0D9488).</summary>
        string AccentColorHex { get; }

        /// <returns>True if a step was added to the automation.</returns>
        bool TryExecute(IStepActionContext context);
    }
}
