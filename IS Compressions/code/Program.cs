using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using IS_Compressions.code.display;
using IS_Compressions.code.formats;
using IS_Compressions.code.util;
using Map_Generator_CSharp.Source.tiles;
using SFML.System;
using static IS_Compressions.code.display.DisplayManager;

string inPath = @"../../../resources/apollosmall256.bmp";
string outPath = @"../../../resources/out.cop";

string bitPath = @"../../../resources/apollo24bmid.bmp";

string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
string path = string.Format("{0}Resources\\", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\..\")));

Console.WriteLine("Starting");
main();

//RunLength.Encode(inPath, outPath);
//RunLength.Decode(outPath, outPath2);

void main()
{
    runBitmap();
}


void runBitmap ()
{
    Bitmap bmp = new Bitmap(bitPath);
    int width = bmp.width - 1;
    int height = bmp.height;

    var initialScreenSize = new Vector2i(1366, 768);

    DisplaySettings ds = new DisplaySettings(
        true, // whether to start on the map
        initialScreenSize.X, initialScreenSize.Y, // Screen width and height
        -width / 2, -height / 2, // Starting camera x and ys
        150, // Starting tiles shown
        24.0, 1600, // Min and max tiles shown
        200, 200, 200, // Base text color
        20, 20, 20, // Outline text color
        0, width * 1, height * 1 // Default display mode
        );

    Layer tileMap = new Layer(ds.pixelWidth, ds.pixelHeight);

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            tileMap.getTile(x, height - y - 1).setColor(bmp.getColor(x, y));
        }

    }

    Clock clock = new Clock();

    DisplayManager dm = new DisplayManager(ds, tileMap, path);
    dm.GetWindow().SetFramerateLimit(60);
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
            dm.fps = ((int)(fps + 0.5));
            lastFPSTime = currentTime;
            frameCounter = 0;
        }
    }
}


/*
    //Test the huffman decoding

    JPEG j = new JPEG();

    HuffmanTable h;

    string file = @"../../../tests/HuffmanTest";
    using (var f = new ByteFile(file, 'r',false))
    {
        j.bf = f;
        h = j.InitHuffmanTable();
    }
    Console.WriteLine(h);
*/