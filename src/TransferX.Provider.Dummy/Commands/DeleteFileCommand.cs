// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.Extensions.Logging;
using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Commands;

/// <summary>
/// Löscht eine Datei im Basisverzeichnis des Dummy-Providers.
/// </summary>
/// <remarks>Erstellt eine neue Instanz von <see cref="DeleteFileCommand"/>.</remarks>
/// <param name="basePath">Basisverzeichnis des Dummy-Providers.</param>
/// <param name="credentials">Anmeldeinformationen (werden nicht ausgewertet).</param>
/// <param name="logger">Logger für die Protokollierung.</param>
internal sealed class DeleteFileCommand(
    string basePath,
    ProviderCredentials? credentials,
    ILogger<DeleteFileCommand>? logger = null) : IProviderCommand<DeleteFileRequest, DeleteFileResponse>
{
    /// <inheritdoc/>
    public Task<DeleteFileResponse> ExecuteAsync(
        DeleteFileRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        logger?.LogDebug(
            "DeleteFile BasePath: {BasePath}, Credentials: {Credentials}, Path: {Path}",
            basePath,
            DummyPathHelper.FormatCredentialsForLog(credentials),
            request.Path);

        var absolutePath = DummyPathHelper.BuildAbsolutePath(basePath, request.Path);

        if (!File.Exists(absolutePath))
        {
            return Task.FromResult(new DeleteFileResponse
            {
                Success = request.IgnoreIfNotExists,
                Path = request.Path,
                WasDeleted = false,
                WasNotFound = true
            });
        }

        File.Delete(absolutePath);

        return Task.FromResult(new DeleteFileResponse
        {
            Success = true,
            Path = request.Path,
            WasDeleted = true,
            WasNotFound = false
        });
    }
}
