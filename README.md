# Net-Language-Interoperability
LanguageInteroperability for DotNet

A compact collection of small, focused examples and step‑by‑step guides that demonstrate how .NET (C#) interoperates with native libraries (C/C++). The repository contains multiple approaches: P/Invoke, LibraryImport, and NativeAOT, plus test harnesses and build instructions in English and German.

Short: Practical reference for .NET ↔ native C interoperability on Windows x64 using modern and classic techniques.

Badges (optional)
- Build / CI: (add when you enable GitHub Actions)
- License: MIT (or your chosen license)

Table of contents
- Overview
- Repository structure (files & projects)
- Architecture diagrams (ASCII + Mermaid)
- Projects: purpose, internals, relationships
  - NativeLibrary
  - LanguageInteroperability/CsToC
    - LibraryImport
    - NativeAOT
  - TestLanguageInteroperability
  - TestNativeLibrary
- How the parts interact (technical)
- Quickstart (Windows x64)
- Build automation snippet (csproj)
- Troubleshooting & tips
- Suggested improvements & priorities
- Contributing & license note
- Summary / Conclusion

---

Overview
========
This repository demonstrates three common techniques to call native code from .NET and to expose .NET code as native exports:

- P/Invoke (classic `DllImport`) — runtime binding to native DLLs.
- LibraryImport (modern attribute/source-gen style) — lower-overhead imports and better interop ergonomics.
- NativeAOT — compiling a .NET library into a native binary that can be consumed by native code or used like a native DLL.

The samples are intentionally small and practical: they show minimal, focused examples for learning and experimentation rather than a full product.

Repository structure (high-level)
================================
- NativeLibrary/
  - .NET project demonstrating NativeAOT exports (Library.cs, NativeLibrary.csproj)
  - Proceed-NativeLibrary-en.txt / Proceed-NativeLibrary-de.txt — step-by-step build instructions
- LanguageInteroperability/CsToC/
  - LibraryImport/
    - crandom.c �� example native C implementation (random bytes)
    - LibraryImport.cs — example C# using [LibraryImport]
    - Proceed-LibraryImport-en.txt / -de.txt
  - NativeAOT/
    - instructions and helper files to create add_aot.dll that links to NativeLibrary.lib
    - Proceed-NativeAOT-en.txt / -de.txt
- TestLanguageInteroperability/
  - Test harness (Program.cs) that runs P/Invoke, LibraryImport and NativeAOT examples
- TestNativeLibrary/
  - Native-side test harness and build helpers (pch.h etc.)
- Proceed-*.txt (EN / DE) — many how-to documents scattered across folders
- WhatCanBeDeleted.txt — list of build artifacts that can be removed
- LICENSE — currently present; you plan to change it to MIT

Architecture diagrams
=====================

ASCII overview (simple)
-----------------------
LanguageInteroperability (C#)                       NativeLibrary (C# NativeAOT)
+-----------------------------+                    +----------------------+
| TestLanguageInteroperability |  <---uses---       | NativeLibrary (AOT)  |
| - CsToC.StartPInvoke()       |  <---P/Invoke-->   | - exports aot_add    |
| - CsToC.StartLibraryImport() |  <---LibraryImport->| - NativeLibrary.lib  |
| - CsToC.StartNativeAOT()     |  <---native dll--->| - aot_add.c          |
+-----------------------------+                    +----------------------+
            ^
            |
       builds / runs
       (Windows x64)

Mermaid (rendered on platforms that support it)
```mermaid
flowchart LR
  A[TestLanguageInteroperability C#] -->|P/Invoke| B[crandom.dll (native)]
  A -->|LibraryImport| C[crandom.dll]
  A -->|NativeAOT (published)| D[NativeLibrary native DLL]
  D -->|provides| E[aot_add entry point]
```

Projects — purpose, internals and relations
==========================================

NativeLibrary
------------
Purpose
- Demonstrates how to export .NET methods as native entry points using NativeAOT.

What it contains
- Library.cs with methods marked by `[UnmanagedCallersOnly(EntryPoint="...")]` for direct exports (example: `aot_add`).
- NativeLibrary.csproj with `<PublishAot>true</PublishAot>` and settings for AOT publishing.

Why it exists
- Produces native DLLs and `.lib` artifacts that can be linked into other native components. Useful when you want to expose .NET logic to native code with low runtime overhead.

Key technical notes
- TargetFramework: `net10.0`.
- Use `dotnet publish -c Release -r win-x64` to produce native artifacts.
- Project sets `<DisableRuntimeMarshalling>true</DisableRuntimeMarshalling>` to control marshaling behaviour; be explicit about parameter types.

LanguageInteroperability/CsToC
------------------------------
Purpose
- Collection of C# → C examples showing multiple interop styles.

LibraryImport
- Modern example using `[LibraryImport]` (source-gen style), with efficient usage of `Span<byte>` and `stackalloc` to avoid heap allocations.
- Native part: `crandom.c` — small native routine to fill a buffer with random bytes; compiled to `crandom.dll`.
- C# wrapper demonstrates `partial` extern declarations and calling patterns.

NativeAOT (in CsToC)
- Shows how to produce `add_aot.dll` by linking against `NativeLibrary.lib` (produced by native AOT publish).
- Demonstrates building a native wrapper (`add_aot.c`) that calls into AOT-produced exports.

TestLanguageInteroperability
----------------------------
Purpose
- A console project that sequentially invokes the three approaches:
  - `TestCsToCPInvoke()` → classic P/Invoke example
  - `TestCsToCLibraryImport()` → LibraryImport example
  - `TestCsToCNativeAOT()` → NativeAOT example

Why it exists
- Acts as a smoke test and usage example, showing expected build/copy steps for native assets and verifying runtime behavior.

TestNativeLibrary
-----------------
Purpose
- Native/C++ test harness and helper headers for building and testing native pieces.

How the parts interact (technical)
=================================
- P/Invoke:
  - C# declares extern methods with `[DllImport("crandom.dll", ...)]`.
  - At runtime the CLR loader locates and loads `crandom.dll`.
  - Careful alignment of calling conventions and types is required.

- LibraryImport:
  - Uses the modern `LibraryImport` attribute (source-generated stubs) for lower marshaling overhead.
  - Supports Span-based patterns: allocate stack buffers in C#, pass pointers/lengths into native code — high performance when used carefully.

- NativeAOT:
  - .NET code is AOT‑compiled to a native DLL. Exported managed methods can be called from native code via the generated entry points with `[UnmanagedCallersOnly]`.
  - Native binaries such as `NativeLibrary.lib` can be used to link native wrappers (`cl /LD add_aot.c NativeLibrary.lib /Fe:add_aot.dll`).

- Test harnesses:
  - Test projects expect native artifacts to be present in their runtime output folder. Either copy them manually or use PostBuild events / csproj targets to place native DLLs next to the managed outputs.

Quickstart — Build & Run (Windows x64)
======================================
Prerequisites
- .NET 10 SDK
- Visual Studio build tools (cl, dumpbin) or full Visual Studio with x64 Native Tools
- Windows x64 machine

Manual quick build
1. Clone the repository:
   git clone https://github.com/michelenatale/Net-Language-Interoperability.git

2. Build the native sample (crandom.dll):
   - Open "x64 Native Tools Command Prompt"
   - cd LanguageInteroperability\CsToC\LibraryImport
   - cl /LD crandom.c /Fe:crandom.dll
   - For a release-like optimized build: cl /LD /O2 /GL /Gy /DNDEBUG crandom.c /Fe:crandom.dll

3. Build NativeLibrary (NativeAOT):
   - cd NativeLibrary
   - dotnet publish -c Release -r win-x64
   - Look for artifacts in bin\x64\Release\net10.0\win-x64\publish (native DLL, .lib, possible C wrapper sources)

4. Build and run the test harness:
   - dotnet build TestLanguageInteroperability
   - dotnet run --project TestLanguageInteroperability

Copying native artifacts
- If the test project cannot find native DLLs, copy `crandom.dll`, `add_aot.dll`, `NativeLibrary.dll` into the test project's output directory (e.g. `bin\<cfg>\net10.0\`).
- Alternatively automate this with the csproj snippet below.

Build automation snippet (csproj)
--------------------------------
Place this in your test project's .csproj to copy a native DLL into the managed output after build:

```xml
<Target Name="CopyNativeLibs" AfterTargets="Build">
  <ItemGroup>
    <NativeFiles Include="$(SolutionDir)LanguageInteroperability\CsToC\LibraryImport\crandom.dll" Condition="Exists('$(SolutionDir)LanguageInteroperability\CsToC\LibraryImport\crandom.dll')" />
  </ItemGroup>
  <Copy SourceFiles="@(NativeFiles)" DestinationFolder="$(OutputPath)" SkipUnchangedFiles="true" />
</Target>
```

Troubleshooting & tips
======================
- "Dll not found" → Ensure the native DLL is in the runtime output folder beside the managed binary.
- "BadImageFormatException" → Architecture mismatch (x86 vs x64). Use `dumpbin /headers <dll> | findstr machine` to verify (8664 = x64).
- Prefer Path.Combine and RuntimeInformation checks over hard-coded backslashes and assumptions.
- For debugging native code, build the native DLL with debug symbols (`/Zi`) and without aggressive optimization (`/Od` or no /O2).
- When using `Span<byte>` and `stackalloc`, document lifetimes carefully: stack memory must not escape to native code that stores the pointer for later use.
- If you change marshaling, ensure both managed and native signatures and calling conventions match.

Suggested improvements & priorities
==================================
Priority 1 — High value, low effort
- Add a comprehensive README (this file).
- Add `.gitignore` (exclude bin/obj, .vs, etc.) and remove any committed build artifacts from history (`git rm --cached`).
- Add PostBuild/Copy targets that place native DLLs where tests expect them.

Priority 2 — Medium
- Add a Windows CI workflow (GitHub Actions) that builds native components and runs the smoke test.
- Add CONTRIBUTING.md and issue/PR templates.

Priority 3 — Longer term
- Provide pre-built release assets (zipped native DLLs) to make it easier for others to try examples.
- Add platform-specific build guides (mingw/clang for Linux/macOS) if you plan cross-platform support.
- Expand tests to assert runtime behaviour (integrated unit/integration tests).

Contributing & license note
===========================
- You indicated you are the sole copyright holder and plan to switch to MIT. If you change the LICENSE file to MIT, include the correct year and your name.
- If others contribute, consider adding a CONTRIBUTING.md and a short CLA statement (simple contributor agreement or a requirement to accept license terms in PR description).

Summary / Conclusion
====================
Net-Language-Interoperability is a compact, well-focused learning repository that demonstrates three widely used interoperability strategies between .NET and native C: P/Invoke, LibraryImport, and NativeAOT. The examples are practical and Windows-oriented, with detailed build notes. To make the repo even more helpful and production-friendly, add a proper README (this file), a `.gitignore`, small build automation for copying native artifacts, and a CI workflow that builds and smoke-tests the examples.

If you’d like, I can:
- Create a branch + PR with this README.md and a .gitignore,
- Add a Windows GitHub Actions workflow that builds native artifacts and runs the test harness,
- Or provide smaller incremental patches (only README or only CI).

Tell me which option you prefer and I’ll prepare the changes.
