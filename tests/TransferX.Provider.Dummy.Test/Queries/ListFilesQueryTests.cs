// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test.Queries;

/// <summary>
/// Tests für Datei-Auflistung via <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("ListFilesQuery")]
public sealed class ListFilesQueryTests
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
        var request = new ListFilesRequest { Path = "/" };

        var response = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.AreEqual(0, response.Items.Count);
        Assert.AreEqual("/", response.SearchedPath);
        Assert.IsFalse(response.WasRecursive);
    }

    [TestMethod]
    public async Task ExecuteAsync_Recursive_ReturnsFilesFromSubfolders()
    {
        this._fixture.CreateTestFile("root.txt", "r");
        var subfolder = this._fixture.CreateTestFolder("sub");
        File.WriteAllText(Path.Combine(subfolder, "nested.txt"), "n");
        var request = new ListFilesRequest { Path = "/", Recursive = true };

        var response = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.WasRecursive);
        Assert.AreEqual(2, response.Items.Count);
    }

    [TestMethod]
    public async Task ExecuteAsync_TwoFiles_ReturnsBothItems()
    {
        this._fixture.CreateTestFile("file1.txt", "Content 1");
        this._fixture.CreateTestFile("file2.txt", "Content 2");
        var request = new ListFilesRequest { Path = "/" };

        var response = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.AreEqual(2, response.Items.Count);
        Assert.IsTrue(response.Items.Any(i => i.Name == "file1.txt"));
        Assert.IsTrue(response.Items.Any(i => i.Name == "file2.txt"));
    }

    [TestMethod]
    public async Task ExecuteAsync_FilesInSubfolder_ReturnsOnlySubfolderFiles()
    {
        var subfolder = this._fixture.CreateTestFolder("subfolder");
        File.WriteAllText(Path.Combine(subfolder, "a.txt"), "a");
        File.WriteAllText(Path.Combine(subfolder, "b.txt"), "b");
        var request = new ListFilesRequest { Path = "/subfolder" };

        var response = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.AreEqual(2, response.Items.Count);
    }

    [TestMethod]
    public async Task ExecuteAsync_ListedItems_AllPathsStartWithSlash()
    {
        this._fixture.CreateTestFile("listed.txt", "x");
        var request = new ListFilesRequest { Path = "/" };

        var response = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(request);

        Assert.IsTrue(response.Items.All(i => i.Path.StartsWith('/')));
    }
}
