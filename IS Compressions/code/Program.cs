using System.ComponentModel;
using IS_Compressions.code.core;
using IS_Compressions.code.display;
using IS_Compressions.code.formats;
using IS_Compressions.code.logic.macros;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using static IS_Compressions.code.core.Display;
using static IS_Compressions.code.logic.macros.PixelMacro;
using fColor = IS_Compressions.code.logic.macros.PixelMacro.FloatColor;

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
    //runBitmap();
    runDemo();    
}

void runDemo()
{
    var initialScreenSize = new Vector2i(1366, 768);
    int width = 200;
    int height = 150;

    Display.settings = new DisplaySettings(
    initialScreenSize.X, initialScreenSize.Y, // Screen width and height
    -width / 2, -height / 2, // Starting camera x and ys
    150, // Starting tiles shown
    0, 0, // Min and max tiles shown
    200, 200, 200, // Base text color
    20, 20, 20, // Outline text color
    0, 256 * 1, 256 * 1 // Default display mode
    );
    Pixel.starThreshold = 0.97;
    Layer b = new Layer(250, 200);
    DisplayManager dm = new DisplayManager(b, path);

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

    //b.GetSettings().isTiled = true;
    b.macros.Add(new HorizontalShiftMacro(0, 0, 0, -1));
    b.macros.Add(new VerticalShiftMacro(0, 0, 0, 0.5f));
    b.macros.Add(new GrayscaleMacro(0, 0, 0));


    Pixel.starThreshold = 0.9875;
    Layer colorstars = new Layer(b.GetWidth(), b.GetHeight());
    colorstars.SetTransparent(Color.Black);
    colorstars.macros.Add(new HorizontalShiftMacro(0, 0, 0, -1));
    colorstars.macros.Add(new VerticalShiftMacro(0, 0, 0, 0.5f));
    colorstars.GetSettings().visible = false;

    Layer rain = MakeLayer("rain_drops.bmp");
    rain.GetSettings().isTiled = true;
    rain.GetSettings().opacity = 0.16f;
    rain.macros.Add(new ShearMacro(0, 0, 0, new Vector2f(2f, 3f), new Vector2f(1f,0.12f)));
    rain.macros.Add(new HorizontalShiftMacro(0, 0, 0, 3f));
    rain.macros.Add(new VerticalShiftMacro(0, 0, 0, -2f));

    
    Layer rain2 = MakeLayer("rain_drops.bmp");
    rain2.GetSettings().isTiled = true;
    rain2.GetSettings().opacity = 0.22f;
    rain2.macros.Add(new HorizontalShiftMacro(0, 0, 0, 1f));
    rain2.macros.Add(new VerticalShiftMacro(0, 0, 0, -2.5f));
    rain2.macros.Add(new ShearMacro(0, 0, 0, new Vector2f(0.1f, 2f), new Vector2f(0.11f, 0)));

    Layer clouds = MakeLayer("clouds180.bmp");
    clouds.GetSettings().opacity = 0.96f;
    clouds.GetSettings().yOffset = 320;
    clouds.GetSettings().isTiled = true;
    //clouds.GetSettings().visible = false;
    // clouds.macros.Add(new VerticalShiftMacro(0, 0, 0, 0.5f));

    Layer clouds2 = MakeLayer("clouds.bmp");
    clouds2.GetSettings().isTiled = true;
    clouds2.GetSettings().opacity = 0.96f;

    Layer green = MakeLayer("lightgreen.bmp");
    green.GetSettings().isTiled = true;
    green.GetSettings().opacity = 0f;

    Layer cat = MakeLayer("cat64.bmp");
    cat.GetSettings().xOffset = width / 2;
    cat.GetSettings().yOffset = height / 2;
    cat.GetSettings().opacity = 0f;

    DisplayManager.layers.AddLayer(colorstars);
    DisplayManager.layers.AddLayer(rain2);
    DisplayManager.layers.AddLayer(clouds);
    DisplayManager.layers.AddLayer(clouds2);
    //DisplayManager.layers.AddLayer(green);
    DisplayManager.layers.AddLayer(rain);
    DisplayManager.layers.AddLayer(cat);



    clouds.macros.Add(new VerticalShiftMacro(520, 1, 150, -1));
    clouds.macros.Add(new HorizontalShiftMacro(0, 0, 150, 0.75f));

    clouds2.macros.Add(new VerticalShiftMacro(520, 1, 470, -1));
    clouds2.macros.Add(new HorizontalShiftMacro(0, 0, 470, 0.40f));
    //clouds.macros.Add(new TransparencyMacro(1, 0, 330 + 140, 1, 0));

    //rain.macros.Add(new TransparencyMacro(200, 1, 330 + 140, 1.95f, 1.75f));
    //green.macros.Add(new TransparencyMacro(1, 0, 330 + 140 + 10, 0, 0.70f));

    rain2.macros.Add(new VisibilityMacro(500, false));
    //rain.macros.Add(new VisibilityMacro(600, false));
    clouds.macros.Add(new VisibilityMacro(1000, false));
    clouds2.macros.Add(new VisibilityMacro(1000, false));

    colorstars.macros.Add(new ColorAddMacro(0, 0, 0, new fColor(0.2f, 0.4f, 0.6f)));
    //colorstars.macros.Add(new VisibilityMacro(10, false));
    //colorstars.macros.Add(new VisibilityMacro(40, true));

    var c = 0;
    var o = 0.0f;
    while (dm.isOpen())
    {
        if(c > 600)
        {
            colorstars.GetSettings().visible = true;
            
        }
        if(c > 730)
        {
            if(c < 900)
            {
                o += 0.004f;
            }
            cat.GetSettings().opacity = o;
        }
        c++;
        //Console.WriteLine(rain2.GetSettings().visible);
        //Console.WriteLine(clouds.GetHeight() + clouds.GetSettings().yOffset);
        TimeKeeper.NewFrame();

        window.DispatchEvents();

        dm.NextFrameMacroUpdate();

        DisplayManager.layers.ResetAlreadyDrawn();

        dm.Display();
        //var effectiveOpacity = Math.Max(Math.Min(green.GetSettings().opacity + green.GetMacroSettings().opacity, 1), 0);
        //Console.Write(effectiveOpacity);

    }
}

