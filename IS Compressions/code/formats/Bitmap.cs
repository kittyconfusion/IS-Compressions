using IS_Compressions.code.util;
using SFML.Graphics;

namespace IS_Compressions.code.formats;
public class Bitmap
{
    Color[] colorData;
    public readonly int width;
    public readonly int height;
    public readonly int bitsPerPixel;
    ByteFile f;

    public Color getColor(int x, int y)
    {
        return colorData[(y * width) + x];
    }
    public Bitmap(string file)
    {
        using (f = new ByteFile(file, 'r', false))
        {
            f.Skip(10);
            //Used to skip rest of metadata
            int imageStartOffset = f.ReadHexVals(4) - 0x1B;

            f.Skip(4);
            width = f.ReadHexVals(4);
            height = f.ReadHexVals(4);
            f.Skip(2);
            bitsPerPixel = f.ReadHexVals(2);
            f.Skip(imageStartOffset);

            Console.WriteLine(width + " x " + height);
            colorData = new Color[width * height];

            //Color data stored in blue, green, red order
            byte[] bgr = new byte[3];
            for (int y = 0; y < height; y++)
            {
                //Console.WriteLine(y);
                for (int x = 0; x < width; x++)
                {
                    f.ReadBytes(ref bgr);

                    colorData[(y * width) + x] = new Color(bgr[2], bgr[1], bgr[0]);
                }
                //Calculate padding at the end of every image row
                f.Skip(width % 4);
                //f.Skip(2);
            }
        }
    }
}
