// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test.Commands;

/// <summary>
/// Tests für Datei-Löschung via <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("DeleteFileCommand")]
public sealed class DeleteFileCommandTests
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
    public async Task ExecuteAsync_ExistingFile_DeletesFileAndReturnsSuccess()
    {
        this._fixture.CreateTestFile("DeleteMe.txt", "Inhalt");
        var request = new DeleteFileRequest { Path = "/DeleteMe.txt" };

        var response = (DeleteFileResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.WasDeleted);
        Assert.IsFalse(response.WasNotFound);
        Assert.AreEqual("/DeleteMe.txt", response.Path);
        Assert.IsFalse(File.Exists(Path.Combine(this._fixture.TestBasePath, "DeleteMe.txt")));
    }

    [TestMethod]
    public async Task ExecuteAsync_NonExistentFile_WithIgnoreIfNotExists_ReturnsSuccess()
    {
        var request = new DeleteFileRequest { Path = "/ghost.txt" };

        var response = (DeleteFileResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.WasDeleted);
        Assert.IsTrue(response.WasNotFound);
    }

    [TestMethod]
    public async Task ExecuteAsync_NonExistentFile_WithoutIgnoreIfNotExists_ReturnsSuccessFalse()
    {
        var request = new DeleteFileRequest { Path = "/ghost.txt", IgnoreIfNotExists = false };

        var response = (DeleteFileResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.WasNotFound);
    }
}
