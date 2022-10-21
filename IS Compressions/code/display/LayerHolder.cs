using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static IS_Compressions.code.display.DisplayManager;

namespace IS_Compressions.code.display;
internal class LayerHolder
{
    List<Layer> layers;
    OverlayLayer overlay;

    internal RenderTexture screenRenderTexture;

    public void Resize(uint x, uint y)
    {
        screenRenderTexture = new RenderTexture(x, y);
    }
    public LayerHolder(Layer backgroundLayer)
    {
        screenRenderTexture = new RenderTexture((uint)backgroundLayer.GetWidth(), (uint)backgroundLayer.GetHeight());
        layers = new List<Layer>() { backgroundLayer };
    }

    public void Draw()
    {
        screenRenderTexture.Clear();
        RenderEntire();
        screenRenderTexture.Display();
    }

    public Color GetRenderColor(int x, int y)
    {
        return new Color();
    }
    public Pixel GetPixelAtPosOnTop(int x, int y)
    {
        return null;
    }
    public Pixel[] GetPixelsAtPos(int x, int y)
    {
        return null;
    }
    public void AddLayer(Layer l)
    {
        layers.Add(l);
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
        layers[index].Render(ref screenRenderTexture);
    }
    public void RenderEntire()
    {

        foreach(Layer layer in layers)
        {
            if(layer.GetSettings().visible)
            {
                layer.Render(ref screenRenderTexture);
            }
        }
    }
}
