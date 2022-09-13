using IS_Compressions.code;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static IS_Compressions.code.DisplayManager;

string inPath = @"../../../resources/apollosmall256.bmp";
string outPath = @"../../../resources/out.cop";

string bitPath = @"../../../resources/apollo24bmid.bmp";

string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
string path = string.Format("{0}Resources\\", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\..\")));

Console.WriteLine("Starting");
main();

//RunLength.Encode(inPath, outPath);
//RunLength.Decode(outPath, outttPath);

void main()
{
    Bitmap bmp = new Bitmap(bitPath);
    int width = bmp.width;
    int height = bmp.height;

    var initialScreenSize = new Vector2i(1366, 768);
    
    DisplaySettings ds = new DisplaySettings(
        true, // whether to start on the map
        initialScreenSize.X, initialScreenSize.Y, // Screen width and height
        0, 0, // Starting camera x and ys
        150, // Starting tiles shown
        24.0, 1600, // Min and max tiles shown
        200, 200, 200, // Base text color
        20, 20, 20, // Outline text color
        0, width * 1, height * 1 // Default display mode
        );

    TileMap tileMap = new TileMap(ds.pixelWidth, ds.pixelHeight);

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            tileMap.getTile(x, height - y - 1).setColor(bmp.getColor(x,y));
        }

    }

    Clock clock = new Clock();

    DisplayManager dm = new DisplayManager(ds, tileMap, path);
    dm.GetWindow().SetFramerateLimit(30);
    //dm.GetWindow().SetVerticalSyncEnabled(true);
    dm.SetClock(clock);

    var window = dm.GetWindow();

    window.Closed += dm.OnClose;
    window.Resized += dm.resize;
    window.MouseWheelScrolled += dm.OnMouseScroll;
    window.MouseButtonPressed += dm.OnMousePress;
    window.MouseButtonReleased += dm.OnMouseRelease;
    window.MouseMoved += dm.OnMouseMove;
    window.KeyPressed += dm.OnKeyPress;
    window.KeyReleased += dm.OnKeyRelease;

    double currentTime, lastTime = 0, deltaTime, lastFPSTime = 0, deltaFPSTime;
    double FPSUpdateFreq = 0.1; // How often to update the FPS display (in seconds)
    int frameCounter = 0;

    dm.drawTiles();

    while (dm.isOpen())
    {
        window.DispatchEvents();
        dm.Move();

        dm.display();

        // frame-locked actions

        currentTime = clock.ElapsedTime.AsSeconds();
        deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        dm.deltaTime = deltaTime;

        // FPS calculation
        deltaFPSTime = currentTime - lastFPSTime; // In seconds
        frameCounter++;
        if (deltaFPSTime >= FPSUpdateFreq)
        {
            double fps = frameCounter / deltaFPSTime;
            dm.fps = ((int)(fps));
            lastFPSTime = currentTime;
            frameCounter = 0;
        }
    }

}

public class Bitmap
{
    Color[] colorData;
    public readonly int width;
    public readonly int height;
    ByteFile f;

    public Color getColor(int x, int y)
    {
        return colorData[(y * width) + x];
    }
    public Bitmap(string file)
    {
        using (f = new ByteFile(file, 'r'))
        {
            f.Skip(10);
            //Used to skip rest of metadata
            int imageStartOffset = f.ReadHexVals(4) - 0x17;
            
            f.Skip(4);
            width = f.ReadHexVals(4);
            height = f.ReadHexVals(4);
            f.Skip(imageStartOffset);

            Console.WriteLine(width + " x " + height);
            colorData = new Color[width * height];
            
            //Color data stored in blue, green, red order
            byte[] bgr = new byte[3];
            for (int y = 0; y < height; y++)
            {
                //Console.WriteLine(y);
                for (int x = 0; x < width; x++)
                {
                    f.ReadBytes(ref bgr);

                    colorData[(y * width) + x] = new Color(bgr[2], bgr[1], bgr[0]);
                }
                //Calculate padding at the end of every image row
                f.Skip(width % 4);
                //f.Skip(2);
            }
        }
    }
}
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