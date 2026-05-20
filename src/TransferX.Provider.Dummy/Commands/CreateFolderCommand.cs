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
/// Erstellt einen Ordner im Basisverzeichnis des Dummy-Providers.
/// </summary>
/// <remarks>Erstellt eine neue Instanz von <see cref="CreateFolderCommand"/>.</remarks>
/// <param name="basePath">Basisverzeichnis des Dummy-Providers.</param>
/// <param name="credentials">Anmeldeinformationen (werden nicht ausgewertet).</param>
/// <param name="logger">Logger für die Protokollierung.</param>
internal sealed class CreateFolderCommand(
    string basePath,
    ProviderCredentials? credentials,
    ILogger<CreateFolderCommand>? logger = null) : IProviderCommand<CreateFolderRequest, CreateFolderResponse>
{
    /// <inheritdoc/>
    public Task<CreateFolderResponse> ExecuteAsync(
        CreateFolderRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        logger?.LogDebug(
            "CreateFolder BasePath: {BasePath}, Credentials: {Credentials}, Path: {Path}",
            basePath,
            DummyPathHelper.FormatCredentialsForLog(credentials),
            request.Path);

        var absolutePath = DummyPathHelper.BuildAbsolutePath(basePath, request.Path);
        var providerPath = DummyPathHelper.ToProviderPath(basePath, absolutePath);
        var alreadyExists = Directory.Exists(absolutePath);

        if (alreadyExists)
        {
            return Task.FromResult(new CreateFolderResponse
            {
                Success = request.IgnoreIfExists,
                Path = providerPath,
                WasCreated = false,
                ParentFoldersCreated = 0
            });
        }

        var parentPath = Path.GetDirectoryName(absolutePath);
        if (!request.CreateParentFolders
            && !string.IsNullOrEmpty(parentPath)
            && !Directory.Exists(parentPath))
        {
            return Task.FromResult(new CreateFolderResponse
            {
                Success = false,
                Path = providerPath,
                WasCreated = false,
                ParentFoldersCreated = 0
            });
        }

        var parentFoldersCreated = request.CreateParentFolders
            ? DummyPathHelper.CountMissingAncestorDirectories(basePath, absolutePath)
            : 0;

        Directory.CreateDirectory(absolutePath);

        return Task.FromResult(new CreateFolderResponse
        {
            Success = true,
            Path = providerPath,
            WasCreated = true,
            ParentFoldersCreated = parentFoldersCreated
        });
    }
}
