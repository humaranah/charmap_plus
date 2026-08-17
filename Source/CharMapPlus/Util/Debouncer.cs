using System;
using System.Threading;
using System.Threading.Tasks;

namespace CharMapPlus.Util;

/// <summary>
/// A utility class that provides debouncing functionality for actions.
/// </summary>
/// <param name="delayMilliseconds">
/// The delay in milliseconds to wait before executing the action.
/// </param>
public sealed partial class Debouncer(int delayMilliseconds) : IDisposable
{
    private CancellationTokenSource? _cts;

    /// <summary>
    /// Executes the specified action after the debounce delay.
    /// </summary>
    /// <param name="action">
    /// The action to be executed after the debounce delay.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task ExecuteAsync(Func<Task> action)
    {
        if (_cts is not null)
        {
            await _cts.CancelAsync();
            _cts.Dispose();
        }
        _cts = new CancellationTokenSource();

        try
        {
            await Task.Delay(delayMilliseconds, _cts.Token);
            await action();
        }
        catch (OperationCanceledException)
        {
            // Debounced action was canceled
        }
    }

    /// <summary>
    /// Executes the specified action after the debounce delay.
    /// </summary>
    /// <param name="action">
    /// The action to be executed after the debounce delay.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task ExecuteAsync(Action action)
    {
        await ExecuteAsync(() =>
        {
            action();
            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Disposes the debouncer and cancels any pending actions.
    /// </summary>
    public void Dispose()
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        _cts = null;
    }
}
