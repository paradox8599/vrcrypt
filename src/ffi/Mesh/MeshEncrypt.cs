using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

internal partial class FFI
{
    internal partial class Meshes
    {
        [System.Serializable]
        internal class EncryptInput
        {
            public List<XMesh> meshes;
            public string key;
            public float factor;

            [DebuggerHidden]
            public EncryptInput(List<XMesh> meshes, string key, float factor)
            {
                this.meshes = meshes;
                this.key = key;
                this.factor = factor;
            }
        }

        [System.Serializable]
        internal class EncryptOutput
        {
            public List<XMesh> meshes;

            [DebuggerHidden]
            EncryptOutput(List<XMesh> meshes)
            {
                this.meshes = meshes;
            }
        }

        [DebuggerHidden]
        internal static EncryptOutput Encrypt(List<XMesh> meshes, string key, float factor)
        {
            var input = new EncryptInput(meshes, key, factor);
            string json = JsonUtility.ToJson(input, false);
            StrFnOut fn = ReadString(RustNative.vrcrypt_lib.ffi_encrypt_v1);
            return JsonUtility.FromJson<EncryptOutput>(fn(json));
        }
    }
}
