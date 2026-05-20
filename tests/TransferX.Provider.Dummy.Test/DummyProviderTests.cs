// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Contracts;
using TransferX.Provider.Abstractions.Models;
using TransferX.Provider.Abstractions.Requests;

namespace TransferX.Provider.Dummy.Test;

/// <summary>
/// Tests für Metadaten, Initialisierung und Dispatch von <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("DummyProvider")]
public sealed class DummyProviderTests
{
    private DummyProviderFixture _fixture = null!;

    [TestInitialize]
    public void Setup() => this._fixture = new DummyProviderFixture();

    [TestCleanup]
    public void Cleanup() => this._fixture.Dispose();

    [TestMethod]
    public void Properties_ReturnExpectedValues()
    {
        Assert.AreEqual("Dummy Provider", this._fixture.Provider.Name);
        Assert.AreEqual("Dummy", this._fixture.Provider.ProviderType);
        Assert.IsFalse(string.IsNullOrEmpty(this._fixture.Provider.Version));
    }

    [TestMethod]
    public async Task InitializeAsync_ValidDirectory_DoesNotThrow()
    {
        await this._fixture.Provider.InitializeAsync(this._fixture.CreateValidConfig());
    }

    [TestMethod]
    public async Task InitializeAsync_NonExistingDirectory_ThrowsDirectoryNotFoundException()
    {
        var config = new ProviderConfigItem
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
        };

        await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(
            () => this._fixture.Provider.InitializeAsync(config));
    }

    [TestMethod]
    public async Task InitializeAsync_EmptyBasePath_ThrowsArgumentException()
    {
        var config = new ProviderConfigItem { BasePath = "   " };

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => this._fixture.Provider.InitializeAsync(config));
    }

    [TestMethod]
    public async Task InitializeAsync_NullConfig_ThrowsArgumentNullException()
        => await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            () => this._fixture.Provider.InitializeAsync(null!));

    [TestMethod]
    public async Task ExecuteAsync_UnsupportedRequest_ThrowsNotSupportedException()
    {
        await this._fixture.Provider.InitializeAsync(this._fixture.CreateValidConfig());

        await Assert.ThrowsExceptionAsync<NotSupportedException>(
            () => this._fixture.Provider.ExecuteAsync(new UnknownRequest()));
    }

    [TestMethod]
    public async Task ExecuteAsync_WithoutInitialize_ThrowsInvalidOperationException()
        => await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => this._fixture.Provider.ExecuteAsync(new ListFilesRequest { Path = "/" }));

    [TestMethod]
    public async Task ExecuteAsync_PathTraversal_ThrowsUnauthorizedAccessException()
    {
        await this._fixture.Provider.InitializeAsync(this._fixture.CreateValidConfig());

        await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(
            () => this._fixture.Provider.ExecuteAsync(new DownloadFileRequest { Path = "/../outside.txt" }));
    }

    private sealed record UnknownRequest : ProviderRequest;
}
