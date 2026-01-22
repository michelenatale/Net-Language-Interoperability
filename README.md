# Net-Language-Interoperability
LanguageInteroperability for DotNet


```markdown
# Net-Language-Interoperability

Kurz: Beispiele und Anleitungen, wie .NET (C#) mit nativen Bibliotheken (C/C++) interagiert (P/Invoke, LibraryImport, NativeAOT).

## Quickstart (Windows, empfohlen)
Voraussetzungen:
- .NET 10 SDK
- Visual Studio 2022/2026 Build Tools (cl, dumpbin etc.)
- (optional) Administratorrechte für bestimmte Tools

Schnellstart:
1. Repo klonen:
   git clone https://github.com/michelenatale/Net-Language-Interoperability.git
2. Beispiel native DLL bauen:
   - Öffne "x64 Native Tools Command Prompt" und wechsle in `LanguageInteroperability/CsToC/LibraryImport`
   - cl /LD crandom.c /Fe:crandom.dll
   - oder nutze das mitgelieferte Build-Skript (siehe scripts/)
3. Testprojekt bauen und starten:
   dotnet build TestLanguageInteroperability
   dotnet run --project TestLanguageInteroperability

Weitere Infos:
- Siehe `/NativeLibrary/Proceed-NativeLibrary-en.txt` und die anderen Proceed-*.txt für detaillierte Build-Schritte.
- Für NativeAOT: `dotnet publish -c Release -r win-x64` in NativeLibrary.

## Was verbessert werden kann
- Automatisierter Build (CI) für Windows-Umgebung
- Ein zentrales README-Howto mit Copy-to-output/PostBuildEvents
- Einheitliche .gitignore, CONTRIBUTING und Tests

## Mitmachen
- PRs willkommen: bitte beschreibe Ziel und Testschritte.
- Siehe CONTRIBUTING.md (wenn vorhanden).
```
