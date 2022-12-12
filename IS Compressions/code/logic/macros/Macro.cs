﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.display;
using SFML.Graphics;
using SFML.System;

namespace IS_Compressions.code.logic.macros;
internal abstract class Macro
{

    internal int frames, currentFrame;
    public bool IsActive = false;
    internal int repeat;

    public Macro(int frames, int repeat)
    {
        this.frames = frames;
        this.repeat = repeat;
        currentFrame = 0;
    }
    internal abstract void NextFrame();
}
