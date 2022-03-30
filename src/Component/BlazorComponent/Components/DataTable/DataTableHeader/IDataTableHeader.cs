﻿using Microsoft.AspNetCore.Components;

namespace BlazorComponent
{
    public interface IDataTableHeader : IHasProviderComponent
    {
        bool SingleSelect { get; }

        RenderFragment DataTableSelectContent { get; }

        bool DisableSort { get; }

        string SortIcon { get; }

        bool ShowGroupBy { get; }

        RenderFragment<DataTableHeader> HeaderColContent { get; }

        Task HandleOnGroup(string group);

        Dictionary<string, object> GetHeaderAttrs(DataTableHeader header);

        DataOptions Options { get; }
    }
}

