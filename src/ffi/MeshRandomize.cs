using System.Collections.Generic;
using UnityEngine;

internal partial class FFI
{
    [System.Serializable]
    internal class MeshRandomizeInput
    {
        public List<XMesh> meshes;
        public float factor;

        public MeshRandomizeInput(List<XMesh> meshes, float factor)
        {
            this.meshes = meshes;
            this.factor = factor;
        }
    }

    [System.Serializable]
    internal class MeshRandomizeOutput
    {
        public List<XMesh> meshes;

        MeshRandomizeOutput(List<XMesh> meshes)
        {
            this.meshes = meshes;
        }
    }

    internal static MeshRandomizeOutput CreateRandomMeshes(MeshRandomizeInput input)
    {
        string json = JsonUtility.ToJson(input, false);
        StrFnOut fn = ReadString(RustNative.vrcrypt_lib.ffi_randomize);
        return JsonUtility.FromJson<MeshRandomizeOutput>(fn(json));
    }
}
