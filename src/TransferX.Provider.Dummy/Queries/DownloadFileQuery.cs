// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.Extensions.Logging;
using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Queries;

/// <summary>
/// Lädt eine Datei aus dem Basisverzeichnis des Dummy-Providers herunter.
/// </summary>
/// <remarks>
/// Erstellt eine neue Instanz von <see cref="DownloadFileQuery"/>.<br/>
/// Der zurückgegebene Stream muss vom Aufrufer disposed werden.
/// </remarks>
/// <param name="basePath">Basisverzeichnis des Dummy-Providers.</param>
/// <param name="credentials">Anmeldeinformationen (werden nicht ausgewertet).</param>
/// <param name="logger">Logger für die Protokollierung.</param>
internal sealed class DownloadFileQuery(
    string basePath,
    ProviderCredentials? credentials,
    ILogger<DownloadFileQuery>? logger = null) : IProviderQuery<DownloadFileRequest, DownloadFileResponse>
{
    private const int BufferSize = 81920;

    /// <inheritdoc/>
    public Task<DownloadFileResponse> ExecuteAsync(
        DownloadFileRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        logger?.LogDebug(
            "Download BasePath: {BasePath}, Credentials: {Credentials}, Path: {Path}",
            basePath,
            DummyPathHelper.FormatCredentialsForLog(credentials),
            request.Path);

        var absolutePath = DummyPathHelper.BuildAbsolutePath(basePath, request.Path);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"Die Datei '{request.Path}' wurde nicht gefunden.", absolutePath);
        }

        var fileInfo = new FileInfo(absolutePath);
        var stream = new FileStream(
            absolutePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: BufferSize,
            useAsync: true);

        progress?.Report(new FileProgress
        {
            FileName = Path.GetFileName(request.Path),
            FilePath = request.Path,
            TransferredBytes = fileInfo.Length,
            TotalBytes = fileInfo.Length
        });

        return Task.FromResult(new DownloadFileResponse
        {
            Stream = stream,
            FileSize = fileInfo.Length
        });
    }
}
