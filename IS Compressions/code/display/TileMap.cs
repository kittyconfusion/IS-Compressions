using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;

namespace Map_Generator_CSharp.Source.tiles;

class TileMap
{
    
    private Tile[] tileMap;
    public struct TilemapSettings
    {
        public int width;
        public int height;
        public string imageType;

    }

    private TilemapSettings settings = new TilemapSettings();

    public TileMap(int width, int height)
    {
        settings.width = width;
        settings.height = height;
        tileMap = new Tile[width * height];
        Random rand = new Random();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tileMap[(width * y) + x] = new Tile(rand);
            }
        }

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
    public Tile getTile(int x, int y) 
    {
        return tileMap[(y * settings.width) + x] ;
    }

    public int getWidth() {
        return settings.width;
    }

    public int getHeight() {
        return settings.height;
    }

}