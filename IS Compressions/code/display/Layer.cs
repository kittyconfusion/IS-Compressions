using System.Drawing;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Sources;
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
        internal float HorizontalShear;
        internal float VerticalShear;
        internal float opacity;
        internal float effectiveOpacity;
        internal void Reset()
        {
            HorizontalShift = 0;
            VerticalShift = 0;
            HorizontalShear = 0f;
            VerticalShear = 0f;
            opacity = 0;
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

        ms.effectiveOpacity = Math.Max(Math.Min(s.opacity + ms.opacity, 1), 0);

        if (ms.effectiveOpacity == 0 || !s.visible)
        {
            return;
        }
        

        if (s.isTiled)
        {
            for (var x = 0; x < renderCache.width; x++)
            {
                for (var y = 0; y < renderCache.height; y++)
                {
                    //var screenPos = new Vector2i(x + s.xOffset, y + s.yOffset);
                    var screenPos = new Vector2i(x , y);

                    Color initialCol = GetColor(screenPos);

                    if (initialCol.A == 0) { continue; }

                    var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);
                    Color col = RunEveryPixelMacros(Pixel.CalculateColor(initialCol, screenPixel, s, ms), screenPixel, s);


                    renderCache.SetCachedColor(screenPos, col);
                }
            }
            
        }
        else {
            for (var x = 0; x < Math.Min(renderCache.width, s.width ); x++)
            {
                for (var y = 0; y < Math.Min(renderCache.height, s.height); y++)
                {
                    var screenPos = new Vector2i(x + s.xOffset + ms.HorizontalShift + (int)(ms.HorizontalShear * y), y + s.yOffset + ms.VerticalShift + (int)(ms.VerticalShear * x));

                    //Color initialCol = GetColor(screenPos);
                    Color initialCol = pixels[x + (s.width * y)].GetColor();


                    if (initialCol.A == 0) { continue; }

                    //var screenPixel = renderCache.GetCachedColor((int)screenPos.X, (int)screenPos.Y);
                    var screenPixel = renderCache.GetCachedColor(screenPos.X, screenPos.Y);
                    Color col = RunEveryPixelMacros(Pixel.CalculateColor(initialCol, screenPixel, s, ms), screenPixel, s);
                    //Color col = Pixel.CalculateColor(initialCol, screenPixel, s);

                    //Color col = RunEveryPixelMacros(initialCol, screenPixel, s);

                    renderCache.SetCachedColor(screenPos, col);
                }
            }
        }
    }
    

    private Color GetColor(Vector2i pos)
    {
        int x = Features.Mod(pos.X + ms.HorizontalShift + s.xOffset + (int)(ms.HorizontalShear * pos.Y ), s.width);
        int y = s.width * Features.Mod(pos.Y + s.yOffset + ms.VerticalShift + (int)(ms.VerticalShear * pos.X), s.height);

        return pixels[x + y].GetColor();
    }
    
    private Color RunEveryPixelMacros(Color initialCol, Color screenPixel, LayerSettings s)
    {
        Color current = initialCol;
        foreach (PixelMacro m in macros)
        {
            if(m.CalledEveryPixel && m.IsActive)
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
                m.Update(ref s, ref ms);
            }
        }
    }



    //Only renders what is
    //to the camera
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
    public ref LayerMacroSettings GetMacroSettings()
    {
        return ref ms;
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

    public void SetTransparent(Color c)
    {
        foreach(Pixel p in pixels) { 
            if(p.GetColor().Equals(c))
            {
                p.SetColor(new Color(c.R, c.G, c.B, 0));
            }
        }
    }

    public int GetWidth() {
        return s.width;
    }

    public int GetHeight() {
        return s.height;
    }

    public static Layer operator *(Layer lhs, Layer rhs)
    {
        Layer l = new Layer(lhs.GetWidth(), lhs.GetHeight());
        for (var x = 0; x < lhs.GetWidth(); x++)
        {
            for (var y = 0; y < lhs.GetHeight(); y++)
            {
                l.ChangePixelColor(x, y, lhs.GetColor(new Vector2i(0, 0)) * rhs.GetColor(new Vector2i(0, 0)));
            }
        }
        return l;
    }
}