﻿namespace BlazorComponent
{
    public abstract class BComponentBase : NextTickComponentBase, IHandleEvent
    {
        [Inject]
        [NotNull]
        public virtual IJSRuntime? Js { get; set; }

        [CascadingParameter]
        protected IDefaultsProvider? DefaultsProvider { get; set; }

        /// <summary>
        /// Disable the default value provider.
        /// Components for internal use should not be affected by the default value provider.
        /// Just for internal use.
        /// </summary>
        [CascadingParameter(Name = "DisableDefaultsProvider")]
        public bool DisableDefaultsProvider { get; set; }

        [CascadingParameter]
        protected IErrorHandler? ErrorHandler { get; set; }

        [Parameter]
        public ForwardRef RefBack
        {
            get => _refBack ??= new ForwardRef();
            set => _refBack = value;
        }

        // BlazorComponent will be merge into Masa.Blazor in feature
        public static readonly string ImplementedAssemblyName = "Masa.Blazor";

        private ForwardRef? _refBack;
        private bool _shouldRender = true;
        private string[] _dirtyParameters = Array.Empty<string>();

        protected virtual string ComponentName
        {
            get
            {
                var type = this.GetType();

                while (type.Assembly.GetName().Name != ImplementedAssemblyName)
                {
                    if (type.BaseType is null)
                    {
                        break;
                    }

                    type = type.BaseType;
                }

                return type.IsGenericType ? type.Name.Split('`')[0] : type.Name;
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            _dirtyParameters = parameters.ToDictionary().Keys.ToArray();

            var disableDefaultsExists = parameters.TryGetValue<bool>(nameof(DisableDefaultsProvider), out var disableDefaults);

            if ((!disableDefaultsExists || (disableDefaultsExists && !disableDefaults))
                && parameters.TryGetValue<IDefaultsProvider>(nameof(DefaultsProvider), out var defaultsProvider)
                && defaultsProvider.Defaults is not null
                && defaultsProvider.Defaults.TryGetValue(ComponentName, out var dictionary)
                && dictionary is not null)
            {
                var defaults = ParameterView.FromDictionary(dictionary);
                defaults.SetParameterProperties(this);
            }

            await base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender() => _shouldRender;

        /// <summary>
        /// Check whether the parameter has been assigned value.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected bool IsDirtyParameter(string parameterName)
        {
            return _dirtyParameters.Contains(parameterName);
        }

        protected void InvokeStateHasChanged()
        {
            if (!IsDisposed)
            {
                _ = InvokeAsync(StateHasChanged);
            }
        }

        protected async Task InvokeStateHasChangedAsync()
        {
            if (!IsDisposed)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task<T> JsInvokeAsync<T>(string code, params object?[] args)
        {
            return await Js.InvokeAsync<T>(code, args);
        }

        protected async Task JsInvokeAsync(string code, params object?[] args)
        {
            await Js.InvokeVoidAsync(code, args);
        }

        protected void PreventRenderingUtil(params Action[] actions)
        {
            _shouldRender = false;
            actions.ForEach(action => action());
            _shouldRender = true;
        }

        protected async Task PreventRenderingUtil(params Func<Task>[] funcs)
        {
            _shouldRender = false;
            await funcs.ForEachAsync(func => func());
            _shouldRender = true;
        }

        /// <summary>
        /// Debounce a task in microseconds. 
        /// </summary>
        /// <param name="task">A task to run.</param>
        /// <param name="millisecondsDelay">Delay in milliseconds.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the last task.</param>
        protected static async Task RunTaskInMicrosecondsAsync(Func<Task> task, int millisecondsDelay, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
                await task.Invoke();
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        /// <summary>
        /// Debounce a task in microseconds.
        /// </summary>
        /// <param name="task">A task to run.</param>
        /// <param name="millisecondsDelay">Delay in milliseconds.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the last task.</param>
        protected static async Task RunTaskInMicrosecondsAsync(Action task, int millisecondsDelay, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
                task.Invoke();
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        protected virtual bool AfterHandleEventShouldRender()
        {
            return true;
        }

        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
        {
            var task = callback.InvokeAsync(arg);
            var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
                                  task.Status != TaskStatus.Canceled;

            if (AfterHandleEventShouldRender())
            {
                StateHasChanged();
            }

            return shouldAwaitTask
                ? CallStateHasChangedOnAsyncCompletion(task)
                : Task.CompletedTask;
        }

        private async Task CallStateHasChangedOnAsyncCompletion(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex) // avoiding exception filters for AOT runtime support
            {
                // Ignore exceptions from task cancellations, but don't bother issuing a state change.
                if (task.IsCanceled)
                {
                    return;
                }

                if (ErrorHandler != null)
                {
                    await ErrorHandler.HandleExceptionAsync(ex);
                }
                else
                {
                    throw;
                }
            }

            if (AfterHandleEventShouldRender())
            {
                StateHasChanged();
            }
        }
    }
}
