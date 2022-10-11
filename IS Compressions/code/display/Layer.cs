using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.display;
using Map_Generator_CSharp.Source.tiles;

namespace Map_Generator_CSharp.Source.tiles;

class Layer
{
    
    private Tile[] tileMap;
    public struct TilemapSettings
    {
        public int width;
        public int height;
        public string imageType;
        public int tileSize;

    }

    private TilemapSettings settings = new TilemapSettings();

    public Layer(int width, int height)
    {
        settings.width = width;
        settings.height = height;
        int tWidth = (int)Math.Ceiling((float)width / settings.tileSize);
        int tHeight = (int)Math.Ceiling((float)width / settings.tileSize);
        tileMap = new Tile[tWidth * tHeight];

    }
    public void rerenderTiles(int displayMode)
    {
        for (int x = 0; x < settings.width; x++)
        {
            for (int y = 0; y < settings.height; y++)
            {
                getTile(x, y).renderColor(displayMode);
            }
        }
    }
    public TilemapSettings GetSettings()
    {
        return settings;
    }
    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < settings.width && y >= 0 && y < settings.height;
    }
    public Pixel? GetPixel(int x, int y) 
    {
        if (inBounds(x, y))
        {

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