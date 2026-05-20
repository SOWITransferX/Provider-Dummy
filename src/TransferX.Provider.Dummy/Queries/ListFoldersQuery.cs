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
/// Listet Unterordner in einem Verzeichnis des Dummy-Providers auf.
/// </summary>
/// <remarks>Erstellt eine neue Instanz von <see cref="ListFoldersQuery"/>.</remarks>
/// <param name="basePath">Basisverzeichnis des Dummy-Providers.</param>
/// <param name="credentials">Anmeldeinformationen (werden nicht ausgewertet).</param>
/// <param name="logger">Logger für die Protokollierung.</param>
internal sealed class ListFoldersQuery(
    string basePath,
    ProviderCredentials? credentials,
    ILogger<ListFoldersQuery>? logger = null) : IProviderQuery<ListFoldersRequest, ListFoldersResponse>
{
    /// <inheritdoc/>
    public async Task<ListFoldersResponse> ExecuteAsync(
        ListFoldersRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        logger?.LogDebug(
            "ListFolders BasePath: {BasePath}, Credentials: {Credentials}, Path: {Path}",
            basePath,
            DummyPathHelper.FormatCredentialsForLog(credentials),
            request.Path);

        var absolutePath = DummyPathHelper.BuildAbsolutePath(basePath, request.Path);
        var searchOption = request.Recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        var directories = await Task.Run(
            () => Directory.GetDirectories(absolutePath, "*", searchOption),
            cancellationToken);

        var items = directories
            .Select(dir =>
            {
                var info = new DirectoryInfo(dir);

                return new FolderItem
                {
                    Name = info.Name,
                    Path = DummyPathHelper.ToProviderPath(basePath, dir),
                    LastModified = info.LastWriteTimeUtc,
                    HasChildren = info.EnumerateDirectories().Any()
                };
            })
            .ToList();

        return new ListFoldersResponse
        {
            Items = items,
            SearchedPath = request.Path,
            WasRecursive = request.Recursive
        };
    }
}
