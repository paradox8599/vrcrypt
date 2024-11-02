using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public partial class XGameObject
{
  public SkinnedMeshRenderer? smr => obj.GetComponent<SkinnedMeshRenderer>();
  public MeshFilter? mf => obj.GetComponent<MeshFilter>();
  public Mesh? mesh =>
      smr != null ? smr.sharedMesh
      : mf != null ? mf.sharedMesh
      : null;
  public XMesh? xMesh => mesh == null ? null : new XMesh(mesh, path);

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
}
