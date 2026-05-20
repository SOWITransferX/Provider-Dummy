// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.Extensions.Logging;
using TransferX.Domain.Enumerations;
using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Commands;

/// <summary>
/// Lädt eine Datei per Dummy-Provider in das konfigurierte Basisverzeichnis hoch.
/// </summary>
/// <remarks>Erstellt eine neue Instanz von <see cref="UploadFileCommand"/>.</remarks>
/// <param name="basePath">Basisverzeichnis des Dummy-Providers.</param>
/// <param name="credentials">Anmeldeinformationen (werden nicht ausgewertet).</param>
/// <param name="logger">Logger für die Protokollierung.</param>
internal sealed class UploadFileCommand(
    string basePath,
    ProviderCredentials? credentials,
    ILogger<UploadFileCommand>? logger = null) : IProviderCommand<UploadFileRequest, UploadFileResponse>
{
    private const int BufferSize = 81920;

    /// <inheritdoc/>
    public async Task<UploadFileResponse> ExecuteAsync(
        UploadFileRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        var absolutePath = DummyPathHelper.BuildAbsolutePath(basePath, request.TargetPath);
        var directory = Path.GetDirectoryName(absolutePath)!;

        logger?.LogDebug(
            "Upload BasePath: {BasePath}, Credentials: {Credentials}, TargetPath: {TargetPath}",
            basePath,
            DummyPathHelper.FormatCredentialsForLog(credentials),
            request.TargetPath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var sourceStream = await request.ContentFactory(cancellationToken);
        await using (sourceStream)
        {
            await using var fileStream = new FileStream(
                absolutePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: BufferSize,
                useAsync: true);

            long transferred = 0;
            var buffer = new byte[BufferSize];
            int read;

            while ((read = await sourceStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                transferred += read;

                progress?.Report(new FileProgress
                {
                    FileName = Path.GetFileName(request.TargetPath),
                    FilePath = request.TargetPath,
                    TotalBytes = request.FileSize,
                    TransferredBytes = transferred,
                    Status = FileTransferStatus.Transferring,
                    StartedAt = DateTime.UtcNow
                });
            }
        }

        return new UploadFileResponse
        {
            Success = true,
            Path = request.TargetPath,
            BytesTransferred = new FileInfo(absolutePath).Length
        };
    }
}
