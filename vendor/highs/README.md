# Embedded integer solver

HiGHS 1.15.1, MIT license. Upstream: https://github.com/ERGO-Code/HiGHS/releases/tag/v1.15.1

`highs.dll` is built from the pinned source archive with MSVC x64 and `/MT`. Its only imported DLL is Windows `KERNEL32.dll`; it does not require Python, BLAS, a separately installed Visual C++ runtime, or another .NET runtime. `provenance.json` records source and binary SHA-256, compiler and CMake flags. `LICENSE.txt` is embedded in the product, and exposed through the records directory.

Rebuild from the repository working directory:

```powershell
# Source archive SHA-256 and configure options are in provenance.json.
# Extract the archive under artifacts/build/highs-source/.
cmake -S artifacts/build/highs-source/HiGHS-1.15.1 -B artifacts/build/highs-native-mt -G "Visual Studio 17 2022" -A x64 -DBUILD_SHARED_LIBS=ON -DBUILD_SHARED_EXTRAS_LIB=OFF -DBUILD_CXX_EXE=OFF -DBUILD_TESTING=OFF -DBUILD_EXAMPLES=OFF -DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreaded -DZLIB=OFF -DHIGHSINT64=OFF
cmake --build artifacts/build/highs-native-mt --config Release --target highs --parallel 16
```

Copy the resulting `Release/bin/highs.dll` here only after checking its PE imports and rerunning the native parity suite. The desktop embeds the DLL; no separate file is distributed beside the EXE.

The core calls the documented C API: https://ergo-code.github.io/HiGHS/dev/interfaces/c_api/ . Integer solutions are accepted only after optimal status and a separate resource-constraint check; rounding is never allowed to exceed a material budget.
