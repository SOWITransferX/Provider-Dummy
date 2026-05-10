// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Commands;

/// <summary>
/// LÃ¶scht eine Datei per Dummy.
/// </summary>
internal sealed class DeleteFileCommand : IProviderCommand<DeleteFileRequest, DeleteFileResponse>
{
    private readonly string _basePath;
    private readonly ProviderCredentials? _credentials;

    /// <summary>Erstellt eine neue Instanz von <see cref="DeleteFileCommand"/>.</summary>
    /// <param name="basePath">Basis-URL oder Pfad des Dummy-Servers.</param>
    /// <param name="credentials">Anmeldeinformationen fÃ¼r die Verbindung.</param>
    public DeleteFileCommand(string basePath, ProviderCredentials? credentials)
    {
        this._basePath = basePath;
        this._credentials = credentials;
    }
    
    /// <inheritdoc/>
    public async Task<DeleteFileResponse> ExecuteAsync(
        DeleteFileRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Dummy-spezifische Datei-LÃ¶schlogik hier implementieren
    
        return new DeleteFileResponse
        {
            Success = true,
            Path = request.Path,
            WasDeleted = true,
            WasNotFound = false
        };
    }
}
