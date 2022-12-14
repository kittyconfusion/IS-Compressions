using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.display;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.logic.macros;
internal class ColorAddMacro : PixelMacro
{

    internal FloatColor colAdd;
    internal FloatColor colChange;

    public ColorAddMacro(int frames, int repeat, int delay, FloatColor add) : base(frames, repeat, delay, true)
    {
        colChange = new FloatColor();
        colAdd = add;
    }

    internal override void NextFrame()
    {
        colChange += colAdd;
        colChange %= 255;
    }

    internal override Color PixelDraw(Color initialCol, Color screenPixel, Layer.LayerSettings s)
    {
        FloatColor pc = new FloatColor(initialCol);
        pc += colChange;
        pc %= 255;
        return new Color((byte)pc.r,(byte)pc.g,(byte)pc.b,initialCol.A);
    }
    internal override void UpdateSettings(ref Layer.LayerSettings ls, ref Layer.LayerMacroSettings ms) => throw new NotImplementedException();
}
