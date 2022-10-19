using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace IS_Compressions.code.util;

public struct YCbCr
{
    public readonly double Y;
    public readonly double Cb;
    public readonly double Cr;

    public YCbCr(double y, double cb, double cr)
    {
        Y = y;
        Cb = cb;
        Cr = cr;
    }
    public override string ToString()
    {
        return '(' + ((int)Y).ToString() + ',' + ((int)Cb).ToString() + ',' + ((int)Cr).ToString() + ')';
    }
    public static YCbCr RGBtoYCbCr(Color c)
    {
        double Y = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
        double Cb = -0.168736 * c.R - 0.331264 * c.G + 0.5 * c.B;
        double Cr = 0.5 * c.R - 0.418688 * c.G - 0.081312 * c.B;
        return new YCbCr(Y, Cb, Cr);
    }
    public static Color YCbCrtoRGB(YCbCr c)
    {
        byte R = (byte)(c.Y + 1.402 * c.Cr);
        byte G = (byte)(c.Y - 0.3441 * c.Cb - 0.71414 * c.Cr);
        byte B = (byte)(c.Y + 1.772 * c.Cb);
        return new Color(R, G, B);
    }

}