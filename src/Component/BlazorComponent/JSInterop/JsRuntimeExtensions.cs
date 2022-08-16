﻿using Microsoft.JSInterop;
using OneOf;

namespace BlazorComponent.JSInterop;

public static class JsRuntimeExtensions
{
    public static async Task AddOutsideClickEventListener(this IJSRuntime jsRuntime, Func<ClickOutsideArgs, Task> callback,
        IEnumerable<string> noInvokeSelectors, IEnumerable<string> invokeSelectors = null)
    {
        await jsRuntime.InvokeVoidAsync(JsInteropConstants.AddOutsideClickEventListener,
            DotNetObjectReference.Create(new Invoker<ClickOutsideArgs>(callback)),
            noInvokeSelectors, invokeSelectors);
    }

    public static async Task AddHtmlElementEventListener<T>(this IJSRuntime jsRuntime, string selector, string type, Func<T, Task> callback,
        OneOf<EventListenerOptions, bool> options, EventListenerExtras extras = null)
    {
        await jsRuntime.InvokeVoidAsync(JsInteropConstants.AddHtmlElementEventListener,
            selector,
            type,
            DotNetObjectReference.Create(new Invoker<T>(callback)),
            options.Value,
            extras
        );
    }

    public static async Task AddHtmlElementEventListener(this IJSRuntime jsRuntime, string selector, string type, Action callback,
        OneOf<EventListenerOptions, bool> options, EventListenerExtras extras = null)
    {
        await jsRuntime.InvokeVoidAsync(JsInteropConstants.AddHtmlElementEventListener,
            selector,
            type,
            DotNetObjectReference.Create(new Invoker(callback)),
            options.Value,
            extras
        );
    }
    
    public static async Task AddHtmlElementEventListener(this IJSRuntime jsRuntime, string selector, string type, Func<Task> callback,
        OneOf<EventListenerOptions, bool> options, EventListenerExtras extras = null)
    {
        await jsRuntime.InvokeVoidAsync(JsInteropConstants.AddHtmlElementEventListener,
            selector,
            type,
            DotNetObjectReference.Create(new Invoker(callback)),
            options.Value,
            extras
        );
    }

    public static async Task RemoveHtmlElementEventListener(this IJSRuntime jsRuntime, string selector, string type)
    {
        await jsRuntime.InvokeVoidAsync(JsInteropConstants.RemoveHtmlElementEventListener, selector, type);
    }
}
