using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;

namespace IS_Compressions.code.logic.macros;
internal class VisibilityMacro : PixelMacro
{
    bool visible;
    public VisibilityMacro(int delay, bool visible) : base(10, 1, delay, false)
    {
        this.visible = visible;
    }

    internal override void NextFrame() {}
    internal override Color PixelDraw(Color initialCol, Color screenPixel, Layer.LayerSettings s) => throw new NotImplementedException();
    internal override void UpdateSettings(ref Layer.LayerSettings ls, ref Layer.LayerMacroSettings ms)
    {
        ls.visible = visible;
    }
}
