using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static IS_Compressions.code.display.DisplayManager;

namespace IS_Compressions.code.display;
internal class LayerHolder
{
    internal readonly List<Layer> layers;
    //readonly OverlayLayer overlay;
    internal readonly int tileSize;

    internal RenderTexture screenRenderTexture;
    internal ColorCache screenColorCache;
    internal Color backgroundColor;
    internal RenderWindow screen;

    public void SetLocation(int index, int xOffset, int yOffset)
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
    private void ResetDrawAtLayer(int index)
    {
        var startX = layers[index].GetSettings().xOffset;
        var startY = layers[index].GetSettings().yOffset;
        var width = (int)Math.Ceiling((float)layers[index].GetWidth() / tileSize);
        var height = (int)Math.Ceiling((float)layers[index].GetHeight() / tileSize);
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
    }
    public void Resize(uint x, uint y)
    {
        screenRenderTexture.Dispose();
        screenRenderTexture = new RenderTexture(x, y);
    }
    public LayerHolder(Layer backgroundLayer, RenderWindow screen, int tileSize)
    {
        screenRenderTexture = new RenderTexture((uint)backgroundLayer.GetWidth(), (uint)backgroundLayer.GetHeight());
        screenColorCache = new ColorCache((uint)backgroundLayer.GetWidth(), (uint)backgroundLayer.GetHeight());
        layers = new List<Layer>() { backgroundLayer };
        this.screen = screen;
        this.tileSize = tileSize;
    }

    public void Draw(int xOffset, int yOffset, float scale)
    {
        //screenRenderTexture.Clear();
        //RenderEntire();
        SmartRender(xOffset,yOffset,scale);
        screenRenderTexture.Display();
    }
    public void ResetAlreadyDrawn()
    {
        foreach(Layer l in layers)
        {
            l.ResetAlreadyDrawn();
        }
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
        layers[index].Render(ref screenRenderTexture, ref screenColorCache);
    }
    public void RenderEntire()
    {

        foreach(Layer layer in layers)
        {
            if(layer.GetSettings().visible)
            {
                layer.Render(ref screenRenderTexture, ref screenColorCache);
            }
        }
    }
    public void SmartRender(int screenX, int screenY, float scale)
    {
        foreach(Layer layer in layers)
        {
            if(layer.GetSettings().visible)
            {
                RenderEntire();
                //layer.RenderNeededTilesOnScreen(ref screenRenderTexture, ref screenColorCache, screenX, screenY, scale, screen.Size.X, screen.Size.Y);
            }
        }
        
    }
}
