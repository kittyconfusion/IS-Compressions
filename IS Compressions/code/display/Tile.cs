using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;

namespace IS_Compressions.code.display;
internal class Tile
{
    private Pixel[] pixels;

    int width, height;
    public Tile(int w, int h)
    {
        width = w;
        height = h;
        pixels = new Pixel[w * h];
    }
    public Pixel GetPixel(int x, int y)
    {
        return pixels[(y * width) + x];
    }

    public void SetPixel(int x, int y, Pixel pixel)
    {
        pixels[(y * width) + x] = pixel;
    }
}
