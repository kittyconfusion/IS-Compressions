using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static Map_Generator_CSharp.Source.tiles.Layer;

namespace IS_Compressions.code.display;
internal class Tile
{
    private Pixel[] pixels;

    private int width, height;
    private int xLoc, yLoc;
    private bool needsDraw = true;
    int count = 0;

    public void SetDrawFlag()
    {
        needsDraw = true;

    }
    public bool GetDrawFlag()
    {
        return needsDraw;
    }
    public void Draw(ref List<Vertex> vertices, ref LayerSettings settings, ref ColorCache renderCache)
    {
        needsDraw = false;
        vertices.Clear();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var screenPos = new Vector2f((xLoc * width) + x + settings.xOffset, (yLoc * height) + y + settings.yOffset);
                var t = GetPixel(x, y);

                //var screenPos = new Vector2f(0, 0);
                Color initialCol = t.GetColor();
                
                if(initialCol.A == 0) { continue; }

                

                var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);

                Color col = CalculateColor(initialCol, screenPixel, settings);
                
                var v = new Vertex(screenPos + new Vector2f(0, 0), col);
                vertices.Add(v);

                v = new Vertex(screenPos + new Vector2f(1, 0), col);
                vertices.Add(v);

                v = new Vertex(screenPos + new Vector2f(1, 1), col);
                vertices.Add(v);

                v = new Vertex(screenPos + new Vector2f(0, 1), col);
                vertices.Add(v);
                
                //renderCache.SetCachedColor((xLoc * width) + x, (yLoc * height))
            }
        }
    }
    private Color CalculateColor(Color a, Color b, LayerSettings settings)
    {
        float alphaA = (a.A / 255f);

        float alphaB = (b.A / 255f);

        var alphaOver = Math.Round(255 * settings.opacity * (alphaA + (alphaB * (1 - alphaA))));

        byte red = (byte)(int)Math.Round(a.R + (b.R * alphaA));
        byte green = (byte)(int)Math.Round(count + a.G + (b.G * alphaA));
        byte blue = (byte)(int)Math.Round(count + a.B + (b.B * alphaA));

        //if (settings.debug) { count++; }
        //count++;
        if(count > 60)
        {
            count = 0;
        }

        //Console.WriteLine(alphaOver);
        return new Color(red, green, blue, (byte)(int)(alphaOver));

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
        //pixels[(y * width) + x].RenderColor(0);
        return pixels[(y * width) + x];
    }

    public void SetPixel(int x, int y, Pixel pixel)
    {
        pixels[(y * width) + x] = pixel;
    }
}
