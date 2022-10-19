﻿using System;
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

    public void Draw(ref List<Vertex> vertices)
    {
        vertices.Clear();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {

                var t = GetPixel(x, y);
                var screenPos = new Vector2f(x, y);
                //var screenPos = new Vector2f(0, 0);
                Color col = t.getColor();
                

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
    public void Init(int x, int y)
    {
        width = x;
        height = y;
        pixels = new Pixel[width * height];
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
