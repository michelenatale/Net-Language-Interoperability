
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;


namespace NativeLibrary;


public static class NativeExports
{
    #region Aot Add
    [UnmanagedCallersOnly(EntryPoint = "aot_add")]
    public static int Add(int a, int b) => a + b;
    #endregion Aot Add


    #region Aot Crypto Random

    private static readonly RandomNumberGenerator Rand =
      RandomNumberGenerator.Create();

    [UnmanagedCallersOnly(EntryPoint = "rng_crypto_int_32")]
    public static int RngCryptoInt32()
    {
        var bytes = new byte[4];
        Rand.GetNonZeroBytes(bytes);
        return Math.Abs(BitConverter.ToInt32(bytes));
    }


    [UnmanagedCallersOnly(EntryPoint = "rng_crypto_int_64")]
    public static long RngCryptoInt64()
    {
        var bytes = new byte[8];
        Rand.GetNonZeroBytes(bytes);
        return Math.Abs(BitConverter.ToInt64(bytes));
    }

    #endregion Aot Crypto Random
}
