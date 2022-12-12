using System.Diagnostics;
using IS_Compressions.code.core;
using IS_Compressions.code.logic.macros;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.display;
internal class LayerHolder
{
    internal List<Layer> layers;
    internal List<Vector2i> drawCoords;
    //readonly OverlayLayer overlay;

    internal RenderTexture screenRenderTexture;
    internal ColorCache screenColorCache;
    internal ColorCache layerTempCache;
    internal Color backgroundColor;
    internal RenderWindow screen;

    List<Vertex> vertices;

    public void MoveAmount(int index, int xMove, int yMove)
    {
        ResetDrawAtLayer(index);
        layers[index].GetSettings().xOffset += xMove;
        layers[index].GetSettings().yOffset += yMove;
        ResetDrawAtLayer(index);
    }
    public void Move(int index, int xOffset, int yOffset)
    {
        ResetDrawAtLayer(index);
        layers[index].GetSettings().xOffset = xOffset;
        layers[index].GetSettings().yOffset = yOffset;
        ResetDrawAtLayer(index);
    }
    public void SetOpacity(int index, float value)
    {
        layers[index].GetSettings().opacity = value;
        ResetDrawAtLayer(index);
    }
    public void NextFrameMacroUpdate()
    {
        foreach (Layer l in layers)
        {
            foreach (Macro m in l.macros)
            {
                m.NextFrame();
            }
        }
    }
    private void ResetDrawAtLayer(int index)
    {
        var startX = layers[index].GetSettings().xOffset;
        var startY = layers[index].GetSettings().yOffset;

        var width = layers[index].GetWidth();
        var height = layers[index].GetHeight();

        if (layers[index].GetSettings().isTiled)
        {
            for (var sWidth = 0; sWidth < layers[0].GetWidth(); sWidth += Display.TILE_SIZE)
            {
                for (var sHeight = 0; sHeight < layers[0].GetHeight(); sHeight += Display.TILE_SIZE)
                {
                    if (!drawCoords.Contains(new Vector2i(sWidth, sHeight)))
                    {
                        drawCoords.Add(new Vector2i(sWidth, sHeight));
                    }
                }
            }
        }
        else
        {
            for (var sWidth = Display.TILE_SIZE * (startX / Display.TILE_SIZE); sWidth < startX + width; sWidth += Display.TILE_SIZE)
            {
                for (var sHeight = Display.TILE_SIZE * (startY / Display.TILE_SIZE); sHeight < startY + height; sHeight += Display.TILE_SIZE)
                {
                    if (!drawCoords.Contains(new Vector2i(sWidth, sHeight)))
                    {
                        drawCoords.Add(new Vector2i(sWidth, sHeight));
                    }
                }
            }
        }
        

        /*
        Console.WriteLine(width + " " + height);
        foreach (Layer l in layers)
        {
 
            var x = startX - l.GetSettings().xOffset;
            var y = startY - l.GetSettings().yOffset;
            for (int i = 0; i <= width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //GetPixelsAtPos
                    Tile? t = l.GetPixelTile(x + (i * tileSize), y + (j * tileSize));
                    if (t != null)
                    {
                        t.SetDrawFlag();
                    }
                }
            }
        }
        */
    }
    public void Resize(uint x, uint y)
    {
        screenRenderTexture.Dispose();
        screenRenderTexture = new RenderTexture(x, y);
    }
    public LayerHolder(Layer backgroundLayer, RenderWindow screen)
    {
        screenRenderTexture = new RenderTexture((uint)backgroundLayer.GetWidth(), (uint)backgroundLayer.GetHeight());
        screenColorCache = new ColorCache((uint)backgroundLayer.GetWidth(), (uint)backgroundLayer.GetHeight());
        layerTempCache = new ColorCache((uint)backgroundLayer.GetWidth(), (uint)backgroundLayer.GetHeight());
        layers = new List<Layer>() { backgroundLayer };
        drawCoords = new List<Vector2i>();
        this.screen = screen;
        this.vertices = new List<Vertex>(Display.TILE_SIZE * Display.TILE_SIZE * 8);
        
    }
    
    public void Draw()
    {
        //screenRenderTexture.Clear();
        RenderEntire();

        //SmartRender(xOffset,yOffset);
        screenRenderTexture.Display();
    }
    public void ResetAlreadyDrawn()
    {
        ResetDrawAtLayer(0);
    }

