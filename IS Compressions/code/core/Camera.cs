using SFML.System;

namespace IS_Compressions.code.core;
internal static class Camera
{
    internal static float xOffset, yOffset;
    internal static float scale = 3;
    internal static double cameraSpeed;//, effectiveCameraSpeed;
    public static Vector2f GetOffset()
    {
        return new Vector2f(xOffset, yOffset);
    }
    public static void MoveCamera(float x, float y)
    {
        xOffset += x / scale;
        yOffset += y / scale;
    }
    public static void ChangeScale(double delta)
    {
        scale = (float)Math.Min(Math.Max(scale + delta, 4), 50);
    }
}
