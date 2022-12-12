using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.display;

internal class ColorCache
{
    internal uint width;
    internal uint height;

    private Color[] colors;
    private int colLen;

    public ColorCache(uint width, uint height)
    {
        this.width = width;
        this.height = height;
        colors = new Color[width * height];
        colLen = colors.Length - 1;
    }

    public ColorCache(ColorCache c)
    {
        this.width = c.width;
        this.height = c.height;
        this.colors = (Color[]?)c.colors.Clone();
        this.colLen = c.colLen;
    }

    public void CopyColors(ColorCache c)
    {
        this.colors = (Color[]?)c.colors.Clone();
    }


    public bool InBounds(int tryX, int tryY)
    {
        if (tryX < 0 || tryY < 0 || tryX >= width || tryY >= height)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void SetCachedColor(Vector2i pos, Color c)
    {
        colors[(pos.Y * width) + pos.X] = c;
    }
    public void SetCachedColorIfInBounds(int x, int y, Color c)
    {
        if(InBounds(x, y))
        {
            colors[(y * width) + x] = c;
        }
    }
    public void SetCachedColor(int x, int y, Color c)
    {
        colors[(y * width) + x] = c;
    }
    public Color GetCachedColor(int x, int y)
    {
        if((y * width) + x > colLen || (y * width) + x < 0)
        {
            return new Color(0, 0, 0, 255);
        }
        return colors[(y * width) + x];
    }
    public void ClearColors(Color c)
    {
        for (int i = 0; i < width * height; i++)
        {
            colors[i] = c;
        }
    }
}
