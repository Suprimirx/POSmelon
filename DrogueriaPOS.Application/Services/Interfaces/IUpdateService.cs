
namespace DrogueriaPOS.Application.Services.Interfaces;

public interface IUpdateService
{
    Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default);
    Task DownloadAndApplyUpdateAsync(UpdateCheckResult update, CancellationToken cancellationToken = default);
}

public sealed record UpdateCheckResult(
    bool IsUpdateAvailable,
    string? Version,
    string? ReleaseNotesHtml,
    object? UpdateHandle); // detalle interno de Velopack, opaco para quien consuma esto
