using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

internal partial class FFI
{
    internal partial class Meshes
    {
        [System.Serializable]
        internal class RandomizeInput
        {
            public List<XMesh> meshes;
            public float factor;

            [DebuggerHidden]
            public RandomizeInput(List<XMesh> meshes, float factor)
            {
                this.meshes = meshes;
                this.factor = factor;
            }
        }

        [System.Serializable]
        internal class RandomizeOutput
        {
            public List<XMesh> meshes;

            [DebuggerHidden]
            RandomizeOutput(List<XMesh> meshes)
            {
                this.meshes = meshes;
            }
        }

        [DebuggerHidden]
        internal static RandomizeOutput Randomize(List<XMesh> meshes, float factor)
        {
            var input = new RandomizeInput(meshes, factor);
            string json = JsonUtility.ToJson(input, false);
            StrFnOut fn = ReadString(RustNative.vrcrypt_lib.ffi_randomize);
            return JsonUtility.FromJson<RandomizeOutput>(fn(json));
        }
    }
}
