# Net-Language-Interoperability

---
> [!IMPORTANT]
> ***Attention:*** The entire project is still in progress and is not yet complete.

---

**[German-Version](https://github.com/michelenatale/Net-Language-Interoperability/blob/main/Dokuments/GithubReadMe-DE.md)**

Small, focused examples demonstrating how .NET (C#) interacts with native libraries (C/C++), using:

- **P/Invoke**
- **LibraryImport**
- **NativeAOT**
- **Interop from C# → C** and **from C → C#**

This repository is designed to illustrate the mechanisms clearly — not to provide a full framework, but compact, practical building blocks.

---

## Table of Contents

- [Quickstart](#-quickstart-windows-x64)
- [Architecture Overview](#architecture-overview)
- [Which Technique for What](#which-technique-for-what)
- [How the Components Work Together](#which-technique-for-what)
- [Project Structure](#project-structure)
- [Code Examples](#code-examples)
  - [P/Invoke](#-pinvoke)
  - [LibraryImport](#-libraryimport)
  - [NativeAOT Export](#-nativeaot-export-c--c)
  - [C calling NativeAOT](#-c-calling-nativeaot)
- [Troubleshooting](#troubleshooting)
- [License  Contributions](#license--contributions)
- [Summary](#summary)

---

## Quickstart (Windows x64)

### 1. Clone the repository
```bash
git clone https://github.com/michelenatale/Net-Language-Interoperability.git
cd Net-Language-Interoperability
```

### 2. Build the native C library (`crandom.dll`)
```cmd
cd LanguageInteroperability/CsToC/LibraryImport
cl /LD crandom.c /Fe:crandom.dll
```

### 3. Build the NativeAOT library (`NativeLibrary.dll`)
```cmd
cd NativeLibrary
dotnet publish -c Release -r win-x64
```

### 4. Run the .NET tests
```cmd
dotnet run --project TestLanguageInteroperability
```

### Optional: Build the C test project
```cmd
cd TestNativeLibrary
cl main.c NativeLibrary.lib /Fe:test.exe
```

---

## Architecture Overview

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
                     |  exports unmanaged funcs   |
                     +---------------------------+

                                   ^
                                   |  C → C#
                                   |
                     +---------------------------+
                     |     TestNativeLibrary     |
                     |            (C)            |
                     |  calls NativeAOT exports  |
                     +---------------------------+
```
---

## Which Technique for What?

| Technique | Direction | Advantages | Typical Use |
|----------|-----------|------------|--------------|
| **P/Invoke (`DllImport`)** | C# → C | Simple, established, widely supported | Calling existing C APIs, Win32 |
| **LibraryImport (Source Generator)** | C# → C | Faster, compile‑time validation, less overhead | High‑performance interop |
| **NativeAOT (C# → native DLL/.so)** | C# → C | C# compiled to real native code | When C/C++ must call C# |
| **NativeAOT + .lib Export** | C → C# | C compiler links against C# code | Integrating .NET into native apps |
| **C‑Wrapper → C#** | C# → C → C# | Full ABI control | Complex or cross‑platform interop |

---

## How the Components Work Together

### 🔹 P/Invoke (C# → C)
- C# declares an external function using `DllImport`.
- The .NET runtime loads the native DLL at runtime.
- Parameters are automatically marshalled.
- Signatures, calling convention, and architecture must match exactly.

### 🔹 LibraryImport (C# → C)
- Modern mechanism using a source generator.
- Marshaling is validated at compile time.
- Lower overhead than classic P/Invoke.
- Supports `Span<T>` and `stackalloc` for zero‑allocation interop.

### 🔹 NativeAOT (C# → native DLL/.so)
- C# code is compiled ahead‑of‑time into a real native library.
- Functions are exported using `[UnmanagedCallersOnly]`.
- Native programs (C/C++) can link against the generated `.lib` or `.a`.
- Ideal when native applications need to call C# logic.

### 🔹 C → C# via NativeAOT
- C code links against the NativeAOT‑generated `.lib`.
- Exported functions behave like regular C functions.
- Useful for integrating .NET logic into existing C/C++ codebases.

### 🔹 How it works in this repository
- `crandom.c` → compiled to `crandom.dll` → consumed by C# via P/Invoke/LibraryImport.
- `NativeLibrary` (C#) → compiled via NativeAOT to `NativeLibrary.dll` + `.lib`.
- `TestNativeLibrary` (C) → links against `NativeLibrary.lib` → calls C# exports.
- `TestLanguageInteroperability` (C#) → calls both native C and NativeAOT exports.

---

## Project Structure

- `/NativeLibrary`  
  NativeAOT example: C# compiled into a native DLL + `.lib`.

- `/LanguageInteroperability/CsToC`
  - `/LibraryImport` — C example (`crandom.c`) + C# LibraryImport usage  
  - `/NativeAOT` — building an additional native DLL (`add_aot.dll`) using `NativeLibrary.lib`

- `/TestLanguageInteroperability`  
  C# console project testing all interop variants.

- `/TestNativeLibrary`  
  Native C/C++ project calling C# NativeAOT exports.

- `Proceed-*.txt` — step‑by‑step instructions  
- `WhatCanBeDeleted.txt` — notes on generated artifacts

---

## Code Examples

### ✔ P/Invoke
```csharp
[DllImport("crandom.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern void fill_random(byte[] buffer, int length);

var data = new byte[16];
fill_random(data, data.Length);
```

### ✔ LibraryImport
```csharp
[LibraryImport("crandom.dll")]
private static partial void fill_random_lib_import(Span<byte> buffer, int length);

Span<byte> data = stackalloc byte[16];
fill_random_lib_import(data, data.Length);
```

### ✔ NativeAOT Export (C# → C)
```csharp
[UnmanagedCallersOnly(EntryPoint = "aot_add")]
public static int Add(int a, int b) => a + b;
```

### ✔ C calling NativeAOT
```c
#include <stdio.h>

__declspec(dllimport) int aot_add(int a, int b);

int main() {
    printf("Result: %d\n", aot_add(3, 4));
    return 0;
}
```

---

## Troubleshooting

- **DllNotFoundException** → DLL not in output folder  
- **BadImageFormatException** → x86/x64 mismatch  
- Use `dumpbin /headers` to check architecture  
- Use `Path.Combine` and MSBuild macros for paths  

---

## License & Contributions

See `LICENSE`.  
Contributions welcome.

---

## Summary

This repository provides a compact, practical set of examples demonstrating the three major approaches to .NET ↔ native interoperability: 
- P/Invoke
- LibraryImport
- NativeAOT - including both C# → C and C → C#

The structure is designed to make the mechanisms easy to understand and reuse in your own projects.
