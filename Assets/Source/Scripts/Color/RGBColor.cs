public class RGBColor
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }

    public RGBColor()
    {
        R = G = B = 0;
    }

    public RGBColor(float r, float g, float b)
    {
        R = (int)r;
        G = (int)g;
        B = (int)b;
    }

}