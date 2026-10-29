# TransferX.Provider.Dummy

`TransferX.Provider.Dummy` ist ein Referenz- und Test-Provider für das Framework TransferX.
Das Projekt dient als Vorlage für Entwickler, die eigene Provider-Plugins für TransferX erstellen möchten.

Der Dummy-Provider implementiert dieselben grundlegenden Operationen wie produktive Provider, arbeitet jedoch ausschliesslich auf einem konfigurierten **lokalen Basisverzeichnis** (`ProviderConfigItem.BasePath`).

Entwickler können dieses Repository:

* direkt als Ausgangsbasis verwenden
* forken und erweitern
* herunterladen und daraus einen eigenen Provider entwickeln

Das Ziel ist es, die Entwicklung neuer TransferX-Provider zu vereinfachen und eine funktionierende Referenzimplementierung bereitzustellen.

## Was ist TransferX?

TransferX ist ein modulares Framework für Datei- und Datentransfers zwischen unterschiedlichen Systemen und Datenquellen.

TransferX ermöglicht:

* Daten zwischen Providern zu kopieren oder zu synchronisieren
* unterschiedliche Systeme miteinander zu verbinden
* eigene Provider-Plugins zu entwickeln
* Transfer-Strategien wie Copy, Move oder Sync flexibel zu erweitern

Weitere Informationen zur Architektur und zum Gesamtsystem befinden sich in der Hauptdokumentation des Projekts. 

## Unterstützte Operationen

- Ordner und Dateien auflisten (`ListFolders`, `ListFiles`)
- Datei hoch- und herunterladen (`UploadFile`, `DownloadFile`)
- Ordner erstellen (`CreateFolder`)
- Datei löschen (`DeleteFile`)

## Konfiguration

| Eigenschaft   | Beschreibung                                                |
| ------------- | ----------------------------------------------------------- |
| `BasePath`    | Existierendes Root-Verzeichnis (Pflicht)                    |
| `Credentials` | Optional; werden nicht ausgewertet und nur maskiert geloggt |

## Abhängigkeiten

| Paket | Version | Quelle |
| --- | --- | --- |
| `TransferX.Provider.Abstractions` | `3.0.0` | nuget.org |

Transitiv folgt `TransferX.Domain` in derselben Version.
Das Plugin referenziert `TransferX.Core` nicht.

Die Plugin-Version (`IProvider.Version`, Projektdatei) bleibt `1.0.0` und ist nicht an die SDK-Version gekoppelt.

## Projektstruktur

- `src/TransferX.Provider.Dummy` – Provider-Implementierung
- `tests/TransferX.Provider.Dummy.Test` – MSTest-Tests

## Build & Test

```bash
dotnet build TransferX.Provider.Dummy.sln
dotnet test TransferX.Provider.Dummy.sln
```

## Verwendung als Vorlage für eigene Provider

Der Dummy-Provider zeigt die grundlegende Struktur eines TransferX-Providers und kann als Ausgangspunkt für eigene Implementierungen verwendet werden.

Typische Vorgehensweise:

1. Repository forken oder herunterladen
2. Projekt und Namespace umbenennen
3. Eigene Provider-Logik implementieren
4. Provider in TransferX registrieren
5. Tests erweitern und anpassen

Mögliche Zielsysteme für eigene Provider:

* FTP / SFTP
* webDAV
* Cloud Storage
* REST APIs
* SQL Datenbanken
* ERP-Systeme
* Lokale Dateisysteme

## Hinweise

* Pfade in Requests sind provider-relativ und beginnen typischerweise mit `/`
  (z. B. `/ordner/datei.txt`)
* Pfade mit `..` oder absoluten Segmenten ausserhalb von `BasePath` führen zu `UnauthorizedAccessException`
* `ExecuteAsync` ohne vorheriges `InitializeAsync` wirft `InvalidOperationException`
* Nicht existierende Downloads werfen `FileNotFoundException`
* Fehlgeschlagene Löschvorgänge liefern `Success = false`
* Der Provider ist primär für Entwicklung, Referenzimplementierungen und automatisierte Tests gedacht
