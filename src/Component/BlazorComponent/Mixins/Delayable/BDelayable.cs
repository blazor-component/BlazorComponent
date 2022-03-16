﻿using Microsoft.AspNetCore.Components;

namespace BlazorComponent;

public class BDelayable : BDomComponentBase
{
    private CancellationTokenSource _cancellationTokenSource;

    [Parameter]
    public int OpenDelay { get; set; } = 10;

    [Parameter]
    public int CloseDelay { get; set; } = 10;

    public async Task RunOpenDelayAsync(Func<Task> cb = null)
    {
        await InvokeAsync(async () =>
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Delay(OpenDelay, _cancellationTokenSource.Token);

            if (cb != null)
            {
                await cb.Invoke();
            }
        });
    }

    public async Task RunCloseDelayAsync(Func<Task> cb = null)
    {
        await InvokeAsync(async () =>
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Delay(CloseDelay, _cancellationTokenSource.Token);

            if (cb != null)
            {
                await cb.Invoke();
            }
        });
    }
}