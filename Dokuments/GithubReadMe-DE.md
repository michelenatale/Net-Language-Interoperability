# Net-Language-Interoperability

---
> [!IMPORTANT]
> ***Achtung:*** Das ganze Projekt ist noch in Bearbeitung, und entspricht nicht dem Finale.
---

**[English-Version](https://github.com/michelenatale/Net-Language-Interoperability/blob/main/README.md)**

Kompakte Beispiele, wie .NET (C#) mit nativen Bibliotheken (C/C++) interagiert — mit:

- **P/Invoke**
- **LibraryImport**
- **NativeAOT**
- **Interop von C# → C** und **von C → C#**

Dieses Repository zeigt die Mechanismen klar und nachvollziehbar — kein Framework, sondern praktische Bausteine.

---

## Inhaltsverzeichnis

- [Quickstart](#quickstart-windows-x64)
- [Architekturüberblick](#architekturüberblick)
- [Welche Technik wofür](#welche-technik-wofür)
- [Wie die Teile technisch zusammenspielen](#wie-die-teile-technisch-zusammenspielen)
- [Projektstruktur](#projektstruktur)
- [Code-Beispiele](#codebeispiele)
  - [P/Invoke](#-pinvoke)
  - [LibraryImport](#-libraryimport)
  - [NativeAOT Export](#-nativeaot-export)
  - [C ruft NativeAOT auf](#-c-ruft-nativeaot-auf)
- [Troubleshooting](#troubleshooting)
- [Lizenz  Beiträge](#lizenz--beiträge)
- [Zusammenfassung](#zusammenfassung)


---

## Quickstart (Windows x64)

### 1. Repository klonen
```bash
git clone https://github.com/michelenatale/Net-Language-Interoperability.git
cd Net-Language-Interoperability
```

### 2. Native C‑Bibliothek bauen (`crandom.dll`)
```cmd
cd LanguageInteroperability/CsToC/LibraryImport
cl /LD crandom.c /Fe:crandom.dll
```

### 3. NativeAOT‑Bibliothek bauen (`NativeLibrary.dll`)
```cmd
cd NativeLibrary
dotnet publish -c Release -r win-x64
```

### 4. Tests ausführen
```cmd
dotnet run --project TestLanguageInteroperability
```

### Optional: C‑Testprojekt
```cmd
cd TestNativeLibrary
cl main.c NativeLibrary.lib /Fe:test.exe
```

---

## Architekturüberblick

```text
                   +------------------------------+
                   | TestLanguageInteroperability |
                   |            (C#)              |
                   |------------------------------|
                   | - P/Invoke                   |
                   | - LibraryImport              |
                   | - NativeAOT (C# → C)         |
                   +---------------+--------------+
                                   |
                                   |  C# → C
                                   v
                     +---------------------------+
                     |   Native C Libraries      |
                     |  (crandom.dll / .so)      |
                     +---------------------------+

                                   ^
                                   |  C → C#
                                   |
                     +---------------------------+
                     |     NativeLibrary (C#)    |
                     |     NativeAOT DLL/.lib    |
                     |  exportiert Funktionen     |
                     +---------------------------+

                                   ^
                                   |  C → C#
                                   |
                     +---------------------------+
                     |     TestNativeLibrary     |
                     |            (C)            |
                     |  ruft NativeAOT‑Exports   |
                     +---------------------------+
```
---

## Welche Technik wofür?

| Technik | Richtung | Vorteile | Typische Nutzung |
|--------|----------|----------|------------------|
| **P/Invoke (`DllImport`)** | C# → C | Einfach, etabliert | Zugriff auf C‑APIs, Win32 |
| **LibraryImport** | C# → C | Schnell, compile‑time‑validiert | Performance‑kritische Interop |
| **NativeAOT (C# → native DLL/.so)** | C# → C | C# wird zu echter nativer DLL | Wenn C‑Programme C#‑Code aufrufen sollen |
| **NativeAOT + .lib Export** | C → C# | C‑Compiler linkt gegen C# | Integration in C/C++‑Code |
| **C‑Wrapper → C#** | C# → C → C# | Volle ABI‑Kontrolle | Komplexe Interop‑Szenarien |

---

## Wie die Teile technisch zusammenspielen

### 🔹 P/Invoke (C# → C)
- C# deklariert eine externe Funktion mit `DllImport`.
- Die .NET‑Runtime lädt die native DLL zur Laufzeit.
- Parameter werden automatisch gemarshalled.
- Wichtig: Signaturen, Calling Convention und Architektur (x86/x64) müssen exakt übereinstimmen.

### 🔹 LibraryImport (C# → C)
- Moderner Mechanismus mit Source‑Generator.
- Marshaling wird zur Compile‑Time validiert.
- Weniger Overhead als P/Invoke.
- Unterstützt `Span<T>` und `stackalloc` für Zero‑Allocation‑Interop.

### 🔹 NativeAOT (C# → native DLL/.so)
- C#‑Code wird ahead‑of‑time in eine echte native Bibliothek kompiliert.
- Funktionen werden über `[UnmanagedCallersOnly]` exportiert.
- Native Programme (C/C++) können direkt dagegen linken.
- Zusätzlich wird eine `.lib` (Windows) oder `.a` (Linux/macOS) erzeugt.

### 🔹 C → C# über NativeAOT
- C‑Code linkt gegen die von NativeAOT erzeugte `.lib`.
- Exportierte Funktionen verhalten sich wie normale C‑Funktionen.
- Ideal, wenn bestehende C‑Programme C#‑Logik nutzen sollen.

### 🔹 Zusammenspiel im Repository
- `crandom.c` → wird zu `crandom.dll` → von C# via P/Invoke/LibraryImport genutzt.
- `NativeLibrary` (C#) → wird via NativeAOT zu `NativeLibrary.dll` + `.lib`.
- `TestNativeLibrary` (C) → linkt gegen `NativeLibrary.lib` → ruft C#‑Funktionen auf.
- `TestLanguageInteroperability` (C#) → ruft sowohl C‑ als auch NativeAOT‑Exports auf.

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

## Code‑Beispiele

### ✔ P/Invoke
```csharp
[DllImport("crandom.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern void fill_random(byte[] buffer, int length);
```

### ✔ LibraryImport
```csharp
[LibraryImport("crandom.dll")]
private static partial void fill_random_lib_import(Span<byte> buffer, int length);
```

### ✔ NativeAOT Export
```csharp
[UnmanagedCallersOnly(EntryPoint = "aot_add")]
public static int Add(int a, int b) => a + b;
```

### ✔ C ruft NativeAOT auf
```c
__declspec(dllimport) int aot_add(int a, int b);
```

---

## Troubleshooting

- **DllNotFoundException** → DLL fehlt  
- **BadImageFormatException** → Architektur falsch  
- `dumpbin /headers` hilft  
- Pfade über MSBuild‑Makros setzen  

---

## Lizenz & Beiträge

Siehe `LICENSE`.  
Beiträge willkommen.

---

## Zusammenfassung

Dieses Repository ist ein bewusst kompaktes, praxisorientiertes Set von Beispielen, das die drei wichtigsten Wege der .NET ↔ native Interoperabilität demonstriert: 
- P/Invoke
- LibraryImport
- NativeAOT - Inklusive C# → C und C → C#.

Die Struktur ist so gewählt, dass man die Mechanismen klar erkennen und in eigene Projekte übertragen kann.



