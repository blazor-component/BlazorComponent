﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponent
{
    public interface IIcon : IHasProviderComponent
    {
        bool Dense { get; set; }

        bool Disabled { get; set; }

        bool Left { get; set; }

        ElementReference Ref { get; set; }

        bool Right { get; set; }

        StringNumber Size { get; set; }

        string Icon { get; set; }

        string Tag { get; set; }

        string NewChildren { get; set; }

        Dictionary<string, object> SvgAttrs { get; set; }

        EventCallback<MouseEventArgs> OnClick { get; set; }

        Task HandleOnClick(MouseEventArgs args);

        IDictionary<string, object> Attrs { get; }
    }
}
