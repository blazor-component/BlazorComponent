﻿using Microsoft.AspNetCore.Components.Routing;

namespace BlazorComponent
{
    public partial class BListItem : BRoutableGroupItem<ItemGroupBase>
    {
        public BListItem() : base(GroupType.ListItemGroup)
        {
        }

        [CascadingParameter(Name = "IsInGroup")]
        public bool IsInGroup { get; set; }

        [CascadingParameter(Name = "IsInMenu")]
        public bool IsInMenu { get; set; }

        [CascadingParameter(Name = "IsInList")]
        public bool IsInList { get; set; }

        [CascadingParameter(Name = "IsInNav")]
        public bool IsInNav { get; set; }

        [CascadingParameter]
        public BList? List { get; set; }

        [Parameter]
        public string? Color { get; set; }

        [Parameter]
        public RenderFragment<ItemContext>? ItemContent { get; set; }

        [Parameter]
        public bool OnClickStopPropagation { get; set; }

        [Parameter]
        public bool OnClickPreventDefault { get; set; }

        [Parameter] [MasaApiParameter(ReleasedOn = "v1.5.0")] public string? Title { get; set; }

        [Parameter] [MasaApiParameter(ReleasedOn = "v1.5.0")] public string? Subtitle { get; set; }

        [Parameter] [MasaApiParameter(ReleasedOn = "v1.5.0")] public string? PrependIcon { get; set; }

        [Parameter] [MasaApiParameter(ReleasedOn = "v1.5.0")] public string? PrependAvatar { get; set; }

        [Parameter] [MasaApiParameter(ReleasedOn = "v1.5.0")] public string? AppendIcon { get; set; }

        [Parameter] [MasaApiParameter(ReleasedOn = "v1.5.0")] public string? AppendAvatar { get; set; }

        [Parameter]
        public bool Dark { get; set; }

        [Parameter]
        public bool Light { get; set; }

        [CascadingParameter(Name = "IsDark")]
        public bool CascadingIsDark { get; set; }

        public bool IsDark
        {
            get
            {
                if (Dark)
                {
                    return true;
                }

                if (Light)
                {
                    return false;
                }

                return CascadingIsDark;
            }
        }

        protected bool IsClickable => Router?.IsClickable is true || Matched;

        public bool IsLink => Router?.IsLink is true;

        protected override bool IsRoutable => Href != null && List?.Routable is true;

        private bool HasBuiltInContent => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Subtitle);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            SetAttrs();
        }

        protected override bool AfterHandleEventShouldRender() => false;

        protected virtual async Task HandleOnClick(MouseEventArgs args)
        {
            if (args.Detail > 0)
            {
                await JsInvokeAsync(JsInteropConstants.Blur, Ref);
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(new MouseEventWithRefArgs(args, Ref));
            }

            if (IsRoutable) return;

            await ToggleAsync();
        }

        private void SetAttrs()
        {
            Attributes["aria-disabled"] = Disabled ? true : null;
            Attributes["tabindex"] = IsClickable ? 0 : -1;

            if (Attributes.ContainsKey("role"))
            {
                // do nothing, role already provided
            }
            else if (IsInNav)
            {
                // do nothing, role is inherit (TODO:check)
            }
            else if (IsInGroup)
            {
                Attributes["role"] = "option";
                Attributes["aria-selected"] = InternalIsActive.ToString();
            }
            else if (IsInMenu)
            {
                Attributes["role"] = IsClickable ? "menuitem" : null;
                Attributes["id"] = Id ?? $"list-item-{Id}"; // TODO:check
            }
            else if (IsInList)
            {
                Attributes["role"] = "listitem";
            }
        }

        private ItemContext GenItemContext()
        {
            return new ItemContext(
                InternalIsActive,
                InternalIsActive ? ComputedActiveClass : "",
                ToggleAsync,
                RefBack,
                Value
            );
        }
    }
}
