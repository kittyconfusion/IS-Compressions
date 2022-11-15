using IS_Compressions.code.core;
using IS_Compressions.code.display;
using IS_Compressions.code.formats;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static IS_Compressions.code.core.Display;

string inPath = @"../../../resources/apollosmall256.bmp";
string outPath = @"../../../resources/out.cop";

string bitPath = @"../../../resources/apollo24bsma.bmp";
string secondLayerPath = @"../../../resources/graycatsmall.bmp";
string thirdLayerPath = @"../../../resources/catpumpkin.bmp";

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
    Bitmap background = new Bitmap(bitPath);
    int width = background.width - 1;
    int height = background.height;

    var initialScreenSize = new Vector2i(1366, 768);

    Display.settings = new DisplaySettings(
        true, // whether to start on the map
        initialScreenSize.X, initialScreenSize.Y, // Screen width and height
        -width / 2, -height / 2, // Starting camera x and ys
        150, // Starting tiles shown
        24.0, 1600, // Min and max tiles shown
        200, 200, 200, // Base text color
        20, 20, 20, // Outline text color
        0, width * 1, height * 1 // Default display mode
        );

    const int TILE_SIZE = 32;

    Layer layer1 = new Layer(Display.settings.pixelWidth, Display.settings.pixelHeight, TILE_SIZE);
    layer1.GetSettings().opacity = 1.0f;
    
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            layer1.GetPixel(x, height - y - 1).SetColor(background.getColor(x, y));
            //Console.WriteLine(background.getColor(x, y).A);
        }

    }


    Bitmap top = new Bitmap(secondLayerPath);
    width = top.width - 1;
    height = top.height;

    Layer layer2 = new Layer(width, height, TILE_SIZE);

    
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            Color c = top.getColor(x, y);
            if (c.R > 200) { c.A = 0; }
            
            //Draw the pixels bottom to top, as is in the format
            layer2.GetPixel(x, height - y - 1).SetColor(new Color(c));
        }

    }

    Bitmap emma = new Bitmap(thirdLayerPath);
    width = emma.width - 1;
    height = emma.height;
    Layer layer3 = new Layer(width, height, TILE_SIZE);
    layer3.GetSettings().opacity = 1.0f;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            layer3.GetPixel(x, height - y - 1).SetColor(emma.getColor(x, y));
            //Console.WriteLine(background.getColor(x, y).A);
        }

    }


    //Clock clock = new Clock();

    DisplayManager dm = new DisplayManager(layer1, path);
    //dm.GetLayers().AddLayer(layer2);
    //dm.GetLayers().AddLayer(layer3);

    dm.GetWindow().SetFramerateLimit(30);
    //dm.GetWindow().SetVerticalSyncEnabled(true);

    var window = dm.GetWindow();

    window.Closed += dm.OnClose;
    window.Resized += dm.WindowResize;
    window.MouseWheelScrolled += dm.OnMouseScroll;
    window.MouseButtonPressed += dm.OnMousePress;
    window.MouseButtonReleased += dm.OnMouseRelease;
    window.MouseMoved += dm.OnMouseMove;
    window.KeyPressed += dm.OnKeyPress;
    window.KeyReleased += dm.OnKeyRelease;

    double currentTime, lastTime = 0, deltaTime, lastFPSTime = 0, deltaFPSTime;
    double FPSUpdateFreq = 0.1; // How often to update the FPS display (in seconds)
    int frameCounter = 0;

    dm.Display();

    var o = 0f;
    bool dir = true;

    int emmaX = 10;
    int emmaY = 20;
    int emmaS = 0;
    // dm.GetLayers().SetLocation(1, 60, 40);
    // dm.GetLayers().GetLayer(2).GetSettings().debug = true;
    //Console.WriteLine(dm.GetLayers().screenColorCache.GetCachedColor(10, 10));
    while (dm.isOpen())
    {
        if (o >= 0.99f) { dir = false; }
        if (o <= 0.01f) { dir = true; }
        if (dir) { o = o + 0.005f; }
        else      { o = o - 0.005f; }

        if (emmaX > 100) { emmaX -= 1; emmaS += 1; }
        if (emmaX < 0) { emmaX += 1; emmaS += 1; }
        if (emmaY > 100) { emmaY -= 1; emmaS += 1; }
        if (emmaY < 0) { emmaY += 1; emmaS += 1; }

        if (emmaS % 4 == 0) { emmaX += 1; }
        if (emmaS % 4 == 2) { emmaX -= 1; }
        if (emmaS % 4 == 1) { emmaY += 1; }
        if (emmaS % 4 == 3) { emmaY -= 1; }


        //dm.GetLayers().SetOpacity(1, o);
        //dm.GetLayers().SetOpacity(2, Math.Max(0.5f,o*2));
        //dm.GetLayers().SetLocation(2, (int)-dm.xOffset, (int)-dm.yOffset);
        //dm.GetLayers().SetLocation(2, emmaX, emmaY);
        //dm.GetLayers().SetLocation(1, 60, 40);
        //dm.GetLayers().layers[1].GetSettings().opacity = (dm.GetLayers().GetLayer(1).GetSettings().opacity + 0.01f) % 1;
        //dm.GetLayers().GetLayer(1).GetSettings().yOffset += 1;
        //dm.GetLayers().ResetAlreadyDrawn();
        //Console.WriteLine(dm.getTileCoordsFromScreenCoords());
        //Console.WriteLine(dm.GetLayers().GetLayer(1).GetSettings().opacity);

        TimeKeeper.NewFrame();

        window.DispatchEvents();

        dm.Display();

        
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