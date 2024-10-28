using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

internal class FFI
{
    delegate IntPtr StrFnIn(ref sbyte input);
    delegate string StrFnOut(string input);

    class StringHandle : SafeHandle
    {
        public StringHandle()
            : base(IntPtr.Zero, true) { }

        public override bool IsInvalid => handle == IntPtr.Zero;

        public static StringHandle FromFunction(StrFnIn ffiFunction, string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input + "\0");
            unsafe
            {
                fixed (byte* inputPtr = inputBytes)
                {
                    IntPtr ptr = ffiFunction(ref *(sbyte*)inputPtr);
                    return new StringHandle { handle = ptr };
                }
            }
        }

        public string AsString()
        {
            int len = 0;
            while (Marshal.ReadByte(handle, len) != 0)
            {
                ++len;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(handle, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                unsafe
                {
                    sbyte* ptrAsSByte = (sbyte*)handle.ToPointer();
                    RustNative.vrcrypt_lib.unsafe_free_str(out *ptrAsSByte);
                    handle = IntPtr.Zero;
                }
            }
            return true;
        }
    }

    static StrFnOut ReadString(StrFnIn strFn) =>
        (string input) =>
        {
            using (var handle = StringHandle.FromFunction(strFn, input))
            {
                return handle.AsString();
            }
        };

    // ffi function wrappers

    /// Create Random Meshes

    [System.Serializable]
    internal class CreateRandomMeshesInput
    {
        public List<XMesh> meshes;
        public float factor;

        public CreateRandomMeshesInput(List<XMesh> meshes, float factor)
        {
            this.meshes = meshes;
            this.factor = factor;
        }
    }

    internal static XMeshes CreateRandomMeshes(CreateRandomMeshesInput input)
    {
        string json = JsonUtility.ToJson(input, false);
        StrFnOut fn = ReadString(RustNative.vrcrypt_lib.unsafe_create_random_meshes);
        return XMeshes.FromJson(fn(json));
    }
}
