// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Queries;

/// <summary>
/// Listet Ordner in einem Dummy-Verzeichnis auf.
/// </summary>
internal sealed class ListFoldersQuery : IProviderQuery<ListFoldersRequest, ListFoldersResponse>
{
    private readonly string _basePath;
    private readonly ProviderCredentials? _credentials;

    /// <summary>Erstellt eine neue Instanz von <see cref="ListFoldersQuery"/>.</summary>
    /// <param name="basePath">Basis-URL oder Pfad des Dummy-Servers.</param>
    /// <param name="credentials">Anmeldeinformationen fÃ¼r die Verbindung.</param>
    public ListFoldersQuery(string basePath, ProviderCredentials? credentials)
    {
        this._basePath = basePath;
        this._credentials = credentials;
    }
    
    /// <inheritdoc/>
    public async Task<ListFoldersResponse> ExecuteAsync(
        ListFoldersRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Dummy-spezifische Logik zum Auflisten von Ordnern hier implementieren
        var items = new List<FolderItem>();
    
        return new ListFoldersResponse
        {
            Items = items,
            SearchedPath = request.Path,
            WasRecursive = request.Recursive
        };
    }
}
