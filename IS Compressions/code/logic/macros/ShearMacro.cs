using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.logic.macros;
internal class ShearMacro : PixelMacro
{
    float horizontal;
    float vertical;
    Vector2f initial;
    Vector2f change;
    public ShearMacro(int frames, int repeat, int delay, Vector2f initial, Vector2f change) : base(frames, repeat, delay, false)
    {
        this.initial = initial;
        this.change = change;
        horizontal = initial.X;
        vertical = initial.Y;
    }

    internal override void NextFrame()
    {
        horizontal += change.X;
        vertical += change.Y;
    }
    internal override Color PixelDraw(Color initialCol, Color screenPixel, Layer.LayerSettings s) => throw new NotImplementedException();
    internal override void UpdateSettings(ref Layer.LayerSettings ls, ref Layer.LayerMacroSettings ms)
    {
        ms.HorizontalShear = horizontal;
        ms.HorizontalShear = vertical;
    }
}
