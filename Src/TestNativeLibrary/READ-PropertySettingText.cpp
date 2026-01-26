#include "pch.h"


/*
  Deutsch 

  Damit der Linker weiß, wo die .lib liegt:
  Projekt → Eigenschaften → Linker → General → Additional Library Directories
  >> $(SolutionDir)Build\Native

  Dem Linker sagen, welche Library er linken soll:
  Projekt → Eigenschaften → Linker → Input → Additional Dependencies
  >> NativeLibrary.lib

  Path setzen, damit c/c++-exe die dll während der Laufzeit findet:
  Projekt → Eigenschaften → Debugging → Environment
  >> PATH=$(SolutionDir)Build\Native;%PATH%

  Optional: Header‑Dateien einbinden:
  C/C++ → General → Additional Include Directories
  >> $(SolutionDir)Build\Native

  ********** ********** ********** ********** ********** ********** **********
  ********** ********** ********** ********** ********** ********** ********** 

  English

  So that the linker knows where the .lib is located:
  Project → Properties → Linker → General → Additional Library Directories
  >> $(SolutionDir)Build\Native

  Tell the linker which library to link:
  Project → Properties → Linker → Input → Additional Dependencies
  >> NativeLibrary.lib

  Set the path so that c/c++-exe can find the dll during runtime:
  Project → Properties → Debugging → Environment
  >> PATH=$(SolutionDir)Build\Native;%PATH%

  Optional: Include header files:
  C/C++ → General → Additional Include Directories
  >> $(SolutionDir)Build\Native

*/