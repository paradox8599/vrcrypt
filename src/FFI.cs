using System;
using System.Runtime.InteropServices;
using System.Text;

class FFI
{
    private delegate IntPtr StrFnInput(ref sbyte input);
    private delegate string StrFnOutput(string input);

    private static StrFnOutput ReadString(StrFnInput strFn)
    {
        return (string input) =>
        {
            // Convert input string to UTF-8 encoded byte array
            byte[] inputBytes = Encoding.UTF8.GetBytes(input + "\0");
            IntPtr outputPtr = IntPtr.Zero;

            try
            {
                unsafe
                {
                    fixed (byte* inputPtr = inputBytes)
                    {
                        outputPtr = strFn(ref *(sbyte*)inputPtr);
                    }
                }

                return Marshal.PtrToStringAnsi(outputPtr);
            }
            finally
            {
                // Free the unmanaged memory allocated by the native function
                if (outputPtr != IntPtr.Zero)
                {
                    // Marshal.FreeHGlobal(outputPtr);
                    Marshal.FreeCoTaskMem(outputPtr);
                }
            }
        };
    }

    // ffi function callings

    public static string read(string input)
    {
        return FFI.ReadString(RustNative.vrcrypt_lib.read)(input);
    }
}
