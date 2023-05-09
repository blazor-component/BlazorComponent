﻿namespace BlazorComponent
{
    public partial class BSlideGroupNext<TSlideGroup> : ComponentPartBase<TSlideGroup>
        where TSlideGroup : ISlideGroup
    {
        protected bool Active => Component.HasNext;

        protected string? Icon => Component.NextIcon;

        protected RenderFragment? IconContent => Component.NextContent;

        protected bool Visible => !(Component.ShowArrows == null && !Active);

        protected Task HandleOnClick(MouseEventArgs args)
        {
            return Component.OnAffixClick("next");
        }
    }
}