using System.Drawing;
using IS_Compressions.code.core;
using IS_Compressions.code.display;
using SFML.Graphics;
using SFML.System;
using Color = SFML.Graphics.Color;

namespace Map_Generator_CSharp.Source.tiles;

class Layer
{
    //private Tile[] tileMap;
    private Pixel[] pixels;
    internal List<Vector2i[]> drawCoords;
    public struct LayerSettings
    {
        public string name;
        public int width;
        public int height;
        public string imageType;

        public int xOffset;
        public int yOffset;
        public bool visible;
        public float opacity;

        public bool debug;
    }
    
    private LayerSettings s = new LayerSettings();
    public Layer(int width, int height, int tileSize)
    {
        s.width = width;
        s.height = height;
        s.xOffset = 0;
        s.yOffset = 0;

        pixels = new Pixel[width * height];
        for(int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Pixel();
            //tileMap[i].Init(tileSize, tileSize, i % tWidth, i / tWidth);
        }

        s.opacity = 1.0f;
        s.visible = true;
        s.debug = false;
        //Console.WriteLine(tileMap.Length);
    }
    public void Render(ref ColorCache renderCache, List<Vector2i> renderCoords)
    {
        var ltopleft = new Vector2i(s.xOffset, s.yOffset);
        var lbotright = new Vector2i(s.xOffset + s.width, s.yOffset + s.height);

        for (var i = 0; i < renderCoords.Count; i ++) 
        {
            var topleft = renderCoords[i];
            var botright = renderCoords[i] + new Vector2i(Display.tileSize,Display.tileSize);

            //If the render rectangle at all overlaps with the sprite, render
            //if (!(ltopleft.X < botright.X && lbotright.X > topleft.X &&
            //    ltopleft.Y > botright.Y && botright.Y < topleft.Y))
            //if(true)
            //{
                for (var x = Math.Max(topleft.X - s.xOffset, 0); x < Math.Min(botright.X - s.xOffset, s.width); x++)
                {
                    for (var y = Math.Max(topleft.Y - s.yOffset, 0); y < Math.Min(botright.Y - s.yOffset, s.height); y++)
                    {
                        var screenPos = new Vector2i(x + s.xOffset, y + s.yOffset);
                        var t = GetPixel(x, y);

                        //var screenPos = new Vector2f(0, 0);
                        Color initialCol = t.GetColor();

                        if (initialCol.A == 0) { continue; }


                        var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);

                        Color col = Pixel.CalculateColor(initialCol, screenPixel, s);

                        //Console.WriteLine("R(" + col.R + ") G(" + col.G + ") B(" + col.B + ") A(" + col.A + ")  ");

                        renderCache.SetCachedColor(screenPos, col);
                    }
                //}
            }
        }
    }


    //Only renders what is visible to the camera
    /*
    public void RenderNeededTilesOnScreen(ref RenderTexture renderTexture, ref ColorCache renderCache, 
        int screenX, int screenY, float scale, uint windowWidth, uint windowHeight)
    {
        for (int x = -(int)(windowWidth * 0.5f); x <= (int)(windowWidth * 0.5f); x += (int)(s.tileSize))
        {
            for(int y = -(int)(windowHeight * 0.5f); y <= (int)(windowHeight * 0.5f); y += (int)(s.tileSize))
            {
                Tile? t = GetTileRelativeToScreen(screenX - x, screenY - y);
                if(t != null)
                {
                    if (t.GetDrawFlag())
                    {
                        t.Draw(ref rectPoints, ref s, ref renderCache);

                        renderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
                    }
                }
            }
        }
    }
    */

    public ref LayerSettings GetSettings()
    {
        return ref s;
    }
    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < s.width && y >= 0 && y < s.height;
    }
    /*
    public Tile? GetPixelTile(int x, int y)
    {
        int index = (x / s.tileSize) + ((y / s.tileSize) * (int)Math.Ceiling((float)s.width / s.tileSize));
        if (index < tileMap.Length && index > -1)
        {
            return tileMap[index];
        }
        else
        {
            return null;
        }
    }
    */
    /*
    public Tile? GetTileRelativeToScreen(int x, int y)
    {
        return GetPixelTile(x + s.xOffset, y + s.yOffset);
    }
    */
    public Pixel? GetPixelRelativeToScreen(int x, int y)
    {
        return GetPixel(x + s.xOffset, y + s.yOffset);
    }
    public void ChangePixelColor(int x, int y, Color c)
    {
        //int pixelX = x % s.tileSize;// - (s.tileSize * (x / s.tileSize));
        //int pixelY = y % s.tileSize;// - (y * (s.tileSize / s.tileSize));

        if (inBounds(x, y))
        {
            //int tileIndex = (x / s.tileSize) + ((y / s.tileSize) * (int)Math.Ceiling((float)s.width / s.tileSize));
            //tileMap[tileIndex].SetDrawFlag();
            GetPixel(x, y).SetColor(c);
        }
    }
    public Pixel? GetPixel(int x, int y) 
    {
        //int pixelX = x % s.tileSize;// - (s.tileSize * (x / s.tileSize));
        //int pixelY = y % s.tileSize;// - (y * (s.tileSize / s.tileSize));

        if (inBounds(x, y))
        {
            //int tileIndex = (x / s.tileSize) + ((y / s.tileSize) * (int)Math.Ceiling((float)s.width / s.tileSize));
            //Console.WriteLine(x + " " + y + " " + tileIndex + " " + (float)width / s.tileSize + " " + (y / s.tileSize));
            //Console.WriteLine(s.width);
            return pixels[(y * s.width) + x];
        }
        else
        {
            //Console.WriteLine("Tried to get nonexistant pixel at (" + pixelX + ", " + pixelY + ")");
            return null;
        }
        //return tileMap[(y * s.width) + x].GetPixel(x,y);
    }

    public int GetWidth() {
        return s.width;
    }

    public int GetHeight() {
        return s.height;
    }

}