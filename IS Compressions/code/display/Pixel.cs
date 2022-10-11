using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using static Map_Generator_CSharp.Source.tiles.Layer;

namespace Map_Generator_CSharp.Source.tiles;

internal class Pixel
{
    private Color colorCache;

    private bool needToRenderColor = true;

    public Pixel()
    {
        colorCache = new Color(0,0,0,0);
    } 
    public void setColor(Color c)
    {
        colorCache = c;
    }

    public Color getColor(int displayMode = 0)
    {
        if (needToRenderColor) { renderColor(displayMode); needToRenderColor = false; }
        switch (displayMode)
        {
            case 0:
                return colorCache;
            case 1:
                return new Color(colorCache.R, 0, 0);
            case 2:
                return new Color(0, colorCache.G, 0);
            case 3:
                return new Color(0, 0, colorCache.B);
            default:
                return colorCache;
        }
    }
    public void renderColor(int displayMode)
    {
        colorCache = new Color(colorCache.R, colorCache.G, colorCache.B, 255);
    }

};
