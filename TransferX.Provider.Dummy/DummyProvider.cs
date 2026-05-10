// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Contracts;
using TransferX.Provider.Abstractions.Metadata;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.TransferX.Provider.Dummy.Commands;
using TransferX.Provider.TransferX.Provider.Dummy.Queries;

namespace TransferX.Provider.Dummy;

/// <summary>
/// Dummy Provider<br/>
/// UnterstÃ¼tzt: Auflisten von Ordnern und Dateien, Hoch- und Herunterladen,<br/>
/// Erstellen und LÃ¶schen von Ordnern und Dateien.
/// </summary>
[ProviderMetadata]
public sealed class DummyProvider : IProvider
{
    private string _basePath = string.Empty;
    private ProviderCredentials? _credentials;

    /// <inheritdoc/>
    public string Name => "Dummy Provider";
    
    /// <inheritdoc/>
    public string Version => "1.0.0";
    
    /// <inheritdoc/>
    public string ProviderType => "Dummy";
    
    /// <inheritdoc/>
    public Task InitializeAsync(ProviderConfigItem config, CancellationToken cancellationToken = default)
    {
        this._basePath = config.BasePath;
        this._credentials = config.Credentials;
    
        // Verbindungsaufbau / Validierung hier implementieren
        return Task.CompletedTask;
    }
    
    /// <inheritdoc/>
    public async Task<ProviderResponse> ExecuteAsync(
        ProviderRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return request switch
        {
            ListFoldersRequest listFolders =>
                await new ListFoldersQuery(this._basePath, this._credentials)
                    .ExecuteAsync(listFolders, progress, cancellationToken),
    
            ListFilesRequest listFiles =>
                await new ListFilesQuery(this._basePath, this._credentials)
                    .ExecuteAsync(listFiles, progress, cancellationToken),
    
            DownloadFileRequest download =>
                await new DownloadFileQuery(this._basePath, this._credentials)
                    .ExecuteAsync(download, progress, cancellationToken),
    
            UploadFileRequest upload =>
                await new UploadFileCommand(this._basePath, this._credentials)
                    .ExecuteAsync(upload, progress, cancellationToken),
    
            CreateFolderRequest createFolder =>
                await new CreateFolderCommand(this._basePath, this._credentials)
                    .ExecuteAsync(createFolder, progress, cancellationToken),
    
            DeleteFileRequest deleteFile =>
                await new DeleteFileCommand(this._basePath, this._credentials)
                    .ExecuteAsync(deleteFile, progress, cancellationToken),
    
            _ => throw new NotSupportedException(
                \$"Request-Typ '{request.GetType().Name}' wird von {this.ProviderType} nicht unterstÃ¼tzt.")
        };
    }
}
