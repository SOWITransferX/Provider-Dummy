// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test.Commands;

/// <summary>
/// Tests für Ordner-Erstellung via <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("CreateFolderCommand")]
public sealed class CreateFolderCommandTests
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
    public async Task ExecuteAsync_CreateFolderInRoot_CreatesDirectory()
    {
        var request = new CreateFolderRequest { Path = "/neuer_ordner" };

        var response = (CreateFolderResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.WasCreated);
        Assert.AreEqual(0, response.ParentFoldersCreated);
        Assert.IsTrue(Directory.Exists(Path.Combine(this._fixture.TestBasePath, "neuer_ordner")));
    }

    [TestMethod]
    public async Task ExecuteAsync_ExistingFolder_WithIgnoreIfExists_ReturnsSuccessWithoutRecreating()
    {
        this._fixture.CreateTestFolder("existing");
        var request = new CreateFolderRequest { Path = "/existing" };

        var response = (CreateFolderResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.WasCreated);
    }

    [TestMethod]
    public async Task ExecuteAsync_CreateFolderWithParentFolders_CreatesAncestorsAndReportsCount()
    {
        var request = new CreateFolderRequest
        {
            Path = "/a/b/c",
            CreateParentFolders = true
        };

        var response = (CreateFolderResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.WasCreated);
        Assert.AreEqual(2, response.ParentFoldersCreated);
        Assert.IsTrue(Directory.Exists(Path.Combine(this._fixture.TestBasePath, "a", "b", "c")));
    }

    [TestMethod]
    public async Task ExecuteAsync_CreateFolderInSubfolder_CreatesNestedDirectory()
    {
        this._fixture.CreateTestFolder("parent");
        var request = new CreateFolderRequest { Path = "/parent/child" };

        var response = (CreateFolderResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(Directory.Exists(Path.Combine(this._fixture.TestBasePath, "parent", "child")));
    }
}