void runBitmap ()
{
    Bitmap background = new Bitmap(bitPath);
    int width = 200;
    int height = 150;

    var initialScreenSize = new Vector2i(1366, 768);

    Layer bLayer = new Layer(200, 150, Color.White);

    Display.settings = new DisplaySettings(
        initialScreenSize.X, initialScreenSize.Y, // Screen width and height
        -width / 2, -height / 2, // Starting camera x and ys
        150, // Starting tiles shown
        0, 0, // Min and max tiles shown
        200, 200, 200, // Base text color
        20, 20, 20, // Outline text color
        0, 256 * 1, 256 * 1 // Default display mode
        );


    Layer b = new Layer(200, 150);
    b.GetSettings().isTiled = true;
    //b.Fill()
    DisplayManager dm = new DisplayManager(b, path);

    Layer lrb = MakeLayer("cityclose.bmp");

    lrb.GetSettings().isTiled = true;


    Layer lsc = MakeLayer("cat64.bmp");

    //lsc.GetSettings().isTiled = true;
    lsc.GetSettings().opacity = 0.5f;
    lsc.GetSettings().xOffset = 50;
    lsc.GetSettings().yOffset = 18;

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

    bool dir = true;


    
    DisplayManager.layers.layers[0].macros.Add(new ColorAddMacro(0, 0, 0, new fColor(0.098f, 0.016f, 0.008f)));
    //DisplayManager.layers.layers[1].macros.Add(new VerticalShiftMacro(0, 0, 0.5f));
    //DisplayManager.layers.layers[1].macros.Add(new ColorAddMacro(0, 0, 2, 0.5f, 1));
    DisplayManager.layers.layers[2].macros.Add(new HorizontalShiftMacro(0, 0, 0, 1f));
    DisplayManager.layers.layers[1].macros.Add(new HorizontalShiftMacro(0, 0, 0, 0.15f));
    DisplayManager.layers.layers[0].macros.Add(new GrayscaleMacro(0, 0, 200));
    //DisplayManager.layers.layers[0].macros.Add(new VerticalShiftMacro(0, 0, -0.005f));
    //DisplayManager.layers.layers[0].macros.Add(new HorizontalShiftMacro(0, 0, 0.002f));
    DisplayManager.layers.layers[0].macros.Add(new VerticalShiftMacro(0, 0, 0 , 1f));
    DisplayManager.layers.layers[0].macros.Add(new HorizontalShiftMacro(0, 0, 0, 1.5f));
    //DisplayManager.layers.layers[1].macros.Add(new ShearMacro(0, 0, 500, new Vector2f(9990,-9880), new Vector2f(0.1f, -0.13f)));
    //DisplayManager.macros.Add(new CameraMacro(100, 5, 50, new Vector2i(0, 0), new Vector2i(250, 120)));
    DisplayManager.layers.layers[0].macros.Add(new TransparencyMacro(150, 0, 0, 0f, 1f));

    // dm.GetLayers().SetLocation(1, 60, 40);
    // dm.GetLayers().GetLayer(2).GetSettings().debug = true;
    //Console.WriteLine(dm.GetLayers().screenColorCache.GetCachedColor(10, 10));
    while (dm.isOpen())
    {

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

        dm.NextFrameMacroUpdate();

        DisplayManager.layers.ResetAlreadyDrawn();
        
        dm.Display();

        
    }
}

Layer MakeLayer(string path)
{
    Bitmap rb = new Bitmap(@"../../../resources/" + path);
    Layer lrb = new Layer(rb.width - 1, rb.height);

    for (int y = 0; y < rb.height; y++)
    {
        for (int x = 0; x < rb.width - 1; x++)
        {
            //Draw the pixels bottom to top, as is in the format
            lrb.GetPixel(x, rb.height - y - 1).SetColor(rb.getColor(x, y));
        }
    }
    return lrb;
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