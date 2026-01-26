# Net-Language-Interoperability

**[German-Version](https://github.com/michelenatale/Net-Language-Interoperability/blob/main/Docs/GithubReadMe-DE.md)**

## Goal of the project


According to Wikipedia, [LanguageInteroperability](https://en.wikipedia.org/wiki/Language_interoperability) is the ability of two different programming languages to interact natively as part of the same system and to work on the same types of data structures.

This project shows simple examples in .Net for [interoperability](https://learn.microsoft.com/de-de/dotnet/standard/native-interop/) between C, C++, and C#, including:
- P/Invoke
- LibraryImport
- NativeAOT

The goal is also to demonstrate how managed C# code can be compiled as a native library and then called from native programs (focus on NativeAOT). 

The project is deliberately kept minimal and serves as a technical reference for developers who want to link C and C# code via NativeAOT without using Visual Studio.

---

## Table of Contents

- [Goal of the project](#goal-of-the-project)
- [Project structure (folder overview)](#project-structure-folder-overview)
- [Quickstart (Windows & Linux)](#quickstart-windows--linux)
- [Architecture Overview (C ↔ C# NativeAOT)](#architecture-overview-c--c-nativeaot)
- [Which Technique for What](#which-technique-for-what)
- [Code Examples](#code-examples)
  - [P/Invoke](#-pinvoke)
  - [LibraryImport](#-libraryimport)
  - [NativeAOT Export](#-nativeaot-export-c--c)
  - [C calling NativeAOT](#-c-calling-nativeaot)
- [Troubleshooting](#troubleshooting)
- [License  Contributions](#license--contributions)
- [Summary](#summary)
---

## Project structure (folder overview)

```text
Net-Language-Interoperability/
│
├── Docs/
├── Src/
│   ├── NativeLibrary/
│   ├── TestNativeLibrary/
│   ├── NativeLibraryLib/
│   └── TestLanguageInteroperability/
│   └── LanguageInteroperability/
│
└── Build/
    └── Bin/
    └── Native/
``` 
---

## Quickstart (Windows & Linux)

### 1. Clone repository
```bash
git clone https://github.com/michelenatale/Net-Language-Interoperability.git
cd Net-Language-Interoperability
```

### 2. Publishing NativeAOT
Publishing the NativeLibrary project via NativeAOT Build (Windows)
```cmd
dotnet publish Src/NativeLibrary -c Release -r win-x64
```
Publishing NativeLibrary via NativeAOT Build (Linux)
```cmd
dotnet publish Src/NativeLibrary -c Release -r linux-x64
```

### 3. Builds .Net Assyemblies (Windows/Linux)
```cmd
dotnet build -c Release
```
For the TestLanguageInteroperability, LanguageInteroperability, and NativeLibrary projects.

### 4. Builds Native

C/C++ build (Windows, MSVC)
```cmd
cl Src\TestNativeLibrary\main.c
```

C/C++ build (Linux, GCC)
```cmd
gcc Src\TestNativeLibrary/main.c -o TestNativeLibrary
```
The same applies to the NativeLibraryLib project.


### 5. Build für das ganze Project 
```cmd
MSBuild.exe BuildAll.proj
```

---

## Architecture Overview (C ↔ C# NativeAOT)

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

## Which Technique for What?

| Technique | Direction | Advantages | Typical Use |
|----------|-----------|------------|--------------|
| **P/Invoke (`DllImport`)** | C# → C | Simple, established, widely supported | Calling existing C APIs, Win32 |
| **LibraryImport (Source Generator)** | C# → C | Faster, compile‑time validation, less overhead | High‑performance interop |
| **NativeAOT (C# → native DLL/.so)** | C# → C | C# compiled to real native code | When C/C++ must call C# |
| **NativeAOT + .lib Export** | C → C# | C compiler links against C# code | Integrating .NET into native apps |
| **C‑Wrapper → C#** | C# → C → C# | Full ABI control | Complex or cross‑platform interop |

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
- NativeAOT

The structure is designed to make the mechanisms easy to understand and reuse in your own projects.
