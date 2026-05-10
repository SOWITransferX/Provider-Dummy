// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Commands;

/// <summary>
/// LÃ¤dt eine Datei per Dummy zum Provider hoch.
/// </summary>
internal sealed class UploadFileCommand : IProviderCommand<UploadFileRequest, UploadFileResponse>
{
    private readonly string _basePath;
    private readonly ProviderCredentials? _credentials;

    /// <summary>Erstellt eine neue Instanz von <see cref="UploadFileCommand"/>.</summary>
    /// <param name="basePath">Basis-URL oder Pfad des Dummy-Servers.</param>
    /// <param name="credentials">Anmeldeinformationen fÃ¼r die Verbindung.</param>
    public UploadFileCommand(string basePath, ProviderCredentials? credentials)
    {
        this._basePath = basePath;
        this._credentials = credentials;
    }
    
    /// <inheritdoc/>
    public async Task<UploadFileResponse> ExecuteAsync(
        UploadFileRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        await using var stream = await request.ContentFactory(cancellationToken);
    
        // TODO: Dummy-spezifische Upload-Logik hier implementieren
        // Fortschritt periodisch melden:
        /*
        progress?.Report(new FileProgress
        {
            FileName = Path.GetFileName(request.TargetPath),
            FilePath = request.TargetPath,
            TransferredBytes = bytesTransferred,
            TotalBytes = request.FileSize
        });
        */
    
        return new UploadFileResponse
        {
            Success = true,
            Path = request.TargetPath,
            BytesTransferred = request.FileSize
        };
    }
}
