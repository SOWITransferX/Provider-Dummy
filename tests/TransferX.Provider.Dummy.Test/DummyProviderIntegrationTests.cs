// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransferX.Provider.Abstractions.Requests;
using TransferX.Provider.Abstractions.Responses;

namespace TransferX.Provider.Dummy.Test;

/// <summary>
/// Integrationstests für typische Workflows mit <see cref="DummyProvider"/>.
/// </summary>
[TestClass]
[TestCategory("DummyProvider")]
[TestCategory("Integration")]
public sealed class DummyProviderIntegrationTests
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
    public async Task CompleteWorkflow_CreateFolderUploadDownloadDelete_ShouldSucceed()
    {
        const string folderPath = "/workflow_test";
        const string filePath = "/workflow_test/data.txt";
        const string fileContent = "Integration test data";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        var createResponse = (CreateFolderResponse)await this._fixture.Provider.ExecuteAsync(
            new CreateFolderRequest { Path = folderPath });
        Assert.IsTrue(createResponse.Success);

        var uploadResponse = (UploadFileResponse)await this._fixture.Provider.ExecuteAsync(
            new UploadFileRequest
            {
                TargetPath = filePath,
                FileSize = contentBytes.Length,
                ContentFactory = _ => Task.FromResult<Stream>(new MemoryStream(contentBytes))
            });
        Assert.IsTrue(uploadResponse.Success);

        var downloadResponse = (DownloadFileResponse)await this._fixture.Provider.ExecuteAsync(
            new DownloadFileRequest { Path = filePath });
        await using (downloadResponse.Stream)
        using (var reader = new StreamReader(downloadResponse.Stream))
        {
            Assert.AreEqual(fileContent, await reader.ReadToEndAsync());
        }

        var deleteResponse = (DeleteFileResponse)await this._fixture.Provider.ExecuteAsync(
            new DeleteFileRequest { Path = filePath });
        Assert.IsTrue(deleteResponse.Success);
        Assert.IsFalse(File.Exists(Path.Combine(this._fixture.TestBasePath, "workflow_test", "data.txt")));
    }

    [TestMethod]
    public async Task ListingAfterModifications_ShouldReflectChanges()
    {
        const string filePath = "/listed.txt";
        var content = "Test"u8.ToArray();

        await this._fixture.Provider.ExecuteAsync(new UploadFileRequest
        {
            TargetPath = filePath,
            FileSize = content.Length,
            ContentFactory = _ => Task.FromResult<Stream>(new MemoryStream(content))
        });

        var listBefore = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(
            new ListFilesRequest { Path = "/" });
        Assert.AreEqual(1, listBefore.Items.Count);

        await this._fixture.Provider.ExecuteAsync(new DeleteFileRequest { Path = filePath });

        var listAfter = (ListFilesResponse)await this._fixture.Provider.ExecuteAsync(
            new ListFilesRequest { Path = "/" });
        Assert.AreEqual(0, listAfter.Items.Count);
    }
}
