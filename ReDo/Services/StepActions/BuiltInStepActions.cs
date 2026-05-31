using FontAwesome.WPF;
using ReDo.Models;
using ReDo.Windows;
using System;

namespace ReDo.Services.StepActions
{
    /// <summary>Built-in step actions shown in the add-step menu.</summary>
    public static class BuiltInStepActions
    {
        public static void RegisterAll()
        {
            StepActionRegistry.Register(new CoordinateClickAction());
            StepActionRegistry.Register(new ImageClickAction());
            StepActionRegistry.Register(new KeyPressAction());
            StepActionRegistry.Register(new WaitAction());
            StepActionRegistry.Register(new CoordinateDoubleClickAction());
            StepActionRegistry.Register(new ImageDoubleClickAction());
        }

        private sealed class CoordinateClickAction : IStepActionProvider
        {
            public string Id => "click";
            public string Title => "Click";
            public string Description => "Click at a screen position (x, y)";
            public FontAwesomeIcon Icon => FontAwesomeIcon.Crosshairs;
            public string AccentColorHex => "#0D9488";

            public bool TryExecute(IStepActionContext context)
            {
                context.MinimizeForCapture();
                var win = new ClickCaptureWindow { Owner = context.OwnerWindow };
                try
                {
                    if (win.ShowDialog() == true && !win.Cancelled)
                    {
                        context.AppendInstruction(new MouseInstruction(UtilityType.Click, win.ResultX, win.ResultY));
                        return true;
                    }
                }
                finally
                {
                    context.RestoreAfterCapture();
                }
                return false;
            }
        }
        private sealed class CoordinateDoubleClickAction : IStepActionProvider
        {
            public string Id => "double-click";
            public string Title => " Double Click";
            public string Description => "Double Click at a screen position (x, y)";
            public FontAwesomeIcon Icon => FontAwesomeIcon.Crosshairs;
            public string AccentColorHex => "#0D2494";
            public bool TryExecute(IStepActionContext context)
            {
                context.MinimizeForCapture();
                var win = new ClickCaptureWindow { Owner = context.OwnerWindow };
                try
                {
                    if (win.ShowDialog() == true && !win.Cancelled)
                    {
                        context.AppendInstruction(new MouseInstruction(UtilityType.DoubleClick, win.ResultX, win.ResultY));
                        return true;
                    }
                }
                finally
                {
                    context.RestoreAfterCapture();
                }
                return false;

            }
        }
        private sealed class ImageClickAction : IStepActionProvider
        {
            public string Id => "image-click";
            public string Title => "Image click";
            public string Description => "Find UI element by image, then click";
            public FontAwesomeIcon Icon => FontAwesomeIcon.PictureOutline;
            public string AccentColorHex => "#EA580C";

            public bool TryExecute(IStepActionContext context)
            {
                context.MinimizeForCapture();
                var win = new ImageCaptureWindow { Owner = context.OwnerWindow };
                try
                {
                    if (win.ShowDialog() == true && !win.Cancelled)
                    {
                        context.AppendInstruction(new ImageClickInstruction(
                            UtilityType.ImageClick,
                            win.ResultImageBase64,
                            win.ResultWidth,
                            win.ResultHeight));
                        return true;
                    }
                }
                finally
                {
                    context.RestoreAfterCapture();
                }
                return false;
            }
        }

        private sealed class KeyPressAction : IStepActionProvider
        {
            public string Id => "key";
            public string Title => "Key press";
            public string Description => "Send a keyboard key";
            public FontAwesomeIcon Icon => FontAwesomeIcon.KeyboardOutline;
            public string AccentColorHex => "#6366F1";

            public bool TryExecute(IStepActionContext context)
            {
                var win = new KeyCaptureWindow { Owner = context.OwnerWindow };
                if (win.ShowDialog() == true && !win.Cancelled)
                {
                    context.AppendInstruction(new KeyboardInstruction(UtilityType.Keys, win.KeyCode, win.KeyName));
                    return true;
                }
                return false;
            }
        }

        private sealed class WaitAction : IStepActionProvider
        {
            public string Id => "wait";
            public string Title => "Wait";
            public string Description => "Pause before the next step";
            public FontAwesomeIcon Icon => FontAwesomeIcon.ClockOutline;
            public string AccentColorHex => "#64748B";

            public bool TryExecute(IStepActionContext context)
            {
                var win = new WaitEditWindow(500) { Owner = context.OwnerWindow };
                if (win.ShowDialog() == true && win.Confirmed)
                {
                    context.AppendInstruction(new DelayInstruction(
                        UtilityType.Delay,
                        TimeSpan.FromMilliseconds(win.ResultMilliseconds)));
                    return true;
                }
                return false;
            }
        }
        private sealed class ImageDoubleClickAction : IStepActionProvider
        {
            public string Id => "image-double-click";
            public string Title => "Image double click";
            public string Description => "Find UI element by image, then double click";
            public FontAwesomeIcon Icon => FontAwesomeIcon.PictureOutline;
            public string AccentColorHex => "#DC2626"; 

            public bool TryExecute(IStepActionContext context)
            {
                context.MinimizeForCapture();
                var win = new ImageCaptureWindow { Owner = context.OwnerWindow };
                try
                {
                    if (win.ShowDialog() == true && !win.Cancelled)
                    {

                        context.AppendInstruction(new ImageClickInstruction(
                            UtilityType.ImageDoubleClick,
                            win.ResultImageBase64,
                            win.ResultWidth,
                            win.ResultHeight));
                        return true;
                    }
                }
                finally
                {
                    context.RestoreAfterCapture();
                }
                return false;
            }

        }
    }
}