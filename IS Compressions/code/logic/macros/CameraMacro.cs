using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace IS_Compressions.code.logic.macros;
internal class CameraMacro : Macro
{
    Vector2i start;
    Vector2i end;
    public CameraMacro(int numberOfFrames, int repeat, Vector2i start, Vector2i end) : base(numberOfFrames, repeat)
    {
        this.start = start;
        this.end = end;
    }
}
