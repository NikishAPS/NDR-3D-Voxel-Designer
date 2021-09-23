using UnityEngine;

public static class RGBColorExtension
{
    public static Color ToColor(this RGBColor rgbColor)
    {
        return new Color(rgbColor.R /255f , rgbColor.G / 255f, rgbColor.B / 255f, 1f);
    }
}
