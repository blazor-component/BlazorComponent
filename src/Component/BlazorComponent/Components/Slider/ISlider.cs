﻿namespace BlazorComponent
{
    public interface ISlider<TValue, out TNumeric> : IInput<TValue>, ILoadable
    {
        bool InverseLabel => default;

        Dictionary<string, object> InputAttrs { get; }

        ElementReference TrackElement { set; }

        TNumeric Step { get; }

        bool ShowTicks { get; }

        double TickSize { get; }

        double NumTicks { get; }

        bool Vertical { get; }

        List<string> TickLabels { get; }

        ElementReference ThumbElement { get; set; }

        ElementReference SliderElement { get; set; }

        Task HandleOnFocusAsync(FocusEventArgs args);

        Task HandleOnBlurAsync(FocusEventArgs args);

        Dictionary<string, object> ThumbAttrs { get; }

        bool ShowThumbLabel { get; }

        bool ShowThumbLabelContainer { get; }

        RenderFragment<int> ComputedThumbLabelContent { get; }

        Task HandleOnSliderClickAsync(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        Task HandleOnSliderMouseDownAsync(ExMouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        Task HandleOnTouchStartAsync(ExTouchEventArgs args)
        {
            return Task.CompletedTask;
        }

        Task HandleOnKeyDownAsync(KeyboardEventArgs args);
    }
}
