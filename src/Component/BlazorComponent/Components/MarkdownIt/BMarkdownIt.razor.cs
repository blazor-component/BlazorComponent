﻿namespace BlazorComponent;

public partial class BMarkdownIt : BDomComponentBase
{
    [Inject]
    protected MarkdownItProxyModule MarkdownItProxyModule { get; set; }

    /// <summary>
    /// Enable markdown-it-header-sections plugin.
    /// </summary>
    [Parameter]
    public bool HeaderSections { get; set; }

    [Parameter]
    [EditorRequired]
    public string? Source { get; set; }

    [Parameter]
    public string? Key { get; set; }

    #region MarkdownItOptions

    /// <summary>
    /// Enable HTML tags in source
    /// </summary>
    [Parameter]
    public bool Html
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <summary>
    /// Use '/' to close single tags (<br />).
    /// This is only for full CommonMark compatibility.
    /// </summary>
    [Parameter]
    public bool XHtmlOut
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <summary>
    /// Convert '\n' in paragraphs into br tag.
    /// </summary>
    [Parameter]
    public bool Breaks
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <summary>
    /// CSS language prefix for fenced blocks.
    /// Can be useful for external highlighters.
    /// </summary>
    [Parameter]
    public string? LangPrefix
    {
        get => GetValue("language-");
        set => SetValue(value);
    }

    /// <summary>
    /// Autoconvert URL-like text to links
    /// </summary>
    [Parameter]
    public bool Linkify
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <summary>
    /// Enable some language-neutral replacement + quotes beautification
    /// </summary>
    [Parameter]
    public bool Typographer
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <summary>
    ///  Double + single quotes replacement pairs,
    /// when typographer enabled, and smartquotes on. 
    /// </summary>
    [Parameter]
    public string[]? Quotes
    {
        get => GetValue(new[] { "“", "”", "‘", "’" });
        set => SetValue(value);
    }

    #endregion

    /// <summary>
    /// Determines whether to wrap it with elements
    /// </summary>
    [Parameter]
    public bool NoWrapper { get; set; }

    /// <summary>
    /// A callback that will be invoked when the <see cref="Source"/> rendered.
    /// </summary>
    [Parameter]
    public EventCallback<string?> OnFrontMatterParsed { get; set; }

    /// <summary>
    /// Enable markdown-it-anchor plugin.
    /// </summary>
    [Parameter]
    public MarkdownItAnchorOptions? AnchorOptions { get; set; }

    /// <summary>
    /// A callback that will be invoked when the <see cref="AnchorOptions"/> was instantiated and the <see cref="Source"/> rendered.
    /// </summary>
    [Parameter]
    public EventCallback<List<MarkdownItTocContent>?> OnTocParsed { get; set; }

    private string _mdHtml = string.Empty;

    private string? _prevSource;
    private MarkdownItProxy? _markdownItProxy;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Watcher
            .Watch<bool>(nameof(Html), GoCreateMarkdownItProxy)
            .Watch<bool>(nameof(XHtmlOut), GoCreateMarkdownItProxy)
            .Watch<bool>(nameof(Breaks), GoCreateMarkdownItProxy)
            .Watch<string>(nameof(LangPrefix), GoCreateMarkdownItProxy)
            .Watch<bool>(nameof(Linkify), GoCreateMarkdownItProxy)
            .Watch<bool>(nameof(Typographer), GoCreateMarkdownItProxy)
            .Watch<string[]>(nameof(Quotes), GoCreateMarkdownItProxy);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_prevSource != Source)
        {
            _prevSource = Source;

            await TryParse();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await CreateMarkdownItProxy();
            await TryParse();
        }
    }

    private async void GoCreateMarkdownItProxy()
    {
        await CreateMarkdownItProxy();
        NextTick(async () => { await TryParse(); });
    }

    private async Task CreateMarkdownItProxy()
    {
        var options = new MarkdownItOptions()
        {
            Html = Html,
            XHtmlOut = XHtmlOut,
            Breaks = Breaks,
            LangPrefix = LangPrefix,
            Linkify = Linkify,
            Typographer = Typographer,
            Quotes = Quotes
        };

        var key = Key ?? this.GetHashCode().ToString();

        _markdownItProxy = await MarkdownItProxyModule.Create(options, HeaderSections, AnchorOptions, key);
    }

    private async Task TryParse()
    {
        if (_markdownItProxy is null) return;

        if (Source == null)
        {
            _mdHtml = null;
        }
        else
        {
            var result = await _markdownItProxy.ParseAll(Source);

            if (OnFrontMatterParsed.HasDelegate)
            {
                await OnFrontMatterParsed.InvokeAsync(result.FrontMatter);
            }

            _mdHtml = result.MarkupContent;

            if (OnTocParsed.HasDelegate)
            {
                await OnTocParsed.InvokeAsync(result.Toc);
            }
        }

        StateHasChanged();
    }
}
