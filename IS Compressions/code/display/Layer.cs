using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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

    public struct LayerSettings
    {
        public int width;
        public int height;
        public int xOffset;
        public int yOffset;
        public string imageType;
        public int tileSize;


        public bool visible;
        public float opacity;


    }

    private LayerSettings settings = new LayerSettings();

    private List<Vertex> rectPoints;

    public Layer(int width, int height, int tileSize)
    {
        settings.width = width;
        settings.height = height;
        settings.tileSize = tileSize;
        settings.xOffset = 0;
        settings.yOffset = 0;
        rectPoints = new List<Vertex>(tileSize * tileSize  *  4);

        int tWidth = (int)Math.Ceiling((float)width / settings.tileSize);
        int tHeight = (int)Math.Ceiling((float)height / settings.tileSize);
        tileMap = new Tile[tWidth * tHeight];
        for(int i = 0; i < tileMap.Length; i++)
        {
            tileMap[i] = new Tile();
            tileMap[i].Init(tileSize, tileSize, i % tWidth, i / tWidth);
        }

        settings.opacity = 1f;
        settings.visible = true;
        //Console.WriteLine(tileMap.Length);
    }
    
    public void Render(ref ColorCache renderTexture)
    {

        foreach(Tile tile in tileMap)
        {
            tile.Draw(ref rectPoints, settings);
            
            renderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
        }
    }

    public void RenderOnScreen()
    {
       
    }

    public ref LayerSettings GetSettings()
    {
        return ref settings;
    }
    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < settings.width && y >= 0 && y < settings.height;
    }
    public Tile? GetPixelTile(int x, int y)
    {
        if (inBounds(x, y))
        {
            return tileMap[(x / settings.tileSize) + ((y / settings.tileSize) * (int)Math.Ceiling((float)settings.width / settings.tileSize))];
        }
        else
        {
            return null;
        }
    }
    public Pixel? GetPixel(int x, int y) 
    {
        int pixelX = x % settings.tileSize;// - (settings.tileSize * (x / settings.tileSize));
        int pixelY = y % settings.tileSize;// - (y * (settings.tileSize / settings.tileSize));

        if (inBounds(x, y))
        {
            int tileIndex = (x / settings.tileSize) + ((y / settings.tileSize) * (int)Math.Ceiling((float)settings.width / settings.tileSize));
            //Console.WriteLine(x + " " + y + " " + tileIndex + " " + (float)width / settings.tileSize + " " + (y / settings.tileSize));
            //Console.WriteLine(settings.width);
            return tileMap[tileIndex].GetPixel(pixelX, pixelY);
        }
        else
        {
            Console.WriteLine("Tried to get nonexistant pixel at (" + pixelX + ", " + pixelY + ")");
            return null;
        }
        //return tileMap[(y * settings.width) + x].GetPixel(x,y);
    }

    public int GetWidth() {
        return settings.width;
    }

    public int GetHeight() {
        return settings.height;
    }

}