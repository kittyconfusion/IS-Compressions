using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.util;
using SFML.Graphics;

namespace IS_Compressions.code.formats;
public class JPEG
{
    public ByteFile bf;
    internal HuffmanTable InitHuffmanTable()
    {
        int[] bits = new int[16];

        for (int i = 0; i < 16; i++)
        {
           bits[i] = (int)bf.ReadByte();
        }
        int[] huffCodes = new int[bits.Sum()];
        int[] huffvals = new int[bits.Sum()];

        int c = 0;
        for (int i = 0; i < 16; i++)
        {
            int pow = (int)Math.Pow(2, i);
            for(int j = 0; j < bits[i]; j++)
            {
                huffCodes[c] = j + pow;
                huffvals[c] = (int)bf.ReadByte();
                c++;
            }
            
        }
        HuffmanTable ht = new HuffmanTable(huffCodes,huffvals,bits);
        return ht;
    }

}