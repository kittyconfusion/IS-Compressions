using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Compressions.code.util;
//This is not a proper huffman implementation with nodes and trees
//It's really just a dictionary wrapper due to small huffman tables.

//Just works with integers
public class HuffmanTable
{
    Dictionary<int, int[]> table = new Dictionary<int, int[]>();
    int[] bits;
    public HuffmanTable(int[] keys, int[] vals, int[] bits)
    {
        int c = 0;
        int blen = 1;
        foreach(int bit in bits)
        {
            for (int i = 0; i < bit; i++)
            {
                table.Add(keys[c], new int[] { vals[c], blen });
                c++;
            }
            blen++;
        }
    }
    //Returns a value with a cooresponding key and length
    public int? GetValue(int K, int len)
    {
        int? value = null;

        int[] outVal;
        
        if (table.TryGetValue(K, out outVal))
        {
            if (outVal[1] == len)
            {
                value = outVal[0];
            }
        }

        return value;
    }

    public override string ToString()
    {
        string outStr = "---Key---Val---Len---\n";
        
        foreach(KeyValuePair<int, int[]> kv in table)
        {

            int k = kv.Key;
            int v = kv.Value[0];
            int l = kv.Value[1];

            outStr += "   ";
            outStr += k;
            outStr += k < 10 ? "  " : (k < 100 ? " " : "");

            outStr += "   ";
            outStr += v;
            outStr += v < 10 ? "  " : (v < 100 ? " " : "");

            outStr += "   ";
            outStr += l;
            outStr += '\n';
        }

        return outStr;
    } 

}
/*
public class HuffmanTable
{
    Dictionary<object, object> d;
    public HuffmanTable(object K, object V)
    {
        d = new Dictionary<object, object>();
        for (int i = 0; i < K.Length; i++)
        {
            d.Add(K[i], V[i]);
        }
    }
    public object GetValue<K>(ref K)
    {
        d.TryGetValue(K, out object value);
        return value;
    }
}
*/