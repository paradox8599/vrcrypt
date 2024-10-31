using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class XGameObject
{
  public SkinnedMeshRenderer? smr => obj.GetComponent<SkinnedMeshRenderer>();
  public MeshFilter? mf => obj.GetComponent<MeshFilter>();
  public XMesh? xMesh
  {
    get
    {
      var mesh =
          smr != null ? new XMesh(smr.sharedMesh)
          : mf != null ? new XMesh(mf.sharedMesh)
          : null;
      if (mesh == null)
        return null;
      mesh.path = path;
      return mesh;
    }
  }

  [DebuggerHidden]
  public List<XGameObject> GetAllChildrenWithMeshes() =>
      GetAllChildren().FindAll(x => x.xMesh != null);

  [DebuggerHidden]
  public List<XMesh> GetAllMeshes() => GetAllChildrenWithMeshes().ConvertAll(x => x.xMesh!)!;

  [DebuggerHidden]
  public Mesh? ApplyMesh(Mesh mesh) =>
      smr != null ? (smr.sharedMesh = mesh)
      : mf != null ? (mf.sharedMesh = mesh)
      : null;

  [DebuggerHidden]
  public void SaveRandomized(string targetPrefabDir)
  {
    targetPrefabDir = Path.Combine(targetPrefabDir, "vrcrypted");
    // Generate random meshes
    var allmeshes = GetAllMeshes();
    var xMeshes = new List<XMesh>();
    foreach (var x in allmeshes)
    {
      var m = new List<XMesh>();
      m.Add(x);
      var output = FFI.Meshes.Randomize(m, 0.01f);
      xMeshes.Add(output.meshes[0]);
    }

    var meshDir = Path.Combine(targetPrefabDir, "meshes");

    // replace meshes
    foreach (var g in GetAllChildrenWithMeshes())
    {
      var xMesh = xMeshes.Find(x => x.path == g.path);
      if (xMesh == null)
      {
        Debug.LogError($"Mesh not found for: {g.path}");
      }
      xMesh!.SaveAsset(meshDir);

      var meshAsset = XMesh.LoadAsset(Path.Combine(meshDir, $"{xMesh.hash}.asset"));
      if (meshAsset == null)
      {
        Debug.LogError($"Saved mesh assest not found: {g.path}");
      }
      g.ApplyMesh(meshAsset!);
    }

    SavePrefab(targetPrefabDir);
  }

  [DebuggerHidden]
  public void EncryptMeshesAndSave(string targetPrefabDir, string key, float factor = 0.1f)
  {
    targetPrefabDir = Path.Combine(targetPrefabDir, "vrcrypt_encrypted");
    // Generate random meshes
    var allmeshes = GetAllMeshes();
    var xMeshes = new List<XMesh>();
    foreach (var x in allmeshes)
    {
      var m = new List<XMesh>();
      m.Add(x);
      var output = FFI.Meshes.Encrypt(m, key, factor);
      xMeshes.Add(output.meshes[0]);
    }

    var meshDir = Path.Combine(targetPrefabDir, "meshes");

    // replace meshes
    foreach (var g in GetAllChildrenWithMeshes())
    {
      var xMesh = xMeshes.Find(x => x.path == g.path);
      if (xMesh == null)
      {
        Debug.LogError($"Mesh not found for: {g.path}");
      }
      xMesh!.SaveAsset(meshDir);

      var meshAsset = XMesh.LoadAsset(Path.Combine(meshDir, $"{xMesh.hash}.asset"));
      if (meshAsset == null)
      {
        Debug.LogError($"Saved mesh assest not found: {g.path}");
      }
      g.ApplyMesh(meshAsset!);
    }

    SavePrefab(targetPrefabDir);
  }

  [DebuggerHidden]
  public void decryptMeshes(string key, float factor = 0.1f)
  {
    // Generate random meshes
    var allmeshes = GetAllMeshes();
    var xMeshes = new List<XMesh>();
    foreach (var x in allmeshes)
    {
      var m = new List<XMesh>();
      m.Add(x);
      var output = FFI.Meshes.Decrypt(m, key, factor);
      xMeshes.Add(output.meshes[0]);
    }

    // replace meshes
    foreach (var g in GetAllChildrenWithMeshes())
    {
      var xMesh = xMeshes.Find(x => x.path == g.path);
      if (xMesh == null)
      {
        Debug.LogError($"Mesh not found for: {g.path}");
      }
      g.ApplyMesh(xMesh!.ToMesh());
    }
  }
}
