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
/// Listet Dateien in einem Verzeichnis des Dummy-Providers auf.
/// </summary>
/// <remarks>Erstellt eine neue Instanz von <see cref="ListFilesQuery"/>.</remarks>
/// <param name="basePath">Basisverzeichnis des Dummy-Providers.</param>
/// <param name="credentials">Anmeldeinformationen (werden nicht ausgewertet).</param>
/// <param name="logger">Logger für die Protokollierung.</param>
internal sealed class ListFilesQuery(
    string basePath,
    ProviderCredentials? credentials,
    ILogger<ListFilesQuery>? logger = null) : IProviderQuery<ListFilesRequest, ListFilesResponse>
{
    /// <inheritdoc/>
    public async Task<ListFilesResponse> ExecuteAsync(
        ListFilesRequest request,
        IProgress<FileProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        logger?.LogDebug(
            "ListFiles BasePath: {BasePath}, Credentials: {Credentials}, Path: {Path}",
            basePath,
            DummyPathHelper.FormatCredentialsForLog(credentials),
            request.Path);

        var absolutePath = DummyPathHelper.BuildAbsolutePath(basePath, request.Path);
        var searchOption = request.Recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        var files = await Task.Run(
            () => Directory.GetFiles(absolutePath, "*", searchOption),
            cancellationToken);

        var items = files
            .Select(file =>
            {
                var info = new FileInfo(file);

                return new FileItem
                {
                    Name = info.Name,
                    Path = DummyPathHelper.ToProviderPath(basePath, file),
                    LastModified = info.LastWriteTimeUtc,
                    Size = info.Length,
                    ContentType = null
                };
            })
            .ToList();

        return new ListFilesResponse
        {
            Items = items,
            SearchedPath = request.Path,
            WasRecursive = request.Recursive
        };
    }
}
