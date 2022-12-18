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
internal class HorizontalShiftMacro : PixelMacro
{
    internal float change;
    internal float shift = 0;

    public HorizontalShiftMacro(int frames, int repeat, int delay, float changeOnFrame) : base(frames, repeat, delay, false)
    {
        change = changeOnFrame;
    }

    internal override void NextFrame()
    {
        shift += change;
    }

    internal override Color PixelDraw(Color initialCol, Color screenPixel, LayerSettings s) => throw new NotImplementedException();

    internal override void UpdateSettings(ref LayerSettings ls, ref LayerMacroSettings ms)
    {
        ms.HorizontalShift = (int)shift;
    }
}
