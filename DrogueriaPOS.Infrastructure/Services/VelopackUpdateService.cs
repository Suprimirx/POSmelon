using Velopack;
using Velopack.Exceptions;
using Velopack.Sources;
using DrogueriaPOS.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;


namespace DrogueriaPOS.Infrastructure.Services;

public sealed class VelopackUpdateService : IUpdateService
{
    private readonly UpdateManager _updateManager;

    public VelopackUpdateService(IConfiguration configuration)
    {
        var updateSource = configuration["Velopack:UpdateSource"]
            ?? throw new InvalidOperationException("Velopack:UpdateSource configuration is missing.");

        _updateManager = updateSource.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? new UpdateManager(new GithubSource(updateSource, accessToken: null, prerelease: false))
            : new UpdateManager(updateSource);
    }

    public async Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var updateInfo = await _updateManager.CheckForUpdatesAsync();

            return updateInfo is null
                ? new UpdateCheckResult(false, null, null, null)
                : new UpdateCheckResult(
                    IsUpdateAvailable: true,
                    Version: updateInfo.TargetFullRelease.Version.ToString(),
                    ReleaseNotesHtml: updateInfo.TargetFullRelease.NotesHTML,
                    UpdateHandle: updateInfo);
        }
        catch (NotInstalledException)
        {
            // Corriendo desde Visual Studio (F5), no instalado vía Velopack: no hay nada que chequear
            return new UpdateCheckResult(false, null, null, null);
        }
    }

    public async Task DownloadAndApplyUpdateAsync(UpdateCheckResult update, CancellationToken cancellationToken = default)
    {
        if (update.UpdateHandle is not UpdateInfo info) return;

        await _updateManager.DownloadUpdatesAsync(info);
        _updateManager.ApplyUpdatesAndRestart(info); // cierra la app y la reabre ya actualizada
    }
}
