﻿using Microsoft.AspNetCore.Components;

namespace BlazorComponent
{
    public interface IDataFooter : IHasProviderComponent
    {
        IEnumerable<DataItemsPerPageOption> ComputedDataItemsPerPageOptions { get; }

        string ItemsPerPageText { get; }

        DataPagination Pagination { get; }

        RenderFragment<(int PageStart, int PageStop, int ItemsLength)> PageTextContent { get; }

        Task HandleOnPreviousPageAsync();

        string PageText { get; }

        bool ShowCurrentPage { get; }

        DataOptions Options { get; }

        bool DisablePagination { get; }

        Task HandleOnNextPageAsync();

        bool RTL { get; }

        string NextIcon { get; }

        string PrevIcon { get; }

        Task HandleOnFirstPageAsync();

        bool DisableNextPageIcon { get; }

        bool ShowFirstLastPage { get; }

        string LastIcon { get; }

        string FirstIcon { get; }

        Task HandleOnLastPageAsync();
    }
}
