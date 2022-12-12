using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.display;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static Map_Generator_CSharp.Source.tiles.Layer;

namespace IS_Compressions.code.logic.macros;
internal abstract class PixelMacro : Macro
{
    internal bool CalledEveryPixel;
    internal struct FloatColor
    {
        internal float r;
        internal float g;
        internal float b;
        public FloatColor(float r, float g, float b)
        {
            this.r = r; this.g = g; this.b = b;
        }
        public FloatColor(Color color)
        {
            r = color.R; g = color.G; b = color.B;
        }
        public Color ToColor()
        {
            return new Color((byte)(r % 256), (byte)(g % 256), (byte)(b % 256));
        }
        public static FloatColor operator +(FloatColor lhs, FloatColor rhs) => new(lhs.r + rhs.r, lhs.g + rhs.g, lhs.b + rhs.b);

        public static FloatColor operator %(FloatColor lhs, int rhs) => new(lhs.r % rhs, lhs.g % rhs, lhs.b % rhs);
    }
    protected PixelMacro(int frames, int repeat, bool CalledEveryPixel) : base(frames, repeat)
    {
        this.CalledEveryPixel = CalledEveryPixel;
    }

    internal abstract void UpdateSettings(LayerSettings ls, ref LayerMacroSettings ms);

    internal abstract Color PixelDraw(Color initialCol, Color screenPixel, LayerSettings s);
}
