public static class ByteExtension
{
    public static int PopCount(this byte b)
    {
        int count = 0;
        while(b != 0)
        {
            b &= (byte)(b - 1);
            count++;
        }
        //for (; b != 0; count++)
        //    b &= (byte)(b - 1);

        return count;
    }
}