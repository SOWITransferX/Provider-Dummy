// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Commands;

/// <summary>
/// Erstellt einen Ordner per Dummy.
/// </summary>
internal sealed class CreateFolderCommand : IProviderCommand<CreateFolderRequest, CreateFolderResponse>
{
    private readonly string _basePath;
    private readonly ProviderCredentials? _credentials;

    /// <summary>Erstellt eine neue Instanz von <see cref="CreateFolderCommand"/>.</summary>
    /// <param name="basePath">Basis-URL oder Pfad des Dummy-Servers.</param>
    /// <param name="credentials">Anmeldeinformationen für die Verbindung.</param>
    public CreateFolderCommand(string basePath, ProviderCredentials? credentials)
    {
        this._basePath = basePath;
        this._credentials = credentials;
    }

    /// <inheritdoc/>
    public async Task<CreateFolderResponse> ExecuteAsync(
        CreateFolderRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Dummy-spezifische Ordner-Erstellungslogik hier implementieren

        return new CreateFolderResponse
        {
            Success = true,
            Path = request.Path,
            WasCreated = true,
            ParentFoldersCreated = 0
        };
    }
}
