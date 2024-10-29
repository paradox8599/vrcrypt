using System.Collections.Generic;
using UnityEngine;

internal partial class FFI
{
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

  [System.Serializable]
  internal class CreateRandomMeshesOutput
  {
    public List<XMesh> meshes;

    CreateRandomMeshesOutput(List<XMesh> meshes)
    {
      this.meshes = meshes;
    }
  }

  internal static CreateRandomMeshesOutput CreateRandomMeshes(CreateRandomMeshesInput input)
  {
    string json = JsonUtility.ToJson(input, false);
    StrFnOut fn = ReadString(RustNative.vrcrypt_lib.ffi_create_random_meshes);
    return JsonUtility.FromJson<CreateRandomMeshesOutput>(fn(json));
  }
}
