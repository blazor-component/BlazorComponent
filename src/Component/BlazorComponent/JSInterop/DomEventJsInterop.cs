﻿using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Text.Json;

namespace BlazorComponent
{
    public class DomEventJsInterop
    {
        private ConcurrentDictionary<string, List<DomEventSub>> _domEventListeners = new ConcurrentDictionary<string, List<DomEventSub>>();

        private readonly IJSRuntime _jsRuntime;

        public DomEventJsInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private void AddEventListenerToFirstChildInternal<T>(object dom, string eventName, bool preventDefault, Action<T> callback)
        {
            if (!_domEventListeners.ContainsKey(FormatKey(dom, eventName)))
            {
                _jsRuntime.InvokeAsync<string>(JsInteropConstants.AddDomEventListenerToFirstChild, dom, eventName, preventDefault,
                    DotNetObjectReference.Create(new Invoker<T>((p) => { callback?.Invoke(p); })));
            }
        }

        public void AddEventListener(object dom, string eventName, Action<JsonElement> callback, bool preventDefault = false)
        {
            AddEventListener<JsonElement>(dom, eventName, callback, preventDefault);
        }

        public virtual void AddEventListener<T>(object dom, string eventName, Action<T> callback, bool preventDefault = false)
        {
            string key = FormatKey(dom, eventName);

            if (!_domEventListeners.ContainsKey(key) && _domEventListeners.TryAdd(key, new List<DomEventSub>()))
            {
                _jsRuntime.InvokeAsync<string>(JsInteropConstants.AddDomEventListener, dom, eventName, preventDefault, DotNetObjectReference.Create(
                    new Invoker<string>((p) =>
                    {
                        for (var i = 0; i < _domEventListeners[key].Count; i++)
                        {
                            var sub = _domEventListeners[key][i];
                            var args = JsonSerializer.Deserialize(p, sub.Type);
                            sub.Delegate.DynamicInvoke(args);
                        }
                    })));
            }

            _domEventListeners[key].Add(new DomEventSub(callback, typeof(T)));
        }

        public void AddEventListenerToFirstChild(object dom, string eventName, Action<JsonElement> callback, bool preventDefault = false)
        {
            AddEventListenerToFirstChildInternal<string>(dom, eventName, preventDefault, (e) =>
            {
                var jsonElement = JsonDocument.Parse(e).RootElement;
                callback(jsonElement);
            });
        }

        public void AddEventListenerToFirstChild<T>(object dom, string eventName, Action<T> callback, bool preventDefault = false)
        {
            AddEventListenerToFirstChildInternal<string>(dom, eventName, preventDefault, (e) =>
            {
                var obj = JsonSerializer.Deserialize<T>(e);
                callback(obj);
            });
        }

        private static string FormatKey(object dom, string eventName) => $"{dom}-{eventName}";

        public void RemoveEventListener<T>(object dom, string eventName, Action<T> callback)
        {
            string key = FormatKey(dom, eventName);

            if (_domEventListeners.ContainsKey(key))
            {
                var subscription = _domEventListeners[key].SingleOrDefault(s => s.Delegate == (Delegate)callback);
                if (subscription != null)
                {
                    _domEventListeners[key].Remove(subscription);
                }
            }
        }

        public void ResizeObserver<T>(object dom, Action<T> callback)
        {
            _jsRuntime.InvokeAsync<string>(JsInteropConstants.Observer, dom,
                DotNetObjectReference.Create(new Invoker<T>((p) => { callback?.Invoke(p); })));
        }
    }

    public class Invoker<T>
    {
        private Action<T> _action;
        private Func<T, Task> _func;

        public Invoker(Action<T> action)
        {
            _action = action;
        }

        public Invoker(Func<T, Task> func)
        {
            _func = func;
        }

        [JSInvokable]
        public async Task Invoke(T param)
        {
            if (_action != null)
            {
                _action.Invoke(param);
            }
            else if (_func != null)
            {
                await _func(param);
            }
        }
    }
}