# Net-Language-Interoperability

Kompakte Beispiele, wie .NET (C#) mit nativen Bibliotheken (C/C++) interagiert — mit:

- **P/Invoke**
- **LibraryImport**
- **NativeAOT**
- **Interop von C# → C** und **von C → C#**

Dieses Repository zeigt die Mechanismen klar und nachvollziehbar — kein Framework, sondern praktische Bausteine.

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

## Projektstruktur

- `/NativeLibrary` — NativeAOT‑Beispiel  
- `/LanguageInteroperability/CsToC`
  - `/LibraryImport` — C‑Beispiel + C#‑Interop  
  - `/NativeAOT` — zusätzliche native DLL (`add_aot.dll`)  
- `/TestLanguageInteroperability` — C#‑Tests  
- `/TestNativeLibrary` — C‑Tests  
- `Proceed-*.txt` — Schritt‑für‑Schritt  
- `WhatCanBeDeleted.txt` — Artefakte  

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
