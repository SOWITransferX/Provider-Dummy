// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test.Queries;

/// <summary>
/// Tests für Datei-Download via <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("DownloadFileQuery")]
public sealed class DownloadFileQueryTests
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
    public async Task ExecuteAsync_ExistingFile_ReturnsStreamWithCorrectContent()
    {
        const string fileContent = "Download test content";
        this._fixture.CreateTestFile("download.txt", fileContent);
        var request = new DownloadFileRequest { Path = "/download.txt" };

        var response = (DownloadFileResponse)await this._fixture.Provider.ExecuteAsync(request);
        await using var stream = response.Stream;
        using var reader = new StreamReader(stream);

        Assert.AreEqual(fileContent.Length, response.FileSize);
        Assert.AreEqual(fileContent, await reader.ReadToEndAsync());
    }

    [TestMethod]
    public async Task ExecuteAsync_NonExistingFile_ThrowsFileNotFoundException()
        => await Assert.ThrowsExceptionAsync<FileNotFoundException>(
            () => this._fixture.Provider.ExecuteAsync(new DownloadFileRequest { Path = "/ghost.txt" }));
}
