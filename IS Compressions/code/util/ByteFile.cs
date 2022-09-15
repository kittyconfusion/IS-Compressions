using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Compressions.code.util;
public class ByteFile : IDisposable
{
    FileStream fs;

    public ByteFile(string path, char type)
    {
        if (type == 'r')
        {
            fs = File.OpenRead(path);
        }
        else if (type == 'w')
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            //var f = File.Create(path);
            //f.Close();
            File.Create(path).Close();
            fs = File.OpenWrite(path);
        }

    }
    public int ReadBytes(ref byte[] b)
    {
        return fs.Read(b, 0, b.Length);
    }
    public int ReadByte()
    {
        return fs.ReadByte();
    }
    public int ReadHexVals(int num = 1)
    {
        int val = 0;
        for (int i = 0; i < num; i++)
        {
            val += (int)(fs.ReadByte() * Math.Pow(256, i));
        }
        return val;
    }
    public void Skip(int a)
    {
        fs.Position += a;
    }
    public void WriteBytes(byte[] b)
    {
        fs.Write(b);
    }
    public void WriteBytes(byte b)
    {
        fs.Write(new byte[] { b });
    }
    public void Dispose()
    {
        if (fs != null)
        {
            fs.Close();
        }
    }
}