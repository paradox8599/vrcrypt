using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class XColor
{
  public float r;
  public float g;
  public float b;
  public float a;

  [DebuggerHidden]
  public XColor(Color color)
  {
    r = color.r;
    g = color.g;
    b = color.b;
    a = color.a;
  }

  [DebuggerHidden]
  public Color ToColor()
  {
    return new Color(r, g, b, a);
  }

  [DebuggerHidden]
  public static XColor[] FromColors(Color[] colors)
  {
    var xColors = new XColor[colors.Length];
    for (int i = 0; i < colors.Length; i++)
    {
      xColors[i] = new XColor(colors[i]);
    }
    return xColors;
  }

  [DebuggerHidden]
  public static Color[] ToColors(XColor[] xColors)
  {
    var colors = new Color[xColors.Length];
    for (int i = 0; i < xColors.Length; i++)
    {
      colors[i] = xColors[i].ToColor();
    }
    return colors;
  }
}
