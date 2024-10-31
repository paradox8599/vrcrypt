using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public partial class XMesh
{
  private string? _hash = null;
  public string hash => _hash ??= ToHash();

  [DebuggerHidden]
  public byte[] ToBytes() => Encoding.UTF8.GetBytes(JsonUtility.ToJson(this));

  [DebuggerHidden]
  public byte[] ToHashBytes()
  {
    using SHA256 sha256 = SHA256.Create();
    return sha256.ComputeHash(ToBytes());
  }

  [DebuggerHidden]
  public string ToHash()
  {
    StringBuilder builder = new StringBuilder();
    foreach (byte b in ToHashBytes())
    {
      builder.Append(b.ToString("x2"));
    }
    return builder.ToString();
  }
}
