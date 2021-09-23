public class HSVColor
{
    public float H { get; set; }
    public float V { get; set; }
    public float S { get; set; }

    public HSVColor()
    {
        H = S = V = 0;
    }

    public HSVColor(float h, float s, float v)
    {
        H = h;
        S = s;
        V = v;
    }

}
