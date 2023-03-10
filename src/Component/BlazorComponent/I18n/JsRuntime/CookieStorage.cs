﻿using Microsoft.JSInterop;

namespace BlazorComponent.I18n;

public class CookieStorage
{
    private readonly IJSRuntime _jsRuntime;

    public CookieStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    const string GetCookieJs =
        "(function(name){const reg = new RegExp(`(^| )${name}=([^;]*)(;|$)`);const arr = document.cookie.match(reg);if (arr) {return unescape(arr[2]);}return null;})";

    const string SetCookieJs =
        "(function(name,value){ var hostArraies=document.location.host.split('.');var Days = 30;var exp = new Date();exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);document.cookie = `${name}=${escape(value.toString())};path=/;expires=${exp.toUTCString()};domain=.${hostArraies[hostArraies.length-2]}.${hostArraies[hostArraies.length-1]}`;})";

    public async Task<string> GetCookieAsync(string key)
    {
        return await _jsRuntime.InvokeAsync<string>("eval", $"{GetCookieJs}('{key}')");
    }

    public string? GetCookie(string key)
    {
        if (_jsRuntime is IJSInProcessRuntime jsInProcess)
        {
            return jsInProcess.Invoke<string>("eval", $"{GetCookieJs}('{key}')");
        }

        // TODO: how to read config in MAUI?

        return null;
    }

    public async void SetItemAsync<T>(string key, T? value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", $"{SetCookieJs}('{key}','{value}')");
        }
        catch
        {
        }
    }
}
