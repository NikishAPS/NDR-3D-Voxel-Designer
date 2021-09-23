public static class ColorConverter
{
    public static RGBColor ConvertHSV2RGB(HSVColor hsv)
    {
        float h = hsv.H * 360;
        float s = hsv.S * 100;
        float v = hsv.V * 100;

        int hI = ((int)h / 60) % 6;
        float vMin = ((100f - s) * v) / 100;
        float a = (v - vMin) * (h % 60) / 60;

        float vInc = vMin + a;
        float vDec = v - a;

        RGBColor rgb;

        switch(hI)
        {
            case 0:
                rgb = new RGBColor(v, vInc, vMin);
                break;

            case 1:
                rgb = new RGBColor(vDec, v, vMin);
                break;

            case 2:
                rgb = new RGBColor(vMin, v, vInc);
                break;

            case 3:
                rgb = new RGBColor(vMin, vDec, v);
                break;

            case 4:
                rgb = new RGBColor(vInc, vMin, v);
                break;

            default:
                rgb = new RGBColor(v, vMin, vDec);
                break;
        }

        rgb.R = rgb.R * 255 / 100;
        rgb.G = rgb.G * 255 / 100;
        rgb.B = rgb.B * 255 / 100;


        return rgb;
    }

    public static HSVColor ConvertRGB2HSV(RGBColor rgb)
    {
        float r = rgb.R / 255f;
        float g = rgb.G / 255f;
        float b = rgb.B / 255f;

        if (r == 0 && g == 0 && b == 0) return new HSVColor();
        if (r == 1 && g == 1 && b == 1) return new HSVColor(0, 0, 1);

        float max = r;
        if (g > max) max = g;
        if (b > max) max = b;

        float min = r;
        if (g < min) min = g;
        if (b < min) min = b;

        float h = 0;
        if (max == r && g >= b)  h = 60f * (g - b) / (max - min) + 0;
        if (max == r && g < b)  h = 60f * (g - b) / (max - min) + 360;
        if (max == g)  h = 60f * (b - r) / (max - min) + 120;
        if (max == b)  h = 60f * (r - g) / (max - min) + 240;
        h /= 360f;
        h = 1 - h;

        float s = (max == 0) ? 0 : 1 - min / max;

        float v = max;

        return new HSVColor(h, s, v);
    }

}
