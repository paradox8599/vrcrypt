using System.Collections.Generic;
using UnityEngine;

internal partial class FFI
{
    internal partial class Meshes
    {
        [System.Serializable]
        internal class DecryptInput
        {
            public List<XMesh> meshes;
            public string key;
            public float factor;

            public DecryptInput(List<XMesh> meshes, string key, float factor)
            {
                this.meshes = meshes;
                this.key = key;
                this.factor = factor;
            }
        }

        [System.Serializable]
        internal class DecryptOutput
        {
            public List<XMesh> meshes;

            DecryptOutput(List<XMesh> meshes)
            {
                this.meshes = meshes;
            }
        }

        internal static DecryptOutput Decrypt(List<XMesh> meshes, string key, float factor)
        {
            var input = new DecryptInput(meshes, key, factor);
            string json = JsonUtility.ToJson(input, false);
            StrFnOut fn = ReadString(RustNative.vrcrypt_lib.ffi_decrypt_v1);
            return JsonUtility.FromJson<DecryptOutput>(fn(json));
        }
    }
}
