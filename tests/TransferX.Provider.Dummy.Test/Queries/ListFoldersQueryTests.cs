// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test.Queries;

/// <summary>
/// Tests für Ordner-Auflistung via <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("ListFoldersQuery")]
public sealed class ListFoldersQueryTests
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
    public async Task ExecuteAsync_EmptyDirectory_ReturnsEmptyList()
    {
        var request = new ListFoldersRequest { Path = "/" };

        var response = (ListFoldersResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.AreEqual(0, response.Items.Count);
        Assert.AreEqual("/", response.SearchedPath);
        Assert.IsFalse(response.WasRecursive);
    }

    [TestMethod]
    public async Task ExecuteAsync_TwoFolders_ReturnsBothItems()
    {
        this._fixture.CreateTestFolder("TestFolder1");
        this._fixture.CreateTestFolder("TestFolder2");
        var request = new ListFoldersRequest { Path = "/" };

        var response = (ListFoldersResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.AreEqual(2, response.Items.Count);
    }

    [TestMethod]
    public async Task ExecuteAsync_SubfolderListing_ReturnsChildFolders()
    {
        var parentFolder = this._fixture.CreateTestFolder("parent");
        Directory.CreateDirectory(Path.Combine(parentFolder, "child1"));
        Directory.CreateDirectory(Path.Combine(parentFolder, "child2"));
        var request = new ListFoldersRequest { Path = "/parent" };

        var response = (ListFoldersResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.AreEqual(2, response.Items.Count);
    }

    [TestMethod]
    public async Task ExecuteAsync_ListedItems_AllPathsStartWithSlash()
    {
        this._fixture.CreateTestFolder("folder_a");
        var request = new ListFoldersRequest { Path = "/" };

        var response = (ListFoldersResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Items.All(i => i.Path.StartsWith('/')));
    }
}
