using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class FFI
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

    // ffi function wrappers

    public static string read(string input)
    {
        var fn = FFI.ReadString(RustNative.vrcrypt_lib.unsafe_read);
        return fn(input);
    }

    /// Create Random Meshes

    [System.Serializable]
    public class CreateRandomMeshesInput
    {
        public List<XMesh> meshes;
        public float factor;

        public CreateRandomMeshesInput(List<XMesh> meshes, float factor)
        {
            this.meshes = meshes;
            this.factor = factor;
        }
    }

    public static XMeshes CreateRandomMeshes(CreateRandomMeshesInput input)
    {
        var json = JsonUtility.ToJson(input, true);
        Debug.Log("Json: " + json);
        var fn = FFI.ReadString(RustNative.vrcrypt_lib.unsafe_create_random_meshes);
        var output = fn(json);
        Debug.Log("output: " + output);
        return XMeshes.FromJson(output);
    }

    internal class SubMeshDescriptor { }
}
