using ReDo.Models;
using ReDo.Services;
using ReDo.ViewModels;
using System.Windows;

namespace ReDo.Services.StepActions
{
    public class MainWindowStepActionContext : IStepActionContext
    {
        private readonly Window _owner;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowStepActionContext(Window owner, MainWindowViewModel viewModel)
        {
            _owner = owner;
            _viewModel = viewModel;
        }

        public Window OwnerWindow => _owner;

        public void AppendInstruction(IInstructions instruction)
        {
            Recorder.instructions.Add(instruction);
            RefreshSteps();
        }

        public void RefreshSteps() => _viewModel.RefreshRecordedSteps(Recorder.instructions);

        public void MinimizeForCapture() => _owner.WindowState = WindowState.Minimized;

        public void RestoreAfterCapture() => _owner.WindowState = WindowState.Normal;
    }
}
