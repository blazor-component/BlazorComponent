﻿namespace Microsoft.AspNetCore.Components.Web
{
    public class EventTarget
    {
        public string Id => GetAttribute("id");

        private string GetAttribute(string name)
        {
            if (Attributes != null && Attributes.TryGetValue(name, out var value))
            {
                return value;
            }

            return string.Empty;
        }

        public string Class => GetAttribute("class");

        public string Style => GetAttribute("style");

        public ElementReference ElementReference
        {
            get
            {
                if (Attributes != null)
                {
                    var identify = Attributes.FirstOrDefault(attr => attr.Key.StartsWith("_bl_"));
                    return new ElementReference(identify.Key?[4..]);
                }

                return default;
            }
        }

        public Dictionary<string, string> Attributes { get; set; }
    }
}
