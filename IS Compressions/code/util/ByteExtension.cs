using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Compressions.code.util;
public static class ByteExtension
{
    static string byteStr;
    public static string ToBitsString(this byte[] value)
    {
        byteStr = "";
        foreach (byte b in value)
        {
            byteStr += Convert.ToString(b, 2).PadLeft(8, '0');
        }
        return byteStr;
    }
    public static string ToBitsString(this byte value)
    {
        return Convert.ToString(value, 2).PadLeft(8, '0');
    }
}