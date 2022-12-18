using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IS_Compressions.code.core;
public class Features
{
    public static int Mod(int a, int b)
    {
        if (b < 0) //you can check for b == 0 separately and do what you want
            return -Mod(-a, -b);
        int ret = a % b;
        if (ret < 0)
            ret += b;
        return ret;
    }
    public static int Mod(long a, int b) {
        return Mod((int)a, b);
    }
}
