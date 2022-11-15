using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace IS_Compressions.code.core;
internal static class TimeKeeper
{
    static readonly double FPSUpdateFreq = 0.1;

    public static Clock clock = new();

    public static double currentTime, lastTime = 0, deltaTime, lastFPSTime = 0, deltaFPSTime;
    public static int fps;
    static int frameCounter = 0;
    
    
    public static void NewFrame()
    {
        currentTime = clock.ElapsedTime.AsSeconds();
        deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        // FPS calculation
        deltaFPSTime = currentTime - lastFPSTime; // In seconds
        frameCounter++;
        if (deltaFPSTime >= FPSUpdateFreq)
        {
            fps = (int)((frameCounter / deltaFPSTime) + 0.5);
            lastFPSTime = currentTime;
            frameCounter = 0;
        }
    }
}
