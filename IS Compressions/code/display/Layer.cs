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
using SFML.Window;

namespace Map_Generator_CSharp.Source.tiles;

class Layer
{
    private Tile[] tileMap;

    public struct LayerSettings
    {
        public string name;
        public int width;
        public int height;
        public string imageType;
        public int tileSize;

        public int xOffset;
        public int yOffset;
        public bool visible;
        public float opacity;

        public bool debug;
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

        settings.opacity = 1.0f;
        settings.visible = true;
        settings.debug = false;
        //Console.WriteLine(tileMap.Length);
    }
    internal void ResetAlreadyDrawn()
    {
        foreach(Tile t in tileMap)
        {
            t.SetDrawFlag();
        }
    }
    public void Render(ref RenderTexture renderTexture, ref ColorCache renderCache)
    {
        foreach(Tile tile in tileMap)
        {
            if (tile.GetDrawFlag())
            {
                tile.Draw(ref rectPoints, ref settings, ref renderCache);

                renderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
            }
        }
    }

    //Only renders what is visible to the camera
    public void RenderNeededTilesOnScreen(ref RenderTexture renderTexture, ref ColorCache renderCache, 
        int screenX, int screenY, float scale, uint windowWidth, uint windowHeight)
    {
        for (int x = -(int)(windowWidth * 0.5f); x <= (int)(windowWidth * 0.5f); x += (int)(settings.tileSize))
        {
            for(int y = -(int)(windowHeight * 0.5f); y <= (int)(windowHeight * 0.5f); y += (int)(settings.tileSize))
            {
                Tile? t = GetTileRelativeToScreen(screenX - x, screenY - y);
                if(t != null)
                {
                    if (t.GetDrawFlag())
                    {
                        t.Draw(ref rectPoints, ref settings, ref renderCache);

                        renderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
                    }
                }
            }
        }
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
        int index = (x / settings.tileSize) + ((y / settings.tileSize) * (int)Math.Ceiling((float)settings.width / settings.tileSize));
        if (index < tileMap.Length && index > -1)
        {
            return tileMap[index];
        }
        else
        {
            return null;
        }
    }
    public Tile? GetTileRelativeToScreen(int x, int y)
    {
        return GetPixelTile(x + settings.xOffset, y + settings.yOffset);
    }
    public Pixel? GetPixelRelativeToScreen(int x, int y)
    {
        return GetPixel(x + settings.xOffset, y + settings.yOffset);
    }
    public void ChangePixelColor(int x, int y, Color c)
    {
        int pixelX = x % settings.tileSize;// - (settings.tileSize * (x / settings.tileSize));
        int pixelY = y % settings.tileSize;// - (y * (settings.tileSize / settings.tileSize));

        if (inBounds(x, y))
        {
            int tileIndex = (x / settings.tileSize) + ((y / settings.tileSize) * (int)Math.Ceiling((float)settings.width / settings.tileSize));
            tileMap[tileIndex].SetDrawFlag();
            tileMap[tileIndex].GetPixel(pixelX, pixelY).SetColor(c);
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
            //Console.WriteLine("Tried to get nonexistant pixel at (" + pixelX + ", " + pixelY + ")");
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