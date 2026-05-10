// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Queries;

/// <summary>
/// LÃ¤dt eine Datei per Dummy herunter.
/// </summary>
internal sealed class DownloadFileQuery : IProviderQuery<DownloadFileRequest, DownloadFileResponse>
{
    private readonly string _basePath;
    private readonly ProviderCredentials? _credentials;

    /// <summary>Erstellt eine neue Instanz von <see cref="DownloadFileQuery"/>.</summary>
    /// <param name="basePath">Basis-URL oder Pfad des Dummy-Servers.</param>
    /// <param name="credentials">Anmeldeinformationen fÃ¼r die Verbindung.</param>
    public DownloadFileQuery(string basePath, ProviderCredentials? credentials)
    {
        this._basePath = basePath;
        this._credentials = credentials;
    }
    
    /// <inheritdoc/>
    public async Task<DownloadFileResponse> ExecuteAsync(
        DownloadFileRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Dummy-spezifische Download-Logik hier implementieren
        // Fortschritt periodisch melden:
        /*
        progress?.Report(new FileProgress
        {
            FileName = Path.GetFileName(request.Path),
            FilePath = request.Path,
            TransferredBytes = bytesTransferred,
            TotalBytes = totalBytes
        });
        */
    
        // TODO: Echten Stream zurÃ¼ckgeben
        var stream = new MemoryStream();
    
        return new DownloadFileResponse
        {
            Stream = stream,
            FileSize = 0,
            ContentType = null
        };
    }
}
