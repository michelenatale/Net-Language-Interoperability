\# Net-Language-Interoperability



\*\*\[English-Version](https://github.com/michelenatale/Net-Language-Interoperability/blob/main/README.md)\*\*



\## Ziel des Projekts



Gemäss Wikipedia ist \[LanguageInteroperability](https://en.wikipedia.org/wiki/Language\_interoperability) die Fähigkeit zweier verschiedener Programmiersprachen, nativ als Teil desselben Systems zu interagieren und auf denselben Arten von Datenstrukturen zu arbeiten.



Dieses Projekt zeigt einfache Beispiele in .Net zur \[Interoperabilität](https://learn.microsoft.com/de-de/dotnet/standard/native-interop/) zwischen C, C++ und C# unter andem mit:

\- P/Invoke

\- LibraryImport

\- NativeAOT



Ziel ist es auch, zu demonstrieren, wie verwalteter C#‑Code als native Bibliothek kompiliert und anschließend aus nativen Programmen aufgerufen werden kann (Fokus auf NativeAOT). 



Das Projekt ist bewusst minimal gehalten und dient als technische Referenz für Entwickler, die C‑ und C#‑Code über NativeAOT verbinden möchten, ohne Visual Studio zu verwenden.



---



\## Inhaltsverzeichnis



\- \[Ziel des Projekts](#ziel-des-projekts)

\- \[Projektstruktur](#projektstruktur-ordnerübersicht)

\- \[Quickstart (Windows \& Linux)](quickstart)

\- \[Architekturüberblick (C ↔ C# NativeAOT)](architekturüberblick)

\- \[Welche Technik wofür](#welche-technik-wofür)

\- \[Code-Beispiele](#codebeispiele)

&nbsp; - \[P/Invoke](#-pinvoke)

&nbsp; - \[LibraryImport](#-libraryimport)

&nbsp; - \[NativeAOT Export](#-nativeaot-export)

&nbsp; - \[C ruft NativeAOT auf](#-c-ruft-nativeaot-auf)

\- \[Troubleshooting](#troubleshooting)

\- \[Lizenz  Beiträge](#lizenz--beiträge)

\- \[Zusammenfassung](#zusammenfassung)

---



\## Projektstruktur (Ordnerübersicht)



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

&nbsp;   └── Bin/

&nbsp;   └── Native/

``` 

---



\## Quickstart (Windows \& Linux)



\### 1. Repository klonen

```bash

git clone https://github.com/michelenatale/Net-Language-Interoperability.git

cd Net-Language-Interoperability

```



\### 2. Veröffentlichung NativeAOT

Veröffentlichen des Projektes NativeLibrary über NativeAOT Build (Windows)

```cmd

dotnet publish Src/NativeLibrary -c Release -r win-x64

```

Veröffentlichen von NativeLibrary über NativeAOT Build (Linux)

```cmd

dotnet publish Src/NativeLibrary -c Release -r linux-x64

```



\### 3. Builds .Net Assyemblies (Windows/Linux)

```cmd

dotnet build -c Release

```

Für die Projekten TestLanguageInteroperability, LanguageInteroperability und NativeLibrary.



\### 4. Builds Native



C/C++ build (Windows, MSVC)

```cmd

cl Src\\TestNativeLibrary\\main.c

```



C/C++ build (Linux, GCC)

```cmd

gcc Src\\TestNativeLibrary/main.c -o TestNativeLibrary

```

Das gleiche auch mit dem Projekt NativeLibraryLib.





\### 5. Build für das ganze Project 

```cmd

dotnet build NetLanguageInteroperability.sln

```



---



\## Architekturüberblick (C ↔ C# NativeAOT)



```text

&nbsp;                  +------------------------------+

&nbsp;                  | TestLanguageInteroperability |

&nbsp;                  |            (C#)              |

&nbsp;                  |------------------------------|

&nbsp;                  | - P/Invoke                   |

&nbsp;                  | - LibraryImport              |

&nbsp;                  | - NativeAOT (C# → C)         |

&nbsp;                  +---------------+--------------+

&nbsp;                                  |

&nbsp;                                  |  C# → C

&nbsp;                                  v

&nbsp;                    +---------------------------+

&nbsp;                    |   Native C Libraries      |

&nbsp;                    |  (crandom.dll / .so)      |

&nbsp;                    +---------------------------+



&nbsp;                                  ^

&nbsp;                                  |  C → C#

&nbsp;                                  |

&nbsp;                    +---------------------------+

&nbsp;                    |     NativeLibrary (C#)    |

&nbsp;                    |     NativeAOT DLL/.lib    |

&nbsp;                    |  exportiert Funktionen     |

&nbsp;                    +---------------------------+



&nbsp;                                  ^

&nbsp;                                  |  C → C#

&nbsp;                                  |

&nbsp;                    +---------------------------+

&nbsp;                    |     TestNativeLibrary     |

&nbsp;                    |            (C)            |

&nbsp;                    |  ruft NativeAOT‑Exports   |

&nbsp;                    +---------------------------+

```

---



\## Welche Technik wofür?



| Technik | Richtung | Vorteile | Typische Nutzung |

|--------|----------|----------|------------------|

| \*\*P/Invoke (`DllImport`)\*\* | C# → C | Einfach, etabliert | Zugriff auf C‑APIs, Win32 |

| \*\*LibraryImport\*\* | C# → C | Schnell, compile‑time‑validiert | Performance‑kritische Interop |

| \*\*NativeAOT (C# → native DLL/.so)\*\* | C# → C | C# wird zu echter nativer DLL | Wenn C‑Programme C#‑Code aufrufen sollen |

| \*\*NativeAOT + .lib Export\*\* | C → C# | C‑Compiler linkt gegen C# | Integration in C/C++‑Code |

| \*\*C‑Wrapper → C#\*\* | C# → C → C# | Volle ABI‑Kontrolle | Komplexe Interop‑Szenarien |



---







\## Code‑Beispiele



\### ✔ P/Invoke

```csharp

\[DllImport("crandom.dll", CallingConvention = CallingConvention.Cdecl)]

private static extern void fill\_random(byte\[] buffer, int length);

```



\### ✔ LibraryImport

```csharp

\[LibraryImport("crandom.dll")]

private static partial void fill\_random\_lib\_import(Span<byte> buffer, int length);

```



\### ✔ NativeAOT Export

```csharp

\[UnmanagedCallersOnly(EntryPoint = "aot\_add")]

public static int Add(int a, int b) => a + b;

```



\### ✔ C ruft NativeAOT auf

```c

\_\_declspec(dllimport) int aot\_add(int a, int b);

```



---



\## Troubleshooting



\- \*\*DllNotFoundException\*\* → DLL fehlt  

\- \*\*BadImageFormatException\*\* → Architektur falsch  

\- `dumpbin /headers` hilft  

\- Pfade über MSBuild‑Makros setzen  



---



\## Lizenz \& Beiträge



Siehe `LICENSE`.  

Beiträge willkommen.



---



\## Zusammenfassung



Dieses Repository ist ein bewusst kompaktes, praxisorientiertes Set von Beispielen, das die drei wichtigsten Wege der .NET ↔ native Interoperabilität demonstriert: 

\- P/Invoke

\- LibraryImport

\- NativeAOT - Inklusive C# → C und C → C#.



Die Struktur ist so gewählt, dass man die Mechanismen klar erkennen und in eigene Projekte übertragen kann.

