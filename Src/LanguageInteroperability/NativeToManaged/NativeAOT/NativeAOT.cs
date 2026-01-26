

using System.Diagnostics;

namespace LanguageInteroperability;

public partial class NativeToManaged
{

    public static void StartNativeAOT()
    {
        var exefile = @"..\..\..\..\..\TestNativeLibrary\\x64\\Debug\NativeLibrary.exe";
        using var p = Process.Start(exefile);
        p.WaitForExit();
    }
}
