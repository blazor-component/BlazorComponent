﻿using Microsoft.AspNetCore.Components;

namespace BlazorComponent
{
    public partial class BOverlay : BDomComponentBase
    {
        /// <summary>
        /// Controls whether the component is visible or hidden.
        /// </summary>
        [Parameter]
        public bool Value
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [Parameter]
        [ApiDefaultValue(true)]
        public bool Scrim { get; set; } = true;

        [Parameter]
        [ApiDefaultValue(true)]
        public bool Dark { get; set; } = true;

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
    }
}
