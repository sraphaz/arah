using Araponga.Application.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Araponga.Infrastructure.Postgres;

/// <summary>
/// Helper para tratar conflitos de concorrência otimista.
/// </summary>
public static class ConcurrencyHelper
{
    /// <summary>
    /// Executa uma operação com retry em caso de conflito de concorrência.
    /// </summary>
    public static async Task<TResult> ExecuteWithRetryAsync<TResult>(
        Func<Task<TResult>> operation,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        int attempts = 0;
        while (attempts < maxRetries)
        {
            try
            {
                return await operation();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                attempts++;
                ArapongaMetrics.ConcurrencyConflicts.Add(1);
                
                if (attempts >= maxRetries)
                {
                    throw new InvalidOperationException(
                        $"Concurrency conflict after {maxRetries} attempts. The entity was modified by another process.",
                        ex);
                }

                // Aguardar um pouco antes de tentar novamente (exponential backoff)
                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempts), cancellationToken);
            }
        }

        throw new InvalidOperationException("Unexpected error in concurrency retry logic.");
    }

    /// <summary>
    /// Executa uma operação com retry em caso de conflito de concorrência (sem retorno).
    /// </summary>
    public static async Task ExecuteWithRetryAsync(
        Func<Task> operation,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        await ExecuteWithRetryAsync(
            async () =>
            {
                await operation();
                return true;
            },
            maxRetries,
            cancellationToken);
    }
}
