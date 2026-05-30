using ReDo.Models;
using System.Windows;

namespace ReDo.Services.StepActions
{
    /// <summary>Host surface passed to step action providers (dialogs, instruction list).</summary>
    public interface IStepActionContext
    {
        Window OwnerWindow { get; }
        void AppendInstruction(IInstructions instruction);
        void RefreshSteps();
        void MinimizeForCapture();
        void RestoreAfterCapture();
    }
}
