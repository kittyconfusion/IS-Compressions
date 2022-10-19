using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.display;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;

namespace Map_Generator_CSharp.Source.tiles;

class Layer
{
    private Tile[] tileMap;

    private int width, height;
    public struct LayerSettings
    {
        public int width;
        public int height;
        public string imageType;
        public int tileSize;

    }

    private LayerSettings settings = new LayerSettings();

    private List<Vertex> rectPoints;

    public Layer(int width, int height, int tileSize)
    {
        settings.width = width;
        settings.height = height;
        settings.tileSize = tileSize;
        rectPoints = new List<Vertex>(tileSize * tileSize);

        int tWidth = (int)Math.Ceiling((float)width / settings.tileSize);
        int tHeight = (int)Math.Ceiling((float)width / settings.tileSize);
        tileMap = new Tile[tWidth * tHeight];
        foreach (Tile tile in tileMap)
        {
            tile.Init(tWidth, tHeight);
        }
    }

    public void Render(ref RenderTexture renderTexture)
    {
        foreach(Tile tile in tileMap)
        {
            tile.Draw(ref rectPoints);
            renderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
        }
    }

    public void RenderOnScreen()
    {
       
    }

    public LayerSettings GetSettings()
    {
        return settings;
    }
    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < settings.width && y >= 0 && y < settings.height;
    }
    public Tile? GetPixelTile(int x, int y)
    {
        if (inBounds(x, y))
        {
            return tileMap[(x / settings.tileSize) + ((y / settings.tileSize) * (int)Math.Ceiling((float)width / settings.tileSize))];
        }
        else
        {
            return null;
        }
    }
    public Pixel? GetPixel(int x, int y) 
    {
        if (inBounds(x, y))
        {
            Tile t = tileMap[(x / settings.tileSize) + ((y / settings.tileSize) * (int)Math.Ceiling((float)width / settings.tileSize))];
            return t.GetPixel(x - (settings.tileSize * (x / settings.tileSize)),y - (y * (settings.tileSize / settings.tileSize)));
        }
        else
        {
            return null;
        }
        return tileMap[(y * settings.width) + x].GetPixel(x,y);
    }

    public int getWidth() {
        return settings.width;
    }

    public int getHeight() {
        return settings.height;
    }

}