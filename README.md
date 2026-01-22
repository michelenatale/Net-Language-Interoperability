# Net-Language-Interoperability

**[German-Version](https://github.com/michelenatale/Net-Language-Interoperability/blob/main/Dokuments/GithubReadMe-DE.md)**

Examples and explanations demonstrating how .NET (C#) interacts with native libraries (C/C++), focusing on:

- **P/Invoke**
- **LibraryImport**
- **NativeAOT**
- **Interop from C# → C** and **from C → C#**

The goal of this repository is to illustrate the underlying mechanisms clearly and understandably — not to provide a “perfect” framework, but small, focused building blocks.

---

## Overview

**Short (DE):**  
Kompakte Beispiele für Interoperabilität zwischen .NET und nativen C‑Bibliotheken, mit Schwerpunkt auf Windows x64 und NativeAOT.

**Short (EN):**  
Small, focused examples for .NET ↔ native C interoperability (P/Invoke, LibraryImport, NativeAOT). Windows/x64 oriented.

---

## Table of Contents

- [Goals of the Repository](#goals-of-the-repository)
- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Projects in Detail](#projects-in-detail)
  - [NativeLibrary](#nativelibrary)
  - [LanguageInteroperability/CsToC](#languageinteroperabilitycstoc)
  - [TestLanguageInteroperability](#testlanguageinteroperability)
  - [TestNativeLibrary](#testnativelibrary)
- [How the Components Work Together](#how-the-components-work-together)
- [Quickstart: Build & Run (Windows x64)](#quickstart-build--run-windows-x64)
- [Troubleshooting & Tips](#troubleshooting--tips)
- [Possible Extensions](#possible-extensions)
- [License & Contributions](#license--contributions)

---

## Goals of the Repository

This repository aims to:

- Demonstrate **interoperability mechanisms**, not provide a full framework.
- Offer **small, concrete examples** that are easy to understand and adapt.
- Highlight **common pitfalls** (paths, architecture, ABI, marshaling).
- Serve as a **reference** for using P/Invoke, LibraryImport, or NativeAOT in your own projects.

---

## Architecture Overview

### High-Level Picture

- **C# → C**
  - Classic P/Invoke (`DllImport`)
  - Modern `LibraryImport` (source generator)
- **C → C#**
  - C# library compiled into a native DLL using **NativeAOT**
  - C code links against the generated `.lib` and calls exported functions

### ASCII Diagram

```text
+-----------------------------------+
| TestLanguageInteroperability (C#) |
|                                   |
|  - P/Invoke example               |
|  - LibraryImport example          |
|  - NativeAOT example              |
+-----------------+-----------------+
                  |
                  v
        +---------------------+
        |  NativeLibrary (C#) |
        |  NativeAOT DLL      |
        |  exports e.g.       |
        |  aot_add            |
        +---------------------+

C# -> C:  P/Invoke / LibraryImport
C  -> C#: NativeAOT + .lib + DLL exports
```

---

## Project Structure

High-level structure:

- `/NativeLibrary`  
  NativeAOT example: C# project compiled into a native DLL (including `.lib`).

- `/LanguageInteroperability/CsToC`
  - `/LibraryImport`  
    C example (`crandom.c`) + C# code using the DLL via `LibraryImport`.
  - `/NativeAOT`  
    Instructions and files for building an additional native DLL (`add_aot.dll`) using `NativeLibrary.lib`.

- `/TestLanguageInteroperability`  
  C# console project executing all three approaches (P/Invoke, LibraryImport, NativeAOT).

- `/TestNativeLibrary`  
  Native C/C++ test project linking against `NativeLibrary.lib` and calling exported functions.

- `Proceed-*.txt`  
  Step-by-step instructions (DE/EN).

- `WhatCanBeDeleted.txt`  
  Notes on temporary build folders and generated artifacts.

---

## Projects in Detail

### NativeLibrary

**Summary:**  
A small .NET project compiled into a native DLL using **NativeAOT**.

**Key points:**

- Exported methods using  
  `UnmanagedCallersOnly(EntryPoint = "...")`  
  e.g. `aot_add`.
- Built via:  
  `dotnet publish -c Release -r win-x64`
- Produces:
  - native DLL (`NativeLibrary.dll`)
  - import library (`NativeLibrary.lib`)

**Why it matters:**  
Shows how C# code can be compiled into a native DLL callable from C/C++.

---

### LanguageInteroperability/CsToC

Central area for **C# → C** interoperability.

#### CsToC/LibraryImport

- Demonstrates modern `LibraryImport` (source generator) as an alternative to classic `DllImport`.
- Native side: `crandom.c` → `crandom.dll` (e.g. via `cl /LD crandom.c /Fe:crandom.dll`).
- C# side: `LibraryImport.cs` with signatures like:

  ```csharp
  [LibraryImport(CRandFile)]
  private static partial void fill_random_lib_import(Span<byte> buffer, int length);
  ```

- Uses `Span<byte>` and `stackalloc` to avoid heap allocations.

#### CsToC/NativeAOT

- Shows how to:
  - build `NativeLibrary` via NativeAOT,
  - use the generated `NativeLibrary.lib`,
  - create an additional native DLL (`add_aot.dll`) linking against that `.lib`.
- Demonstrates the flow:  
  **C# (NativeAOT) → .lib → C wrapper → DLL → back to C# or C.**

---

### TestLanguageInteroperability

**Summary:**  
Console test project executing all interoperability variants.

- `TestCsToCPInvoke()`
- `TestCsToCLibraryImport()`
- `TestCsToCNativeAOT()`

**Purpose:**

- Smoke tests: loads native DLLs and calls functions.
- Indirectly verifies:
  - correct build steps,
  - correct output folder placement,
  - correct signatures.

---

### TestNativeLibrary

**Summary:**  
Native C/C++ test project linking against `NativeLibrary.lib`.

**Purpose:**

- Demonstrates **C → C#** via NativeAOT.
- Shows how C code can call functions originally implemented in C#.

---

## How the Components Work Together

- **P/Invoke (`DllImport`)**
  - C# declares external methods.
  - .NET runtime loads a native DLL at runtime.
  - Parameters are automatically marshalled.
  - Signatures, calling convention, and architecture must match.

- **LibraryImport**
  - Modern mechanism using source generators.
  - Offers better performance and compile-time validation.
  - Example uses `Span<byte>` + `stackalloc` for zero-allocation calls.

- **NativeAOT**
  - Compiles a .NET project ahead-of-time into a native DLL.
  - `[UnmanagedCallersOnly]` exposes functions callable from C/C++.
  - Also generates a `.lib` for linking from native projects.
  - Example:  
    `cl /LD aot_add.c NativeLibrary.lib /Fe:add_aot.dll`

---

## Quickstart: Build & Run (Windows x64)

### Requirements

- .NET 10 SDK (or the configured target framework)
- Visual Studio / Build Tools with:
  - `cl` (MSVC compiler)
  - `dumpbin`
  - “x64 Native Tools Command Prompt”
- Windows x64

### Steps (simplified)

1. **Clone the repository**

   ```bash
   git clone https://github.com/michelenatale/Net-Language-Interoperability.git
   cd Net-Language-Interoperability
   ```

2. **Build crandom.dll (native C)**

   ```cmd
   Open x64 Native Tools Command Prompt
   cd LanguageInteroperability\CsToC\LibraryImport
   cl /LD crandom.c /Fe:crandom.dll
   ```

3. **Build NativeLibrary (NativeAOT)**

   ```cmd
   cd NativeLibrary
   dotnet publish -c Release -r win-x64
   ```

4. **Build & run TestLanguageInteroperability**

   ```cmd
   dotnet build TestLanguageInteroperability
   dotnet run --project TestLanguageInteroperability
   ```

   If necessary, copy `crandom.dll`, `NativeLibrary.dll`, and `add_aot.dll` into the output folder.

---

## Troubleshooting & Tips

- **“DllNotFoundException”**
  - Ensure the native DLL is in the .NET output folder.
  - Check architecture: x64 DLL for x64 .NET.

- **“BadImageFormatException”**
  - Architecture mismatch (x86/x64).
  - Check with:  
    `dumpbin /headers <dll> | findstr machine`  
    (`8664` = x64)

- **Path issues**
  - Use `Path.Combine` in C#.
  - Use build macros like `$(ProjectDir)`, `$(TargetFramework)`, `$(Configuration)`.

- **Debugging native components**
  - Build without optimizations (`/Zi /Od`).
  - For performance tests, enable `/O2`.

---

## Possible Extensions

- **CI (GitHub Actions)**
  - Automated builds on `windows-latest`.
  - Smoke tests ensuring DLLs load correctly.

- **Tests**
  - Unit/integration tests for interop calls.
  - ABI tests (struct layout, calling convention).

- **Documentation**
  - Shared “Interop Guidelines” (DE/EN) covering marshaling, ownership, ABI stability.

- **Cross-platform**
  - Additional examples for clang/mingw or Linux/macOS.

---

## License & Contributions

- License: see `LICENSE` in the repository (e.g., MIT).
- Contributions are welcome — for larger changes, consider:
  - `CONTRIBUTING.md`
  - Issue and pull request templates

---

**Summary:**  
This repository provides a compact, practical set of examples demonstrating the three major approaches to .NET ↔ native interoperability: P/Invoke, LibraryImport, and NativeAOT — including both C# → C and C → C#.  
The structure is designed to make the mechanisms easy to understand and reuse in your own projects.
