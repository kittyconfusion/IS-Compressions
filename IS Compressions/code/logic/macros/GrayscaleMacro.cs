using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;

namespace IS_Compressions.code.logic.macros;
internal class GrayscaleMacro : PixelMacro
{
    public GrayscaleMacro(int frames, int repeat, int delay) : base(frames, repeat, delay, true)
    {
    }

    internal override void UpdateSettings(ref Layer.LayerSettings ls, ref Layer.LayerMacroSettings ms) => throw new NotImplementedException();
    internal override Color PixelDraw(Color initialCol, Color screenPixel, Layer.LayerSettings s) {
        byte c = (byte)(int)(0.3f * initialCol.R + 0.59f * initialCol.G + 0.11f * initialCol.B);
        return new Color(c, c, c, initialCol.A);
    }
    internal override void NextFrame() { }
}
