using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;

namespace IS_Compressions.code.logic.macros;
internal class TransparencyMacro : PixelMacro
{
    float start;
    float end;
    float change;
    public TransparencyMacro(int frames, int repeat, int delay, float start, float end) : base(frames, repeat, delay, false)
    {
        change = (float)Math.Round((end - start) / frames, 4);
    }

    internal override void NextFrame() {}
    internal override Color PixelDraw(Color initialCol, Color screenPixel, Layer.LayerSettings s) => throw new NotImplementedException();
    internal override void UpdateSettings(ref Layer.LayerSettings ls, ref Layer.LayerMacroSettings ms)
    {
        if(currentFrame > frames)
        {
            ms.opacity = end;
        }
        else
        {
            ms.opacity = (start + (change * currentFrame));
        }
        //Console.WriteLine(ms.opacity + " " + end + " " + currentFrame + " " + frames);
        //Console.WriteLine(ms.opacity);
    }
}
