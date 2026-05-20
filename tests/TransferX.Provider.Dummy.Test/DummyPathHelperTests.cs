// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TransferX.Provider.Dummy.Test;

/// <summary>
/// Tests für <see cref="DummyPathHelper"/> (Sandbox und Pfadnormalisierung).
/// </summary>
[TestClass]
[TestCategory("DummyPathHelper")]
public sealed class DummyPathHelperTests
{
    private string _basePath = null!;

    [TestInitialize]
    public void Setup()
    {
        this._basePath = Path.Combine(Path.GetTempPath(), $"DummyPathHelper_{Guid.NewGuid():N}");
        Directory.CreateDirectory(this._basePath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(this._basePath))
        {
            Directory.Delete(this._basePath, true);
        }
    }

    [TestMethod]
    public void BuildAbsolutePath_ValidRelativePath_ReturnsPathWithinBase()
    {
        var absolute = DummyPathHelper.BuildAbsolutePath(this._basePath, "/sub/file.txt");
        var expected = Path.GetFullPath(Path.Combine(this._basePath, "sub", "file.txt"));

        Assert.AreEqual(expected, absolute);
        Assert.IsTrue(absolute.StartsWith(this._basePath, StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void BuildAbsolutePath_PathTraversal_ThrowsUnauthorizedAccessException()
        => Assert.ThrowsException<UnauthorizedAccessException>(
            () => DummyPathHelper.BuildAbsolutePath(this._basePath, "/../../escape.txt"));

    [TestMethod]
    public void ToProviderPath_AbsolutePath_ReturnsPathStartingWithSlash()
    {
        var absolute = Path.Combine(this._basePath, "ordner", "datei.txt");
        var providerPath = DummyPathHelper.ToProviderPath(this._basePath, absolute);

        Assert.IsTrue(providerPath.StartsWith('/'));
        Assert.AreEqual("/ordner/datei.txt", providerPath);
    }
}
