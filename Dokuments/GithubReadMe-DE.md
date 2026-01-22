# Net-Language-Interoperability

**[English-Version](https://github.com/michelenatale/Net-Language-Interoperability/blob/main/README.md)**

Beispiele und Anleitungen, wie .NET (C#) mit nativen Bibliotheken (C/C++) interagiert – mit Fokus auf:

- **P/Invoke**
- **LibraryImport**
- **NativeAOT**
- **Interop von C# → C** und **von C → C#**

Ziel des Repositories ist es, die Mechanismen verständlich und nachvollziehbar zu demonstrieren – nicht ein „perfektes“ Framework, sondern klar erkennbare, kleine Bausteine.


## Kurzüberblick

**Kurz (DE):**  
Kompakte Beispiele für Interoperabilität zwischen .NET und nativen C‑Bibliotheken, mit Schwerpunkt auf Windows x64 und NativeAOT.

**Short (EN):**  
Small, focused examples for .NET ↔ native C interoperability (P/Invoke, LibraryImport, NativeAOT). Windows/x64 oriented.

---

## Inhaltsverzeichnis

- [Ziele des Repositories](#ziele-des-repositories)
- [Architekturüberblick](#architekturüberblick)
- [Projektstruktur](#projektstruktur)
- [Projekte im Detail](#projekte-im-detail)
  - [NativeLibrary](#nativelibrary)
  - [LanguageInteroperability/CsToC](#languageinteroperabilitycstoc)
  - [TestLanguageInteroperability](#testlanguageinteroperability)
  - [TestNativeLibrary](#testnativelibrary)
- [Wie die Teile technisch zusammenspielen](#wie-die-teile-technisch-zusammenspielen)
- [Quickstart: Build & Run (Windows x64)](#quickstart-build--run-windows-x64)
- [Troubleshooting & Tipps](#troubleshooting--tipps)
- [Mögliche Erweiterungen](#mögliche-erweiterungen)
- [Lizenz & Beiträge](#lizenz--beiträge)

---

## Ziele des Repositories

Dieses Repository soll:

- **Mechanismen** der Interoperabilität zeigen, nicht ein fertiges Framework.
- **konkrete, kleine Beispiele** liefern, die man leicht nachvollziehen und anpassen kann.
- **typische Stolpersteine** sichtbar machen (Pfadprobleme, Architektur, ABI, Marshaling).
- als **Referenz** dienen, wenn man P/Invoke, LibraryImport oder NativeAOT in eigenen Projekten einsetzen möchte.

---

## Architekturüberblick

### Grobes Bild

- **C# → C**  
  - Klassisches P/Invoke (`DllImport`)
  - Modernes `LibraryImport` (Source Generator)
- **C → C#**  
  - C#‑Bibliothek wird via **NativeAOT** zu einer nativen DLL kompiliert  
  - C‑Code linkt gegen die erzeugte `.lib` und ruft exportierte Funktionen auf

### ASCII‑Diagramm

```text
+-----------------------------------+
| TestLanguageInteroperability (C#) |
|                                   |
|  - P/Invoke-Beispiel              |
|  - LibraryImport-Beispiel         |
|  - NativeAOT-Beispiel             |
+-----------------+-----------------+
                  |
                  v
        +---------------------+
        |  NativeLibrary (C#) |
        |  NativeAOT-DLL      |
        |  exportiert z.B.    |
        |  aot_add            |
        +---------------------+

C#  -> C:  P/Invoke / LibraryImport
C   -> C#: NativeAOT + .lib + DLL-Exports
```

---

## Projektstruktur

High-Level-Struktur:

- `/NativeLibrary`  
  NativeAOT‑Beispiel: C#‑Projekt, das zu einer nativen DLL kompiliert wird (inkl. `.lib`).

- `/LanguageInteroperability/CsToC`
  - `/LibraryImport`  
    C‑Beispiel (`crandom.c`) + C#‑Code, der die DLL via `LibraryImport` nutzt.
  - `/NativeAOT`  
    Anleitungen und Dateien, um aus `NativeLibrary.lib` eine weitere native DLL (`add_aot.dll`) zu bauen.

- `/TestLanguageInteroperability`  
  C#‑Konsolenprojekt, das alle drei Ansätze (P/Invoke, LibraryImport, NativeAOT) ausführt.

- `/TestNativeLibrary`  
  Native C/C++‑Testprojekt, das gegen `NativeLibrary.lib` linkt und die exportierten Funktionen aufruft.

- `Proceed-*.txt`  
  Schritt‑für‑Schritt‑Anleitungen (DE/EN) für die einzelnen Teile.

- `WhatCanBeDeleted.txt`  
  Hinweise zu temporären Build‑Ordnern und generierten Artefakten.

---

## Projekte im Detail

### NativeLibrary

**Kurz:**  
Ein kleines .NET‑Projekt, das mit **NativeAOT** in eine native DLL kompiliert wird.

**Wesentliche Punkte:**

- Exportierte Methoden mit  
  `UnmanagedCallersOnly(EntryPoint = "...")`  
  z. B. `aot_add`.
- Build über z. B.:  
  `dotnet publish -c Release -r win-x64`
- Erzeugt:
  - native DLL (`NativeLibrary.dll`)
  - Import‑Library (`NativeLibrary.lib`)

**Warum interessant:**  
Zeigt, wie man C#‑Code so kompiliert, dass er aus nativen Anwendungen (C/C++) direkt aufrufbar ist.

---

### LanguageInteroperability/CsToC

Dies ist der zentrale Bereich für **C# → C**‑Interop.

#### CsToC/LibraryImport

- Demonstriert modernes `LibraryImport` (Source‑Generator) als Alternative zu klassischem `DllImport`.
- Native Seite: `crandom.c` → `crandom.dll` (z. B. via `cl /LD crandom.c /Fe:crandom.dll`).
- C#‑Seite: `LibraryImport.cs` mit Signaturen wie:

  ```csharp
  [LibraryImport(CRandFile)]
  private static partial void fill_random_lib_import(Span<byte> buffer, int length);
  ```

- Verwendet `Span<byte>` und `stackalloc`, um Heap‑Allokationen zu vermeiden.

#### CsToC/NativeAOT

- Zeigt, wie man:
  - `NativeLibrary` via NativeAOT baut,
  - die erzeugte `NativeLibrary.lib` verwendet,
  - und daraus eine zusätzliche native DLL (`add_aot.dll`) erstellt, die gegen diese `.lib` linkt.
- Demonstriert damit den Weg:  
  **C# (NativeAOT) → .lib → C‑Wrapper → DLL → zurück nach C# oder C.**

---

### TestLanguageInteroperability

**Kurz:**  
Konsolen‑Testprojekt, das alle Interop‑Varianten nacheinander ausführt.

- `TestCsToCPInvoke()`  
- `TestCsToCLibraryImport()`  
- `TestCsToCNativeAOT()`

**Zweck:**

- „Smoke Tests“: Lädt die nativen DLLs und ruft die Funktionen auf.
- Prüft indirekt:
  - ob die Build‑Schritte korrekt ausgeführt wurden,
  - ob die DLLs im richtigen Output‑Ordner liegen,
  - ob die Signaturen stimmen.

---

### TestNativeLibrary

**Kurz:**  
Native C/C++‑Testprojekt, das gegen `NativeLibrary.lib` linkt.

**Zweck:**

- Demonstriert **C → C#** über NativeAOT.
- Zeigt, wie man aus C‑Code heraus Funktionen aufruft, die ursprünglich in C# implementiert wurden.

---

## Wie die Teile technisch zusammenspielen

- **P/Invoke (`DllImport`)**  
  - C# deklariert externe Methoden.  
  - Die .NET‑Runtime lädt zur Laufzeit eine native DLL (z. B. `crandom.dll`).  
  - Parameter werden automatisch gemarshalled.  
  - Wichtig: Signaturen, Calling Convention und Architektur (x86/x64) müssen exakt passen.

- **LibraryImport**  
  - Moderner Mechanismus mit Source‑Generator.  
  - Bietet bessere Performance‑Optionen und compile‑time‑Validierung.  
  - Im Beispiel: `Span<byte>` + `stackalloc` → keine unnötigen Heap‑Allokationen.

- **NativeAOT**  
  - .NET‑Projekt wird ahead‑of‑time in eine native DLL kompiliert.  
  - Export via `[UnmanagedCallersOnly]` macht Funktionen direkt aus C/C++ aufrufbar.  
  - Zusätzlich wird eine `.lib` erzeugt, gegen die native Projekte linken können.  
  - Beispiel: `cl /LD aot_add.c NativeLibrary.lib /Fe:add_aot.dll`

---

## Quickstart: Build & Run (Windows x64)

### Voraussetzungen

- .NET 10 SDK (oder das im Projekt konfigurierte Target Framework)
- Visual Studio / Build Tools mit:
  - `cl` (MSVC Compiler)
  - `dumpbin`
  - „x64 Native Tools Command Prompt“
- Windows x64

### Schritte (manuell, vereinfacht)

1. **Repository klonen**

   ```bash
   git clone https://github.com/michelenatale/Net-Language-Interoperability.git
   cd Net-Language-Interoperability
   ```

2. **crandom.dll bauen (native C)**

   ```cmd
   x64 Native Tools Command Prompt öffnen
   cd LanguageInteroperability\CsToC\LibraryImport
   cl /LD crandom.c /Fe:crandom.dll
   ```

3. **NativeLibrary (NativeAOT) bauen**

   ```cmd
   cd NativeLibrary
   dotnet publish -c Release -r win-x64
   ```

   Die erzeugten Artefakte (DLL, LIB) liegen im `bin\x64\Release\...`‑Pfad (siehe Projektdatei / Anleitungen).

4. **TestLanguageInteroperability bauen & ausführen**

   ```cmd
   dotnet build TestLanguageInteroperability
   dotnet run --project TestLanguageInteroperability
   ```

   Falls nötig, `crandom.dll`, `NativeLibrary.dll` und ggf. `add_aot.dll` in den Ausgabeordner kopieren (oder Post‑Build‑Events verwenden).

---

## Troubleshooting & Tipps

- **„DllNotFoundException“**  
  - Prüfen, ob die native DLL im Output‑Ordner der .NET‑Anwendung liegt.  
  - Architektur prüfen: x64‑DLL für x64‑.NET.

- **„BadImageFormatException“**  
  - Mismatch zwischen x86/x64.  
  - Mit `dumpbin /headers <dll> | findstr machine` Architektur prüfen (`8664` = x64).

- **Pfadprobleme**  
  - In C# lieber `Path.Combine` statt hart codierter Backslashes.  
  - In Build‑Events Makros wie `$(ProjectDir)`, `$(TargetFramework)`, `$(Configuration)` nutzen.

- **Debugging nativer Komponenten**  
  - Ohne Optimierungen bauen (`/Zi /Od`), um besser debuggen zu können.  
  - Für Performance‑Tests später `/O2` etc. aktivieren.

---

## Mögliche Erweiterungen

- **CI (GitHub Actions)**  
  - Automatischer Build auf `windows-latest`.  
  - Smoke‑Tests, die sicherstellen, dass alle DLLs gebaut und geladen werden können.

- **Tests**  
  - Kleine Unit‑/Integrationstests, die Interop‑Aufrufe verifizieren.  
  - ABI‑Tests (Struct‑Layout, Calling Convention).

- **Dokumentation**  
  - Gemeinsame „Interop‑Guidelines“ (DE/EN) zu Marshaling, Ownership, ABI‑Stabilität.

- **Cross‑Platform**  
  - Ergänzende Beispiele für clang/mingw oder Linux/macOS, falls gewünscht.

---

## Lizenz & Beiträge

- Aktuell: Lizenz laut `LICENSE` im Repository (z. B. MIT, falls gesetzt).  
- Beiträge sind willkommen – für größere Änderungen empfiehlt sich:
  - `CONTRIBUTING.md`
  - Issue‑ und Pull‑Request‑Vorlagen

---

**Kurzfassung:**  
Dieses Repository ist ein bewusst kompaktes, praxisorientiertes Set von Beispielen, das die drei wichtigsten Wege der .NET ↔ native Interoperabilität demonstriert: P/Invoke, LibraryImport und NativeAOT – inklusive C# → C und C → C#.  
Die Struktur ist so gewählt, dass man die Mechanismen klar erkennen und in eigene Projekte übertragen kann.


