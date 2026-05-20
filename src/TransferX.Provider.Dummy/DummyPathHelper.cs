// SOWI Informatik, www.sowi.ch
// Franz Schönbächler

using TransferX.Provider.Abstractions.Models;

namespace TransferX.Provider.Dummy;

/// <summary>
/// Hilfsmethoden für Pfadauflösung und sicheres Logging im Dummy-Provider.
/// </summary>
internal static class DummyPathHelper
{
    /// <summary>
    /// Kombiniert <paramref name="basePath"/> mit einem relativen Provider-Pfad<br/>
    /// und stellt sicher, dass das Ergebnis innerhalb des Basisverzeichnisses liegt.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">
    /// Der aufgelöste Pfad liegt ausserhalb von <paramref name="basePath"/>.
    /// </exception>
    public static string BuildAbsolutePath(string basePath, string relativePath)
    {
        var normalizedBase = NormalizeBasePath(basePath);
        var absolutePath = Path.GetFullPath(Path.Combine(normalizedBase, relativePath.TrimStart('/')));

        if (!IsWithinBase(absolutePath, normalizedBase))
        {
            throw new UnauthorizedAccessException(
                $"Der Pfad '{relativePath}' liegt ausserhalb des Basisverzeichnisses.");
        }

        return absolutePath;
    }

    /// <summary>
    /// Liefert einen relativen Provider-Pfad (mit führendem <c>/</c>).
    /// </summary>
    public static string ToProviderPath(string basePath, string absolutePath)
        => "/" + Path.GetRelativePath(NormalizeBasePath(basePath), absolutePath).Replace('\\', '/');

    /// <summary>
    /// Formatiert Anmeldedaten für Logs ohne sensible Werte.
    /// </summary>
    public static string FormatCredentialsForLog(ProviderCredentials? credentials)
        => credentials is null ? "(keine)" : "***";

    /// <summary>
    /// Zählt fehlende Verzeichnisse auf dem Pfad zum Ziel (ohne das Ziel selbst),<br/>
    /// begrenzt auf Nachfahren von <paramref name="basePath"/>.
    /// </summary>
    public static int CountMissingAncestorDirectories(string basePath, string absoluteTargetPath)
    {
        var normalizedBase = NormalizeBasePath(basePath);
        var missing = 0;
        var current = Path.GetDirectoryName(absoluteTargetPath);

        while (!string.IsNullOrEmpty(current))
        {
            var fullCurrent = Path.GetFullPath(current);

            if (!IsWithinBase(fullCurrent, normalizedBase)
                || string.Equals(fullCurrent, normalizedBase.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (!Directory.Exists(current))
            {
                missing++;
            }

            current = Path.GetDirectoryName(current);
        }

        return missing;
    }

    /// <summary>
    /// Normalisiert das Basisverzeichnis für konsistente Pfadvergleiche.
    /// </summary>
    internal static string NormalizeBasePath(string basePath)
    {
        var full = Path.GetFullPath(basePath);
        if (!full.EndsWith(Path.DirectorySeparatorChar) && !full.EndsWith(Path.AltDirectorySeparatorChar))
        {
            full += Path.DirectorySeparatorChar;
        }

        return full;
    }

    private static bool IsWithinBase(string absolutePath, string normalizedBaseWithSeparator)
    {
        var full = Path.GetFullPath(absolutePath);
        return full.StartsWith(normalizedBaseWithSeparator, StringComparison.OrdinalIgnoreCase);
    }
}
