using LanguageInteroperability;

namespace TestLanguageInteroperability;

public class Program
{
    public static void Main()
    {
        TestManagedToNative();
        TestNativeToManaged();


        Console.WriteLine();
        Console.WriteLine("Finish C# Console\n\n");
        Console.ReadLine();
    }

    #region Managed → Native

    private static void TestManagedToNative()
    {
        Console.WriteLine("MANAGED CALL NATIVE");
        Console.WriteLine("*******************\n");

        ManagedToNative.StartPInvoke();
        ManagedToNative.StartLibraryImport();
        ManagedToNative.StartNativeAOT();
    }

    #endregion
    #region Native → Managed
    private static void TestNativeToManaged()
    {
        Console.WriteLine("NATIVE CALL MANAGED");
        Console.WriteLine("*******************\n");
        NativeToManaged.StartNativeAOT();
    }
    #endregion

}