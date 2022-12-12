﻿using System;
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
internal class VerticalShiftMacro : PixelMacro
{
    internal float change;
    internal float shift = 0;

    public VerticalShiftMacro(int frames, int repeat, float changeOnFrame) : base(frames, repeat, false)
    {
        change = changeOnFrame;
    }

    internal override void NextFrame()
    {
        shift += change;
    }

    internal override Color PixelDraw(Color initialCol, Color screenPixel, LayerSettings s) => throw new NotImplementedException();

    internal override void UpdateSettings(LayerSettings ls, ref LayerMacroSettings ms)
    {
        ms.VerticalShift = (int)shift;
    }
}
