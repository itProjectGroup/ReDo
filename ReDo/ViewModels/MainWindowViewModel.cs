using ReDo.Models;
using ReDo.Services.StepActions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ReDo.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            RecordedSteps = new ObservableCollection<RecordedStepViewModel>();
            AvailableStepActions = new ObservableCollection<StepActionItemViewModel>();
            LoadAvailableStepActions();
        }

        public ObservableCollection<StepActionItemViewModel> AvailableStepActions { get; }

        public ICollectionView FilteredStepActions { get; private set; }

        private string _stepActionSearchQuery = string.Empty;
        public string StepActionSearchQuery
        {
            get => _stepActionSearchQuery;
            set
            {
                if (_stepActionSearchQuery == value) return;
                _stepActionSearchQuery = value ?? string.Empty;
                OnPropertyChanged(nameof(StepActionSearchQuery));
                RefreshStepActionFilter();
            }
        }

        public bool ShowStepActionEmptyState =>
            !string.IsNullOrWhiteSpace(StepActionSearchQuery) && !HasFilteredStepActions;

        public bool HasFilteredStepActions
        {
            get
            {
                if (FilteredStepActions == null) return true;
                return FilteredStepActions.Cast<object>().Any();
            }
        }

        public void ClearStepActionSearch() => StepActionSearchQuery = string.Empty;

        private void LoadAvailableStepActions()
        {
            AvailableStepActions.Clear();
            foreach (var provider in StepActionRegistry.All)
            {
                AvailableStepActions.Add(new StepActionItemViewModel
                {
                    Id = provider.Id,
                    Title = provider.Title,
                    Description = provider.Description,
                    Icon = provider.Icon,
                    AccentColorHex = provider.AccentColorHex
                });
            }

            FilteredStepActions = CollectionViewSource.GetDefaultView(AvailableStepActions);
            FilteredStepActions.Filter = FilterStepAction;
        }

        private bool FilterStepAction(object item)
        {
            if (!(item is StepActionItemViewModel action))
                return false;

            if (string.IsNullOrWhiteSpace(StepActionSearchQuery))
                return true;

            var query = StepActionSearchQuery.Trim();
            return action.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                || action.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                || action.Id.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RefreshStepActionFilter()
        {
            FilteredStepActions?.Refresh();
            OnPropertyChanged(nameof(HasFilteredStepActions));
            OnPropertyChanged(nameof(ShowStepActionEmptyState));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<RecordedStepViewModel> _recordedSteps;
        public ObservableCollection<RecordedStepViewModel> RecordedSteps
        {
            get => _recordedSteps;
            set { _recordedSteps = value; OnPropertyChanged(nameof(RecordedSteps)); OnPropertyChanged(nameof(HasRecordedSteps)); }
        }

        public bool HasRecordedSteps => RecordedStepsCount > 0;

        private int _recordedStepsCount;
        public int RecordedStepsCount
        {
            get => _recordedStepsCount;
            set { _recordedStepsCount = value; OnPropertyChanged(nameof(RecordedStepsCount)); OnPropertyChanged(nameof(HasRecordedSteps)); }
        }

        public UIElementBtnState BtnStop
        { get { return new UIElementBtnState { btnName = "Stop", btnPath = "M14 10H3V12H14V10M14 6H3V8H14V6M3 16H10V14H3V16M21.5 11.5L23 13L16 20L11.5 15.5L13 14L16 17L21.5 11.5Z" }; } }

        public UIElementBtnState BtnPlayback
        { get { return new UIElementBtnState { btnName = "PlayBack", btnPath = "M4,2A2,2 0 0,0 2,4V14H4V4H14V2H4M8,6A2,2 0 0,0 6,8V18H8V8H18V6H8M20,12V20H12V12H20M20,10H12A2,2 0 0,0 10,12V20A2,2 0 0,0 12,22H20A2,2 0 0,0 22,20V12A2,2 0 0,0 20,10M14,13V19L18,16L14,13Z" }; } }


        private UIElementBtnState _btnData;
        public UIElementBtnState BtnData
        {
            get { return _btnData; }
            set
            {
                _btnData = value;
                OnPropertyChanged("PathData");
            }
        }

        private string _btnState;

        public string BtnState
        {
            get { return _btnState; }
            set
            {
                _btnState = value;
                OnPropertyChanged("PathData");
            }
        }


        /// <summary>Rebuilds the recorded steps list from the given instructions (e.g. Recorder.instructions).</summary>
        public void RefreshRecordedSteps(IList instructions)
        {
            RecordedSteps.Clear();
            if (instructions == null) return;
            int displayIndex = 0;
            for (int i = 0; i < instructions.Count; i++)
            {
                var item = instructions[i];
                if (item == null) continue;
                displayIndex++;
                if (item is MouseInstruction mouse)
                {
                    bool isDoubleClick = mouse.Type == UtilityType.DoubleClick;

                    RecordedSteps.Add(new RecordedStepViewModel
                    {
                        Index = displayIndex,
                        InstructionIndex = i,
                        TypeLabel = isDoubleClick ? "Double Click" : "Click", 
                        StepType = isDoubleClick ? "DoubleClick" : "Click",
                        Description = isDoubleClick
                            ? $"Double Click at ({mouse.X}, {mouse.Y})"
                            : $"Click at ({mouse.X}, {mouse.Y})"
                    });
                }
                else if (item is ImageClickInstruction imageClick)
                {
                    bool isDoubleClickImage = imageClick.Type == UtilityType.ImageDoubleClick;
                    string label = string.IsNullOrEmpty(imageClick.Label)
                        ? $"{imageClick.TemplateWidth}×{imageClick.TemplateHeight} px"
                        : imageClick.Label;
                    RecordedSteps.Add(new RecordedStepViewModel
                    {
                        Index = displayIndex,
                        InstructionIndex = i,
                        TypeLabel = isDoubleClickImage ? "DoubleImageClick" : "Image",
                        StepType = isDoubleClickImage? "DoubleImageClick" :"ImageClick",
                        Description = $"Click image ({label})",
                        ImagePreviewBase64 = imageClick.ImageBase64
                    });
                }
                else if (item is KeyboardInstruction key)
                {
                    string keyDisplay = string.IsNullOrEmpty(key.KeyName) ? $"Key code {key.KeyCode}" : key.KeyName;
                    RecordedSteps.Add(new RecordedStepViewModel
                    {
                        Index = displayIndex,
                        InstructionIndex = i,
                        TypeLabel = "Key",
                        StepType = "Key",
                        Description = keyDisplay
                    });
                }
                else if (item is DelayInstruction delay)
                {
                    double ms = delay.Delay.TotalMilliseconds;
                    RecordedSteps.Add(new RecordedStepViewModel
                    {
                        Index = displayIndex,
                        InstructionIndex = i,
                        TypeLabel = "Wait",
                        StepType = "Delay",
                        Description = ms < 1000 ? $"{ms:F0} ms" : $"{delay.Delay.TotalSeconds:F2} s",
                        DelayMilliseconds = ms
                    });
                }
            }
            RecordedStepsCount = RecordedSteps.Count;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
