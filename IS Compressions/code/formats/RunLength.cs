using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.util;

namespace IS_Compressions.code.formats;
public class RunLength
{
    const int BUFFER_SIZE = 256;
    const byte ESCAPE_CHAR = 0xFE;
    public static void Encode(string inPath, string outPath)
    {
        //using (FileStream fs = File.OpenRead(path))
        using (ByteFile inFile = new ByteFile(inPath, 'r'),
                        outFile = new ByteFile(outPath, 'w'))
        {
            byte[] bBuffer = new byte[BUFFER_SIZE];

            while (inFile.ReadBytes(ref bBuffer) > 0)
            {
                byte currentByte = bBuffer[0];
                int c = -1;
                foreach (byte b in bBuffer)
                {
                    if (b == currentByte)
                    {
                        c++;
                    }
                    else
                    {
                        if (currentByte == ESCAPE_CHAR || c > 1)
                        {
                            outFile.WriteBytes(new byte[] { 0xFE, (byte)c, currentByte });
                        }
                        else if (c == 1)
                        {
                            outFile.WriteBytes(new byte[] { currentByte, currentByte });
                        }
                        else
                        {
                            outFile.WriteBytes(currentByte);
                        }

                        currentByte = b;
                        c = 0;
                    }
                }
                outFile.WriteBytes(new byte[] { 0xFE, (byte)c, currentByte });
            }
        }
    }
    public static void Decode(string inPath, string outPath)
    {
        //using (FileStream fs = File.OpenRead(path))
        using (ByteFile inFile = new ByteFile(inPath, 'r'),
                        outFile = new ByteFile(outPath, 'w'))
        {
            byte[] bBuffer = new byte[BUFFER_SIZE];

            bool lastByteEscape = false;
            int numRept = -1;

            while (inFile.ReadBytes(ref bBuffer) > 0)
            {
                byte currentByte = bBuffer[0];

                foreach (byte b in bBuffer)
                {
                    if (lastByteEscape && numRept != -1)
                    {
                        for (int i = 0; i <= numRept; i++)
                        {
                            outFile.WriteBytes(b);
                        }
                        lastByteEscape = false;
                        numRept = -1;
                    }
                    else if (lastByteEscape)
                    {
                        numRept = b;
                    }
                    else if (b == ESCAPE_CHAR)
                    {
                        lastByteEscape = true;
                    }
                    else
                    {
                        outFile.WriteBytes(b);
                    }
                }
            }
        }
    }
}