    public Color GetRenderColor(int x, int y)
    {
        return new Color();
    }
    public Layer? GetTopLayerAtPos(int x, int y)
    {
        foreach (Layer layer in layers)
        {
            Pixel? p = layer.GetPixelRelativeToScreen(x, y);
            if(p == null) { return null; }
            else
            {
                if (p.GetColor().A != 0) { return layer; }
            }
        }
        return null;
    }
    public Pixel? GetPixelOnTop(int x, int y)
    {
        foreach (Layer layer in layers)
        {
            Pixel? p = layer.GetPixelRelativeToScreen(x, y);
            if (p != null) { return p; }
        }
        return null;
    }
    public Pixel[] GetPixelsAtPos(int x, int y)
    {
        List<Pixel> p = new List<Pixel>();
        foreach(Layer layer in layers)
        {
            p.Add(layer.GetPixelRelativeToScreen(x, y));
        }
        return p.ToArray();
    }
    public void AddLayer(Layer l)
    {
        layers.Add(l);
    }
    public Layer? GetLayer(int index)
    {
        return layers[index];
    }
    public void SetSetting<T>(string s)
    {

    }
    public void ChangeLayerOrder(int layerIndex, int newPos)
    {
        if (layerIndex == newPos) { return; }
        Layer l = layers[layerIndex];
        layers.RemoveAt(layerIndex);
        layers.Insert(newPos, l);
    }
    public void RenderEntireLayer(int index)
    {
        layers[index].Render(screenColorCache, drawCoords, layerTempCache);
    }
    public void RenderEntire()
    {
        /*
        foreach (var coord in drawCoords)
        {
            for (var x = coord.X; x < coord.X + Display.TILE_SIZE; x++)
            {
                for (var y = coord.Y; y < coord.Y + Display.TILE_SIZE; y++)
                {
                    screenColorCache.SetCachedColorIfInBounds(x, y, Color.Black);
                }
            }
        }
        */
        screenColorCache.ClearColors(Color.Black);
        foreach (Layer layer in layers)
        {
            if (layer.GetSettings().visible && layer.GetSettings().opacity != 0f)
            {
                layer.Render(screenColorCache, drawCoords, layerTempCache);
            }
        }
        
        

        //foreach (Macro m in macros)
        //{
        //    ColorCache copy = new ColorCache(screenColorCache);
        //    m.Draw(screenColorCache, copy);
        //    screenColorCache = copy;
        //}

        

        vertices.Clear();
        //Random r = new Random();

        foreach(var coord in drawCoords)
        {
            for(var x = coord.X; x < coord.X + Display.TILE_SIZE; x++)
            {
                for(var y = coord.Y; y < coord.Y + Display.TILE_SIZE; y++)
                {
                    
                    /*
                    Color c = new Color((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255));

                    vertices.Add(new Vertex(new Vector2f(x + 0, y + 0), c));

                    vertices.Add(new Vertex(new Vector2f(x + 1, y + 0), c));

                    vertices.Add(new Vertex(new Vector2f(x + 1, y + 1), c));

                    vertices.Add(new Vertex(new Vector2f(x + 0, y + 1), c));
                    */
                    
                    vertices.Add(new Vertex(new Vector2f(x + 0, y + 0), screenColorCache.GetCachedColor(x + 0, y + 0)));
                    
                    //vertices.Add(new Vertex(new Vector2f(x + 1, y + 0), screenColorCache.GetCachedColor(x + 1, y + 0)));
                    
                    //vertices.Add(new Vertex(new Vector2f(x + 1, y + 1), screenColorCache.GetCachedColor(x + 1, y + 1)));
                    
                    //vertices.Add(new Vertex(new Vector2f(x + 0, y + 1), screenColorCache.GetCachedColor(x + 0, y + 1)));
                    
                }
            }

            if(vertices.Count == vertices.Capacity)
            {
                screenRenderTexture.Draw(vertices.ToArray(), PrimitiveType.Points);
                vertices.Clear();
            }
        }

        screenRenderTexture.Draw(vertices.ToArray(), PrimitiveType.Points);
        drawCoords.Clear();
    }
    public void SmartRender(int screenX, int screenY)
    {

        foreach(Layer layer in layers)
        {
            if(layer.GetSettings().visible)
            {
                
                //layer.RenderNeededTilesOnScreen(ref screenRenderTexture, ref screenColorCache, screenX, screenY, scale, screen.Size.X, screen.Size.Y);
            }
        }
        
    }
}
