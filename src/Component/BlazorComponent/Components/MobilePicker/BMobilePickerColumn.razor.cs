﻿using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponent;

public partial class BMobilePickerColumn<TColumnItem>
{
    [Parameter]
    public List<TColumnItem> Items { get; set; } = new();

    [Parameter]
    public int ItemHeight { get; set; }

    [Parameter]
    public Func<TColumnItem, string> ItemText { get; set; }

    [Parameter]
    public Func<TColumnItem, bool> ItemDisabled { get; set; } = _ => false;

    [Parameter]
    public int SelectedIndex { get; set; }

    [Parameter]
    public int SwipeDuration { get; set; }

    [Parameter]
    public StringNumber VisibleItemCount { get; set; }

    [Parameter]
    public EventCallback<int> OnChange { get; set; }

    private const int DefaultDuration = 200;

    // 惯性滑动思路:
    // 在手指离开屏幕时，如果和上一次 move 时的间隔小于 `MomentumLimitTime` 且 move
    // 距离大于 `MomentumLimitDistance` 时，执行惯性滑动
    private const int MomentumLimitTime  = 300;
    private const int MomentumLimitDistance  = 15;

    private bool _moving;
    private double _startOffset;
    private Func<Task> _transitionEndTrigger;
    private long _touchStartTime;
    private double _momentumOffset;
    private double _startX;
    private double _startY;
    private double _deltaX;
    private double _deltaY;
    private double _offsetX;
    private double _offsetY;
    private string _direction;

    private ElementReference Wrapper { get; set; }

    protected double Offset { get; private set; }
    protected int Duration { get; private set; }

    private int Count => Items.Count;

    protected int BaseOffset => ItemHeight * (VisibleItemCount.ToInt32() - 1) / 2;

    private int _prevSelectedIndex;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_prevSelectedIndex != SelectedIndex)
        {
            _prevSelectedIndex = SelectedIndex;
            SetIndex(SelectedIndex);
        }
    }

    private async Task OnTouchstart(TouchEventArgs args)
    {
        Touchstart(args);

        if (_moving)
        {
            var translateY = await JsInvokeAsync<double>(JsInteropConstants.GetElementTranslateY, Wrapper);
            Offset = Math.Min(0, translateY - BaseOffset);
            _startOffset = Offset;
        }
        else
        {
            _startOffset = Offset;
        }

        Duration = 0;
        _transitionEndTrigger = null;
        _touchStartTime = DateTime.Now.Millisecond;
        _momentumOffset = _startOffset;
    }

    private void OnTouchmove(TouchEventArgs args)
    {
        Touchmove(args);

        if (_direction == "vertical")
        {
            _moving = true;
            // TODO: preventDefault(args, true);
        }

        Offset = Range(_startOffset + _deltaY, -(Count * ItemHeight), ItemHeight);

        var now = DateTime.Now.Millisecond;
        if (now - _touchStartTime > MomentumLimitTime)
        {
            _touchStartTime = now;
            _momentumOffset = Offset;
        }
    }

    private void OnTouchend(TouchEventArgs args)
    {
        var distance = Offset - _momentumOffset;
        var duration = DateTime.Now.Millisecond - _touchStartTime;
        var allowMomentum = duration < MomentumLimitTime && Math.Abs(distance) > MomentumLimitDistance;

        if (allowMomentum)
        {
            Momentum(distance, duration);
            return;
        }

        var index = GetIndexByOffset(Offset);
        Duration =  DefaultDuration;
        SetIndex((int)Math.Ceiling(index), true);

        // compatible with desktop scenario
        // TODO: use setTimeout to skip the click event Emitted after touchstart in js, how to do it in Blazor
        // setTimeout(() => { _moving = false; }, 0);
        _moving = false;
    }

    private void Momentum(double distance, long duration)
    {
        var speed = Math.Abs(distance / duration);
        distance = Offset + (speed / 0.003) * (distance < 0 ? -1 : 1);
        var index = GetIndexByOffset(distance);

        Duration = SwipeDuration;
        StateHasChanged();
        SetIndex((int)Math.Ceiling(index), true);
    }

    private double GetIndexByOffset(double offset)
    {
        return Range(Math.Round(-offset / ItemHeight), 0, Count - 1);
    }

    internal void SetIndex()
    {
        SetIndex(SelectedIndex);
    }

    private void SetIndex(int index, bool emitChange = false)
    {
        index = AdjustIndex(index) ?? 0;

        var offset  = -index * ItemHeight;

        var trigger = async () =>
        {
            if (index != SelectedIndex)
            {
                SelectedIndex = index;

                if (emitChange)
                {
                    await OnChange.InvokeAsync(index);
                }
            }
        };

        if (_moving && Math.Abs(offset - Offset) > 0)
        {
            _transitionEndTrigger = trigger;
        }
        else
        {
            trigger();
        }

        Offset = offset;
    }

    private void OnClickItem(int index)
    {
        if (_moving)
        {
            return;
        }

        _transitionEndTrigger = null;
        Duration = DefaultDuration;
        SetIndex(index, true);
    }

    private int? AdjustIndex(int index)
    {
        index = Range(index, 0, Count);

        for (int i = index; i < Count; i++)
        {
            if (!ItemDisabled(Items[i])) return i;
        }

        for (int i = index - 1; i >= 0; i--)
        {
            if (!ItemDisabled(Items[i])) return i;
        }

        return null;
    }

    private double Range(double num, double min, double max)
    {
        return Math.Min(Math.Max(num, min), max);
    }

    private int Range(int num, int min, int max)
    {
        return Math.Min(Math.Max(num, min), max);
    }

    private void Touchstart(TouchEventArgs args)
    {
        ResetTouchStatus();
        _startX = args.Touches[0].ClientX;
        _startY = args.Touches[0].ClientY;
    }

    private void Touchmove(TouchEventArgs args)
    {
        var touch = args.Touches[0];

        _deltaX = touch.ClientX < 0 ? 0 : touch.ClientX - _startX;
        _deltaY = touch.ClientY - _startY;
        _offsetX = Math.Abs(_deltaX);
        _offsetY = Math.Abs(_deltaY);

        const int lockDirectionDistance = 10;

        if (string.IsNullOrEmpty(_direction) || (_offsetX < lockDirectionDistance && _offsetY < lockDirectionDistance))
        {
            _direction = GetDirection(_offsetX, _offsetY);
        }

        string GetDirection(double x, double y)
        {
            if (x > y)
            {
                return "horizontal";
            }

            if (y > x)
            {
                return "vertical";
            }

            return string.Empty;
        }
    }

    private void ResetTouchStatus()
    {
        _direction = string.Empty;
        _deltaX = 0;
        _deltaY = 0;
        _offsetX = 0;
        _offsetY = 0;
    }

    private async Task OnTransitionEnd()
    {
        await StopMomentum();
    }

    private async Task StopMomentum()
    {
        _moving = false;
        Duration = 0;

        if (_transitionEndTrigger is not null)
        {
            await _transitionEndTrigger.Invoke();
            _transitionEndTrigger = null;
        }
    }
}
