using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.display;
internal class Tile
{
    private Pixel[] pixels;

    int width, height;
    int xLoc, yLoc;

    public void Draw(ref List<Vertex> vertices, int xOffset, int yOffset)
    {
        vertices.Clear();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {

                var t = GetPixel(x, y);
                var screenPos = new Vector2f((xLoc * width) + x + xOffset, (yLoc * height) + y + yOffset);
                //var screenPos = new Vector2f(0, 0);
                Color col = t.GetColor();
                
                var v = new Vertex(screenPos + new Vector2f(0, 0), col);
                vertices.Add(v);

                v = new Vertex(screenPos + new Vector2f(1, 0), col);
                vertices.Add(v);

                v = new Vertex(screenPos + new Vector2f(1, 1), col);
                vertices.Add(v);

                v = new Vertex(screenPos + new Vector2f(0, 1), col);
                vertices.Add(v);
            }
        }
    }
    public void Init(int width, int height, int xLoc, int yLoc)
    {
        Console.WriteLine("Tile initiating with size (" + width + ", " + height + ") and location (" + xLoc + ", " + yLoc + ")");
        this.width = width;
        this.height = height;
        this.xLoc = xLoc;
        this.yLoc = yLoc;
        pixels = new Pixel[width * height];
        for(int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Pixel();
        }
    }
    public Pixel GetPixel(int x, int y)
    {
        //Console.WriteLine(pixels.Length + " Attempted to access " + ((y * width) + x));
        return pixels[(y * width) + x];
    }

    public void SetPixel(int x, int y, Pixel pixel)
    {
        pixels[(y * width) + x] = pixel;
    }
}
