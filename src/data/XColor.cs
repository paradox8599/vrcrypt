using UnityEngine;

[System.Serializable]
public class XColor
{
  public float r;
  public float g;
  public float b;
  public float a;

  public XColor(Color color)
  {
    r = color.r;
    g = color.g;
    b = color.b;
    a = color.a;
  }

  public Color ToColor()
  {
    return new Color(r, g, b, a);
  }

  public static XColor[] FromColors(Color[] colors)
  {
    var xColors = new XColor[colors.Length];
    for (int i = 0; i < colors.Length; i++)
    {
      xColors[i] = new XColor(colors[i]);
    }
    return xColors;
  }

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
