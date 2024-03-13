﻿namespace BlazorComponent
{
    public partial class BPaginationItems<TPagination> where TPagination : IPagination
    {
        public string? HrefFormat => Component.HrefFormat;
        
        public IEnumerable<StringNumber> GetItems() => Component.GetItems();

        public int Value => Component.Value;

        public async Task HandleItemClickAsync(StringNumber item) => await Component.HandleItemClickAsync(item);
    }
}
