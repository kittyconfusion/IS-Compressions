using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.display;
internal class LayerHolder
{
    List<Layer> layers;
    OverlayLayer overlay;

    private RenderTexture screenRenderTexture;

    internal Sprite renderedTexture;

    public LayerHolder()
    {
        screenRenderTexture = new RenderTexture((uint)tm.getWidth(), (uint)tm.getHeight());
    }

    public void Draw()
    {
        
    }

    public Color GetRenderColor(int x, int y)
    {
    
    }
    public Pixel GetPixelAtPosOnTop(int x, int y)
    {
    
    }
    public Pixel[] GetPixelsAtPos(int x, int y)
    {
    
    }
    public void ChangeLayerOrder(int layerIndex, int newPos)
    {
    
    }
    public void RenderEntireLayer(int index)
    {
        layers[index].Render();
    }
    public void RenderEntire()
    {

        foreach(Layer layer in layers)
        {
            layer.Render(ref screenRenderTexture);
            
        }
    }
}
