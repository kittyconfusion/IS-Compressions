using SFML.Graphics;

namespace IS_Compressions.code.display;

internal class ColorCache
{
    private uint width;
    private uint height;

    private Color[] colors;

    public ColorCache(uint width, uint height)
    {
        this.width = width;
        this.height = height;
        colors = new Color[width * height];
    }
    public void SetCachedColor(int x, int y, Color c)
    {
        colors[(y * width) + x] = c;
    }
    public Color GetCachedColor(int x, int y)
    {
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
