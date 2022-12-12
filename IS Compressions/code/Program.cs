using System.ComponentModel;
using IS_Compressions.code.core;
using IS_Compressions.code.display;
using IS_Compressions.code.formats;
using IS_Compressions.code.logic.macros;
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
    int width = 200;
    int height = 150;

    var initialScreenSize = new Vector2i(1366, 768);

    Layer bLayer = new Layer(200, 150, Color.White);

    Display.settings = new DisplaySettings(
        true, // whether to start on the map
        initialScreenSize.X, initialScreenSize.Y, // Screen width and height
        -width / 2, -height / 2, // Starting camera x and ys
        150, // Starting tiles shown
        24.0, 1600, // Min and max tiles shown
        200, 200, 200, // Base text color
        20, 20, 20, // Outline text color
        0, 256 * 1, 256 * 1 // Default display mode
        );

    const int TILE_SIZE = 32;

    /*
    Layer layer1 = new Layer(Display.settings.pixelWidth, Display.settings.pixelHeight);
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

    Layer layer2 = new Layer(width, height);

    
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
    Layer layer3 = new Layer(width, height);
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

    

    DisplayManager dm = new DisplayManager(bLayer, path);
    dm.GetLayers().AddLayer(layer3);
    //dm.GetLayers().AddLayer(layer3);

    */

    Layer b = new Layer(200, 150);
    b.GetSettings().isTiled = true;
    //b.Fill()
    DisplayManager dm = new DisplayManager(b, path);

    Bitmap rb = new Bitmap(@"../../../resources/cityclose.bmp");
    Layer lrb = new Layer(rb.width - 1, rb.height);

    for (int y = 0; y < rb.height; y++)
    {
        for (int x = 0; x < rb.width - 1; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            lrb.GetPixel(x, rb.height - y - 1).SetColor(rb.getColor(x, y));
            //Console.WriteLine(background.getColor(x, y).A);
        }
    }

    lrb.GetSettings().isTiled = true;
    

    Bitmap sc = new Bitmap(@"../../../resources/cat64.bmp");
    Layer lsc = new Layer(sc.width - 1, sc.height);

    for (int y = 0; y < sc.height; y++)
    {
        for (int x = 0; x < sc.width - 1; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            lsc.GetPixel(x, sc.height - y - 1).SetColor(sc.getColor(x, y));
            //Console.WriteLine(background.getColor(x, y).A);
        }
    }
    //lsc.GetSettings().opacity = 0.01f;
    lsc.GetSettings().xOffset = 50;
    lsc.GetSettings().yOffset = 15;
    DisplayManager.layers.AddLayer(lsc);
    DisplayManager.layers.AddLayer(lrb);

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

    //DisplayManager.layers.layers[1].macros.Add(new VerticalShiftMacro(0, 0, 0.5f));
    DisplayManager.layers.layers[1].macros.Add(new ColorAddMacro(0, 0, 2, 0.5f, 1));
    DisplayManager.layers.layers[2].macros.Add(new HorizontalShiftMacro(0, 0, 1f));
    DisplayManager.layers.layers[1].macros.Add(new HorizontalShiftMacro(0, 0, 0.15f));
    DisplayManager.layers.layers[0].macros.Add(new VerticalShiftMacro(0, 0, -0.005f));
    DisplayManager.layers.layers[0].macros.Add(new HorizontalShiftMacro(0, 0, 0.002f));

    // dm.GetLayers().SetLocation(1, 60, 40);
    // dm.GetLayers().GetLayer(2).GetSettings().debug = true;
    //Console.WriteLine(dm.GetLayers().screenColorCache.GetCachedColor(10, 10));
    while (dm.isOpen())
    {
        if (o >= 0.99f) { dir = false; }
        if (o <= 0.01f) { dir = true; }
        if (dir) { o = o + 0.005f; }
        else      { o = o - 0.005f; }

        if (emmaX > 75) { emmaX -= 1; emmaS += 1; }
        if (emmaX < 0) { emmaX += 1; emmaS += 1; }
        if (emmaY > 75) { emmaY -= 1; emmaS += 1; }
        if (emmaY < 0) { emmaY += 1; emmaS += 1; }

        if (emmaS % 4 == 0) { emmaX += 1; }
        if (emmaS % 4 == 2) { emmaX -= 1; }
        if (emmaS % 4 == 1) { emmaY += 1; }
        if (emmaS % 4 == 3) { emmaY -= 1; }


        //dm.GetLayers().SetOpacity(0, o);
        //dm.GetLayers().SetOpacity(1, o);
        //dm.GetLayers().SetOpacity(1, Math.Max(0.5f,o*2));
        //dm.GetLayers().SetLocation(2, (int)-dm.xOffset, (int)-dm.yOffset);
        //dm.GetLayers().SetLocation(1, emmaX, emmaY);
        //dm.GetLayers().SetLocation(1, 60, 40);
        //dm.GetLayers().layers[1].GetSettings().opacity = (dm.GetLayers().GetLayer(1).GetSettings().opacity + 0.01f) % 1;
        //dm.GetLayers().GetLayer(1).GetSettings().yOffset += 1;
        //dm.GetLayers().ResetAlreadyDrawn();
        //Console.WriteLine(dm.getTileCoordsFromScreenCoords());
        //Console.WriteLine(dm.GetLayers().GetLayer(1).GetSettings().opacity);

        TimeKeeper.NewFrame();

        window.DispatchEvents();

        DisplayManager.layers.NextFrameMacroUpdate();

        DisplayManager.layers.ResetAlreadyDrawn();
        
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