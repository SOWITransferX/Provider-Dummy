// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test.Commands;

/// <summary>
/// Tests für Datei-Upload via <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("UploadFileCommand")]
public sealed class UploadFileCommandTests
{
    private DummyProviderFixture _fixture = null!;

    [TestInitialize]
    public async Task Setup()
    {
        this._fixture = new DummyProviderFixture();
        await this._fixture.Provider.InitializeAsync(this._fixture.CreateValidConfig());
    }

    [TestCleanup]
    public void Cleanup() => this._fixture.Dispose();

    [TestMethod]
    public async Task ExecuteAsync_ValidContent_WritesFileAndReturnsSuccess()
    {
        var content = "Hallo Dummy!"u8.ToArray();
        var request = new UploadFileRequest
        {
            TargetPath = "/upload.txt",
            FileSize = content.Length,
            ContentFactory = _ => Task.FromResult<Stream>(new MemoryStream(content))
        };

        var response = (UploadFileResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.AreEqual("/upload.txt", response.Path);
        Assert.AreEqual(content.Length, response.BytesTransferred);
        Assert.IsTrue(File.Exists(Path.Combine(this._fixture.TestBasePath, "upload.txt")));
    }

    [TestMethod]
    public async Task ExecuteAsync_NonExistingSubDirectory_CreatesDirectoryAutomatically()
    {
        var content = "sub"u8.ToArray();
        var request = new UploadFileRequest
        {
            TargetPath = "/sub/dir/datei.txt",
            FileSize = content.Length,
            ContentFactory = _ => Task.FromResult<Stream>(new MemoryStream(content))
        };

        var response = (UploadFileResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(File.Exists(Path.Combine(this._fixture.TestBasePath, "sub", "dir", "datei.txt")));
    }

    [TestMethod]
    public async Task ExecuteAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var content = new byte[1024];
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var request = new UploadFileRequest
        {
            TargetPath = "/cancel.txt",
            FileSize = content.Length,
            ContentFactory = _ => Task.FromResult<Stream>(new MemoryStream(content))
        };

        await Assert.ThrowsExceptionAsync<OperationCanceledException>(
            () => this._fixture.Provider.ExecuteAsync(request, cancellationToken: cts.Token));
    }
}
