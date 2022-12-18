using SFML.Graphics;
using static Map_Generator_CSharp.Source.tiles.Layer;

namespace Map_Generator_CSharp.Source.tiles;

internal class Pixel
{
    private Color colorCache;
    private static Random r = new Random();
    private static Color Transparent = Color.White;
    internal static double starThreshold = 1;
    public Pixel()
    {
        if(r.NextDouble() > starThreshold)
        {
            colorCache = new Color((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255));
        }
        else
        {
            colorCache = Color.Black;
        }
        //colorCache = new Color(0,0,0,255);
        //colorCache = new Color(100, 100, 200, 0);
    }

    public void SetColor(Color c)
    {
        if(c.Equals(Transparent))
        {
            colorCache = new Color(c.R, c.G, c.B, 0);
        }
        else
        {
            colorCache = c;
        }
        RenderColor(0);
    }

    public Color GetColor(int displayMode = 0)
    {
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
    public void RenderColor(int displayMode)
    {
        colorCache = new Color(colorCache.R, colorCache.G, colorCache.B, colorCache.A);
    }
    internal static Color CalculateColor(Color a, Color b, LayerSettings s, LayerMacroSettings ms)
    { 
        
        float alphaA = ms.effectiveOpacity * (a.A / 255f);
        float alphaB = (b.A / 255f);

        var alphaOver = (alphaA + (alphaB * (1 - alphaA)));
         
        byte red =   (byte)(int)Math.Round((a.R * alphaA + (b.R * alphaB * (1 - alphaA))) / alphaOver);
        byte green = (byte)(int)Math.Round((a.G * alphaA + (b.G * alphaB * (1 - alphaA))) / alphaOver);
        byte blue =  (byte)(int)Math.Round((a.B * alphaA + (b.B * alphaB * (1 - alphaA))) / alphaOver);
        /*

        var alphaOver = Math.Round(255 * settings.opacity * (alphaA + (alphaB * (1 - alphaA))));
        
        byte red = (byte)(int)Math.Round(a.R + (b.R * (1 - alphaA)));
        byte green = (byte)(int)Math.Round(a.G + (b.G * (1 - alphaA)));
        byte blue = (byte)(int)Math.Round(a.B + (b.B * (1 - alphaA)));
        */
        return new Color(red, green, blue, (byte)(int)(alphaOver * 255));
        

    }
};
