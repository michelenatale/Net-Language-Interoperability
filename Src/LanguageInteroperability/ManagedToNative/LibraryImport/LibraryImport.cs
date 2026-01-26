
using System.Runtime.InteropServices;

namespace LanguageInteroperability;

partial class ManagedToNative
{
    private const string CRandFile = @"NativeLibraryLib.dll";

    [LibraryImport(CRandFile)]
    private static partial void fill_rng_bytes(Span<byte> buffer, int length);


    public static void StartLibraryImport()
    {
        TestRandom();
    }

    private static void TestRandom()
    {
        Console.WriteLine("LibraryImport");

        Span<byte> data = stackalloc byte[16];
        fill_rng_bytes(data, data.Length);
        Console.WriteLine($"rng hex:\t{Convert.ToHexString(data)}");

        Console.WriteLine($"rng bytes:\t{string.Join(" ", data.ToArray())}");
        Console.WriteLine();
    }

}
