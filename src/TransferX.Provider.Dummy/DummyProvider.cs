// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.Extensions.Logging;
using TransferX.Domain.ValueObjects.Progress;
using TransferX.Provider.Abstractions;
using TransferX.Provider.Abstractions.Contracts;
using TransferX.Provider.Abstractions.Metadata;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Dummy.Commands;
using TransferX.Provider.Dummy.Queries;

namespace TransferX.Provider.Dummy;

/// <summary>
/// Dummy Provider<br/>
/// Unterstützt: Auflisten von Ordnern und Dateien, Hoch- und Herunterladen,<br/>
/// Erstellen und Löschen von Ordnern und Dateien im konfigurierten Basisverzeichnis.
/// </summary>
[ProviderMetadata]
public sealed class DummyProvider : IProvider
{
    private string _basePath = string.Empty;
    private ProviderCredentials? _credentials;
    private bool _isInitialized;
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    /// Erstellt eine Provider-Instanz für die Plugin-Discovery via <c>Activator.CreateInstance</c>.
    /// </summary>
    public DummyProvider()
        : this(null)
    {
    }

    /// <summary>
    /// Erstellt eine Provider-Instanz mit optionalem <see cref="ILoggerFactory"/> für Tests.
    /// </summary>
    /// <param name="loggerFactory">
    /// Optionaler LoggerFactory für die Erstellung von Loggers in den Queries und Commands.
    /// </param>
    public DummyProvider(ILoggerFactory? loggerFactory)
    {
        this._loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public string Name => "Dummy Provider";

    /// <inheritdoc/>
    public string Version =>
        GetType().Assembly
            .GetName()
            .Version?
            .ToString(3) ?? "1.0.0";

    /// <inheritdoc/>
    public string ProviderType => "Dummy";

    /// <inheritdoc/>
    public Task InitializeAsync(ProviderConfigItem config, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(config);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(config.BasePath))
        {
            throw new ArgumentException("BasePath darf nicht leer sein.", nameof(config));
        }

        if (!Directory.Exists(config.BasePath))
        {
            throw new DirectoryNotFoundException(
                $"Das Basis-Verzeichnis '{config.BasePath}' existiert nicht.");
        }

        this._basePath = Path.GetFullPath(config.BasePath);
        this._credentials = config.Credentials;
        this._isInitialized = true;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<ProviderResponse> ExecuteAsync(
        ProviderRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        this.EnsureInitialized();

        return request switch
        {
            ListFoldersRequest listFolders =>
                await new ListFoldersQuery(this._basePath, this._credentials,
                    _loggerFactory?.CreateLogger<ListFoldersQuery>())
                    .ExecuteAsync(listFolders, progress, cancellationToken),

            ListFilesRequest listFiles =>
                await new ListFilesQuery(this._basePath, this._credentials,
                    _loggerFactory?.CreateLogger<ListFilesQuery>())
                    .ExecuteAsync(listFiles, progress, cancellationToken),

            DownloadFileRequest download =>
                await new DownloadFileQuery(this._basePath, this._credentials,
                    _loggerFactory?.CreateLogger<DownloadFileQuery>())
                    .ExecuteAsync(download, progress, cancellationToken),

            UploadFileRequest upload =>
                await new UploadFileCommand(this._basePath, this._credentials,
                    _loggerFactory?.CreateLogger<UploadFileCommand>())
                    .ExecuteAsync(upload, progress, cancellationToken),

            CreateFolderRequest createFolder =>
                await new CreateFolderCommand(this._basePath, this._credentials,
                    _loggerFactory?.CreateLogger<CreateFolderCommand>())
                    .ExecuteAsync(createFolder, progress, cancellationToken),

            DeleteFileRequest deleteFile =>
                await new DeleteFileCommand(this._basePath, this._credentials,
                    _loggerFactory?.CreateLogger<DeleteFileCommand>())
                    .ExecuteAsync(deleteFile, progress, cancellationToken),

            _ => throw new NotSupportedException(
                $"Request-Typ '{request.GetType().Name}' wird von {this.ProviderType} nicht unterstützt.")
        };
    }

    private void EnsureInitialized()
    {
        if (!this._isInitialized)
        {
            throw new InvalidOperationException(
                "Der Provider wurde nicht initialisiert. InitializeAsync muss zuerst aufgerufen werden.");
        }
    }
}
