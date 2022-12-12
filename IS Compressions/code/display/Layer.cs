using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using IS_Compressions.code.core;
using IS_Compressions.code.display;
using IS_Compressions.code.logic.macros;
using SFML.Graphics;
using SFML.System;
using Color = SFML.Graphics.Color;

namespace Map_Generator_CSharp.Source.tiles;

class Layer
{
    //private Tile[] tileMap;
    private Pixel[] pixels;
    internal List<PixelMacro> macros;
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
        public bool isTiled;
    }

    internal struct LayerMacroSettings
    {
        internal int HorizontalShift;
        internal int VerticalShift;

        internal void Reset()
        {
            HorizontalShift = 0;
            VerticalShift = 0;
        }
    }

    private LayerSettings s = new LayerSettings();
    private LayerMacroSettings ms = new LayerMacroSettings();

    public Layer(int width, int height, Color c) : this(width, height)
    {
        foreach (Pixel p in pixels)
        {
            p.SetColor(c);
        }
    }
    public Layer(int width, int height)
    {
        macros = new List<PixelMacro>();

        s.width = width;
        s.height = height;
        s.xOffset = 0;
        s.yOffset = 0;

        pixels = new Pixel[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Pixel();
            //tileMap[i].Init(tileSize, tileSize, i % tWidth, i / tWidth);
        }

        s.opacity = 1.0f;
        s.visible = true;
        s.debug = false;
        s.isTiled = false;
        //Console.WriteLine(tileMap.Length);
    }
    public void Render(ColorCache renderCache, List<Vector2i> renderCoords, ColorCache cache)
    {
        ms.Reset();
        SingleFrameScreenMacros();

        if (s.isTiled)
        {
            for (var x = 0; x < renderCache.width; x++)
            {
                for (var y = 0; y < renderCache.height; y++)
                {
                    var screenPos = new Vector2i(x + s.xOffset, y + s.yOffset);

                    Color initialCol = GetColor(screenPos);

                    if (initialCol.A == 0) { continue; }

                    var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);
                    Color col = RunEveryPixelMacros(Pixel.CalculateColor(initialCol, screenPixel, s), screenPixel, s);


                    renderCache.SetCachedColor(screenPos, col);
                }
            }
            
        }
        else {
            for (var x = 0; x < Math.Min(renderCache.width, s.width ); x++)
            {
                for (var y = 0; y < Math.Min(renderCache.height, s.height); y++)
                {
                    var screenPos = new Vector2i(x + s.xOffset + ms.HorizontalShift, y + s.yOffset + ms.VerticalShift);

                    //Color initialCol = GetColor(screenPos);
                    Color initialCol = pixels[x + (s.width * y)].GetColor();


                    if (initialCol.A == 0) { continue; }

                    //var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);
                    var screenPixel = renderCache.GetCachedColor(screenPos.X, screenPos.Y);
                    Color col = RunEveryPixelMacros(Pixel.CalculateColor(initialCol, screenPixel, s), screenPixel, s);
                    //Color col = Pixel.CalculateColor(initialCol, screenPixel, s);

                    //Color col = RunEveryPixelMacros(initialCol, screenPixel, s);

                    renderCache.SetCachedColor(screenPos, col);
                }
            }

            /*var ltopleft = new Vector2i(s.xOffset, s.yOffset);
            var lbotright = new Vector2i(s.xOffset + s.width, s.yOffset + s.height);

            for (var i = 0; i < renderCoords.Count; i++)
            {
                var topleft = renderCoords[i];
                var botright = renderCoords[i] + new Vector2i(Display.TILE_SIZE, Display.TILE_SIZE);

                for (var x = Math.Max(topleft.X - s.xOffset, 0); x < Math.Min(botright.X - s.xOffset, Math.Min(s.width, Display.settings.pixelWidth)); x++)
                {
                    for (var y = Math.Max(topleft.Y - s.yOffset, 0); y < Math.Min(botright.Y - s.yOffset, Math.Min(s.height, Display.settings.pixelHeight)); y++)
                    {
                        var screenPos = new Vector2i(x + s.xOffset, y + s.yOffset);

                        //var screenPos = new Vector2f(0, 0);
                        Color initialCol = GetPixel(x, y).GetColor();

                        if (initialCol.A == 0) { continue; }


                        var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);

                        Color col = Pixel.CalculateColor(initialCol, screenPixel, s);

                        //Console.WriteLine("R(" + col.R + ") G(" + col.G + ") B(" + col.B + ") A(" + col.A + ")  ");

                        renderCache.SetCachedColor(screenPos, col);
                    }
                }
            }*/
        }
    }
    private int CorrectModulus(int a, int b)
    {
        return b * (a / b) + (a % b);
    }
    int mod(int a, int b)
    {
        if (b < 0) //you can check for b == 0 separately and do what you want
            return -mod(-a, -b);
        int ret = a % b;
        if (ret < 0)
            ret += b;
        return ret;
    }
        private Color GetColor(Vector2i pos)
    {
        int x = mod(pos.X + ms.HorizontalShift, s.width);
        int y = s.width * mod(pos.Y + ms.VerticalShift, s.height);
        //Console.WriteLine(pos.X + " " + pos.Y + " " + ((pos.X + ms.HorizontalShift) % s.width) + " " + (s.width * ((pos.Y + ms.VerticalShift) % s.height)));
        /*try { return pixels[((pos.X + ms.HorizontalShift) % s.width) + (s.width * (((pos.Y + ms.VerticalShift) % s.height)))].GetColor();
            
        }
        catch (Exception e)
        {
            Console.WriteLine(pos.X + " " + pos.Y + " " + ms.HorizontalShift + " " + ms.VerticalShift);
            Console.WriteLine(pixels.Length);
            Console.WriteLine(((pos.X + ms.HorizontalShift) % s.width) + (s.width * ((pos.Y + ms.VerticalShift) % s.height)));
            Console.WriteLine(((pos.X + ms.HorizontalShift) % s.width) + " " + (s.width * (((pos.Y + ms.VerticalShift) % s.height))));
            Console.WriteLine();
            return Color.White;
        }
        */
        try
        {
            return pixels[x + y].GetColor();
        } 
        catch (Exception e)
        {
            Console.WriteLine(x + " " + y);
        }

        return Color.White;
    }
    private Color RunEveryPixelMacros(Color initialCol, Color screenPixel, LayerSettings s)
    {
        Color current = initialCol;
        foreach (PixelMacro m in macros)
        {
            if(m.CalledEveryPixel)
            {
                current = m.PixelDraw(current, screenPixel, s);
            }
        }
        return current;
    }
    private void SingleFrameScreenMacros()
    {
        foreach (PixelMacro m in macros)
        {
            if(!m.CalledEveryPixel)
            {
                m.UpdateSettings(s, ref ms);
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

        if (s.isTiled) 
        { 
            return pixels[((y % s.height ) * s.width) + (x % s.width)];
        }
        else
        {
            if (inBounds(x, y))
            {
                return pixels[(y * s.width) + x];
            }
            else
            {
                return null;
            }
        }

        
    }

    public void Fill(Color c)
    {
        for (var i = 0; i < s.width * s.height; i++) {
            pixels[i].SetColor(c);
        }
    }

    public int GetWidth() {
        return s.width;
    }

    public int GetHeight() {
        return s.height;
    }

}