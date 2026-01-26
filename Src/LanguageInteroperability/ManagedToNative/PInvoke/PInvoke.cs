
using System.Runtime.InteropServices;

namespace LanguageInteroperability;

partial class ManagedToNative
{
    private const string CAddPath = @"NativeLibraryLib.dll";

    [DllImport(CAddPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern int addition(int a, int b);

    public static void StartPInvoke()
    {
        TestAddition();
    }

    private static void TestAddition()
    {
        Console.WriteLine("P/Invoke");

        var rand = Random.Shared;
        var a = rand.Next(10, 100);
        var b = rand.Next(10, 100);

        Console.WriteLine($"{a} + {b} = {addition(a, b)}");
        Console.WriteLine();
    }
}
