
namespace LanguageInteroperability;

partial class ManagedToNative
{
    //see Project NativLibrary too


    public static void StartNativeAOT()
    {
        //C# -> C-Interop -> C
        Console.WriteLine("NativeAOT");
        Console.WriteLine("See Project NativLibrary");
        Console.WriteLine("It would be possible to access " +
          "from Csharp to a C-dll,\nand then back to the native " +
          "CSharp-AOT-Dll, which could\nbe easily solved via " +
          "LibraryImport and then via C-Import\n(__declspec(dllimport)).\n\n\n");

        // Es wäre möglich, von Csharp auf eine
        // C-dll zuzugreifen und dann zurück zur
        // nativen CSharp-AOT-Dll, was einfach über
        // LibraryImport und dann über C-Import
        // (__declspec(dllimport)) gelöst werden könnte.
    }
}
