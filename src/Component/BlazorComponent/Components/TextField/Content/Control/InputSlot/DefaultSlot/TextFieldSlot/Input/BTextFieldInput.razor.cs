﻿using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponent
{
    public partial class BTextFieldInput<TValue, TInput> where TInput : ITextField<TValue>
    {
        public bool Autofocus => Component.Autofocus;

        public bool Disabled => Component .IsDisabled;

        public bool HasLabel => Component.HasLabel;

        public string Placeholder => (Component.PersistentPlaceholder || Component.IsFocused || !HasLabel) ? Component.Placeholder : null;

        public bool Readonly => Component.IsReadonly;

        public string Id => Component.Id;

        public string InputTag => Component.Tag;

        public Dictionary<string, object> InputAttrs => Component.InputAttrs;

        public EventCallback<ChangeEventArgs> HandleOnChange => EventCallback.Factory.Create<ChangeEventArgs>(Component, Component.HandleOnChangeAsync);

        public EventCallback<FocusEventArgs> HandleOnBlur => EventCallback.Factory.Create<FocusEventArgs>(Component, Component.HandleOnBlurAsync);

        public EventCallback<FocusEventArgs> HandleOnFocus => EventCallback.Factory.Create<FocusEventArgs>(Component, Component.HandleOnFocusAsync);

        public EventCallback<KeyboardEventArgs> HandleOnKeyDown => EventCallback.Factory.Create<KeyboardEventArgs>(Component, Component.HandleOnKeyDownAsync);

        public EventCallback<KeyboardEventArgs> HandleOnKeyUp => EventCallback.Factory.Create<KeyboardEventArgs>(Component, Component.HandleOnKeyUpAsync);
    }
}
