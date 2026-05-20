// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Provider.Abstractions.Models;

namespace TransferX.Provider.Dummy.Test;

/// <summary>
/// Wiederverwendbare Test-Fixture für <see cref="DummyProvider"/>-Tests.<br/>
/// Verwaltet ein temporäres Basisverzeichnis und Hilfsmethoden für Dateien/Streams.
/// </summary>
public sealed class DummyProviderFixture : IDisposable
{
    /// <summary>Initialisierter Dummy-Provider.</summary>
    public DummyProvider Provider { get; }

    /// <summary>Absoluter Pfad des temporären Test-Basisverzeichnisses.</summary>
    public string TestBasePath { get; }

    /// <summary>
    /// Erstellt ein temporäres Verzeichnis und einen neuen <see cref="DummyProvider"/>.
    /// </summary>
    public DummyProviderFixture()
    {
        this.TestBasePath = Path.Combine(Path.GetTempPath(), $"DummyProviderTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.TestBasePath);
        this.Provider = new DummyProvider();
    }

    /// <summary>
    /// Erstellt eine gültige Provider-Konfiguration.
    /// </summary>
    public ProviderConfigItem CreateValidConfig(string? basePath = null)
        => new()
        {
            BasePath = basePath ?? this.TestBasePath,
            Credentials = null
        };

    /// <summary>
    /// Schreibt eine Testdatei ins Basisverzeichnis (oder Unterpfad).
    /// </summary>
    public string CreateTestFile(string fileName, string content = "Test content")
    {
        var filePath = Path.Combine(this.TestBasePath, fileName);
        var directory = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(directory);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    /// <summary>
    /// Erstellt einen Testordner unterhalb des Basisverzeichnisses.
    /// </summary>
    public string CreateTestFolder(string folderName)
    {
        var folderPath = Path.Combine(this.TestBasePath, folderName);
        Directory.CreateDirectory(folderPath);
        return folderPath;
    }

    /// <summary>
    /// Erstellt einen <see cref="MemoryStream"/> mit Textinhalt für Upload-Tests.
    /// </summary>
    public static MemoryStream CreateContentStream(string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Directory.Exists(this.TestBasePath))
        {
            try
            {
                Directory.Delete(this.TestBasePath, true);
            }
            catch
            {
                // Cleanup-Fehler in Tests ignorieren
            }
        }
    }
}
