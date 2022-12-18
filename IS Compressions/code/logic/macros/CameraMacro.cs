using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.core;
using IS_Compressions.code.display;
using SFML.System;

namespace IS_Compressions.code.logic.macros;
internal class CameraMacro : Macro
{
    Vector2i start;
    Vector2i end;
    Vector2f change;
    public CameraMacro(int numberOfFrames, int repeat, int delay, Vector2i start, Vector2i end) : base(numberOfFrames, repeat, delay)
    {
        this.start = start;
        this.end = end;
        change = new Vector2f((float)(end.X - start.X) / numberOfFrames, (float)(end.Y - start.Y) / numberOfFrames);
    }

    internal override void NextFrame()
    {
        Camera.xOffset = -(start.X + (change.X * currentFrame));
        Camera.yOffset = -(start.Y + (change.Y * currentFrame));
    }
}
