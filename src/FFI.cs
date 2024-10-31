using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

internal partial class FFI
{
    delegate IntPtr StrFnIn(ref sbyte input);
    delegate string StrFnOut(string input);

    class StringHandle : SafeHandle
    {
        public StringHandle()
            : base(IntPtr.Zero, true) { }

        public override bool IsInvalid => handle == IntPtr.Zero;

        [DebuggerHidden]
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

        [DebuggerHidden]
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

        [DebuggerHidden]
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                unsafe
                {
                    sbyte* ptrAsSByte = (sbyte*)handle.ToPointer();
                    RustNative.vrcrypt_lib.free_str(out *ptrAsSByte);
                    handle = IntPtr.Zero;
                }
            }
            return true;
        }
    }

    [DebuggerHidden]
    static StrFnOut ReadString(StrFnIn strFn) =>
        (string input) =>
        {
            using (var handle = StringHandle.FromFunction(strFn, input))
            {
                var output = handle.AsString();
                if (output.StartsWith("v"))
                {
                    return output.Substring(1);
                }
                else
                {
                    throw new Exception(output.Substring(1));
                }
            }
        };
}
