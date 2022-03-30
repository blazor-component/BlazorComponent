﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponent
{
    public partial class BTextFieldIconSlot<TValue, TInput> where TInput : ITextField<TValue>
    {
        public string AppendIcon => Component.AppendIcon;

        public RenderFragment AppendContent => Component.AppendContent;

        public EventCallback<MouseEventArgs> HandleOnAppendClickAsync => EventCallback.Factory.Create<MouseEventArgs>(Component, Component.HandleOnAppendClickAsync);
    }
}
