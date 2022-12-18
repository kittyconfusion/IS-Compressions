using System;
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
    public bool IsActive;
    internal int repeat;
    internal int delay;

    public Macro(int frames, int repeat, int delay)
    {
        this.frames = frames;
        this.repeat = repeat;
        this.delay = delay;
        currentFrame = 0;
        if(delay == 0)
        {
            IsActive = true;
        }
    }
    internal void Next()
    {
        if(delay < 1)
        {
            if (currentFrame > frames && repeat > 0)
            {
                currentFrame = 0;
                repeat--;
                if (repeat == 0)
                {
                    IsActive = false;
                }
            }
            if(IsActive)
            {
                NextFrame();
                currentFrame++;
            }
        }
        else
        {
            delay--;
            if(delay == 0)
            {
                IsActive = true;
            }
        }
    }
    internal abstract void NextFrame();
}
