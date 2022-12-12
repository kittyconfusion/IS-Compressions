using IS_Compressions.code.core;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;
using Image = SFML.Graphics.Image;
using Disp = IS_Compressions.code.core.Display;
using IS_Compressions.code.logic.tools;
using IS_Compressions.code.logic.macros;

namespace IS_Compressions.code.display;
class DisplayManager
{
    private RenderWindow window;
    private View view;
    private Font font;


    internal static OverlayLayer overlay;
    internal static LayerHolder layers;
    private Sprite renderTextureSprite = new Sprite();
    private Sprite overlayTextureSprite = new Sprite();

    internal List<Tool> tools = new List<Tool> {new CameraTool(), new MoveTool()};

    bool alt = false;

    private string @resourceDir;

    private void Draw()
    {
        // clear map rendertexture
        //window.Clear(new Color(150, 150, 150));


        layers.Draw();
        overlay.Draw();

        // pass rendertexture to window

        renderTextureSprite.Texture = layers.screenRenderTexture.Texture;
        //renderTextureSprite.Scale = ;
        renderTextureSprite.Position = new Vector2f(Camera.xOffset * Camera.scale + getWindowWidth() / 2, Camera.yOffset * Camera.scale + getWindowHeight() / 2);
        renderTextureSprite.Scale = new Vector2f(Camera.scale, Camera.scale);
        window.Draw(renderTextureSprite);
        
        overlayTextureSprite.Texture = overlay.overlayRenderTexture.Texture;
        overlayTextureSprite.TextureRect = new IntRect(0, (int)getWindowHeight(), (int)getWindowWidth(), (int)-getWindowHeight());
        //overlayTextureSprite.Scale = new Vector2f(1,1);
        window.Draw(overlayTextureSprite);

        //layers.GetLayer(1).RenderNeededTilesOnScreen((int)xOffset, (int)yOffset);
    }

    public DisplayManager(Layer backLayer, string rDir)
    {
        
        window = new RenderWindow(new VideoMode((uint)Disp.settings.startScreenWidth, (uint)Disp.settings.startScreenHeight), "Cat Viewer Deluxe Extreme Edition ++#");

        view = new View();
        view.Center = new Vector2f(Disp.settings.startScreenWidth / 2, Disp.settings.startScreenHeight / 2);
        view.Size = new Vector2f(Disp.settings.startScreenWidth, Disp.settings.startScreenHeight);

        window.SetView(view);

        resourceDir = rDir;

        Console.WriteLine("Using resource directory " + resourceDir);

        Camera.xOffset = Disp.settings.initialXOffset;
        Camera.yOffset = Disp.settings.initialYOffset;
        //tileSize = (float)(Math.Max(settings.screenWidth, settings.screenHeight) / settings.initialTilesShown);

        loadFont();
        loadIcon();

        layers = new LayerHolder(backLayer,window);
        overlay = new OverlayLayer(ref window, ref font, ref layers, ref layers.screenColorCache);

        // How many seconds does it take to move across one screen with the camera?
        double cameraSecondsPerScreen = 2;
        // Camera speed in tiles per second
        Camera.cameraSpeed = Disp.settings.initialTilesShown / cameraSecondsPerScreen;

        layers.ResetAlreadyDrawn();
    }
    public RenderWindow GetWindow()
    {
        return window;
    }

    private string mergePaths(string pathA, string pathB)
    {
        return @pathA + @pathB;
    }
    private string getResourcePath(string resource)
    {
        return mergePaths(resourceDir, resource);
    }

    private void loadFont()
    {
        font = new Font(getResourcePath("font.ttf"));
    }

    private void loadIcon()
    {
        var icon = new Image(getResourcePath("icon.png"));

        if (!File.Exists(getResourcePath("icon.png")))
        {
            throw new FileNotFoundException("Resource not found: " + getResourcePath("icon.png"));
        }

        window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);
    }
    public void WindowResize(object sender, SizeEventArgs e)
    {
        Disp.settings.startScreenWidth = (int)e.Width;
        Disp.settings.startScreenHeight = (int)e.Height;
        Camera.ChangeScale(0); // Make sure tilesize is within bounds

        view.Center = new Vector2f(Disp.settings.startScreenWidth / 2, Disp.settings.startScreenHeight / 2);
        view.Size = new Vector2f(Disp.settings.startScreenWidth, Disp.settings.startScreenHeight);

        window.SetView(view);

        //mapRenderTexture = new RenderTexture((uint)Disp.settings.startScreenWidth, (uint)Disp.settings.startScreenHeight);
        overlay.Resize((uint)Disp.settings.startScreenWidth, (uint)Disp.settings.startScreenHeight);
        //layers.Resize((uint)Disp.settings.startScreenWidth, (uint)Disp.settings.startScreenHeight);
    }
    public void Display()
    {
        tools[Disp.currentToolIndex].Update();
        window.Clear();
        Draw();
        window.Display();
    }
    public bool isOpen()
    {
        return window.IsOpen;
    }
    public void OnClose(object sender, EventArgs e)
    {
        window.Close();
    }
    public uint getWindowWidth()
    {
        return window.Size.X;
    }
    public uint getWindowHeight()
    {
        return window.Size.Y;
    }

    public Vector2f GetTopLeftPos()
    {
        var centerX = Camera.xOffset * Camera.scale + getWindowWidth() / 2;
        var centerY = Camera.yOffset * Camera.scale + getWindowHeight() / 2;

        //var centerX = xOffset / scale;
        //var centerY = yOffset / sacle;// + getWindowHeight() / 2;

        return new Vector2f((float)centerX, (float)centerY);
    }

    internal void OnKeyRelease(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.LAlt || e.Code == Keyboard.Key.RAlt)
        {
            alt = false;
        }
        tools[Disp.currentToolIndex].OnKeyRelease(sender, e);
    }
    internal void OnKeyPress(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Space)
        {
            layers.ResetAlreadyDrawn();
            //layers.macros[0].NextFrame();
        }
        if (e.Code > Keyboard.Key.Num0 && e.Code <= Keyboard.Key.Num9)
        {
            if (alt) { Disp.currentToolIndex = e.Code - Keyboard.Key.Num0; }
            else { Disp.currentSelectedLayer = e.Code - Keyboard.Key.Num0; }
        }
        if (e.Code == Keyboard.Key.LAlt || e.Code == Keyboard.Key.RAlt)
        {
            alt = true;
        }
        tools[Disp.currentToolIndex].OnKeyPress(sender, e);
    }
    internal void OnMouseMove(object sender, MouseMoveEventArgs e)
    {
        tools[Disp.currentToolIndex].OnMouseMove(sender, e);
    }
    internal void OnMouseScroll(object sender, MouseWheelScrollEventArgs e)
    {
        tools[Disp.currentToolIndex].OnMouseScroll(sender, e);
    }
    internal void OnMousePress(object sender, MouseButtonEventArgs e)
    {
        tools[Disp.currentToolIndex].OnMousePress(sender, e);
    }
    internal void OnMouseRelease(object sender, MouseButtonEventArgs e)
    {
        tools[Disp.currentToolIndex].OnMouseRelease(sender, e);
    }

    /*
    public double getMaxTileSize()
    {
        return Math.Max(Disp.settings.startScreenWidth, Disp.settings.startScreenHeight) / Disp.settings.minTilesShown;
    }

    public double getMinTileSize()
    {
        return Math.Max(Disp.settings.startScreenWidth, Disp.settings.startScreenHeight) / Disp.settings.maxTilesShown;
    }
    */

    /*
    public void setViewTile(Vector2i tileCoords, Vector2f screenCoords)
    {
        viewTileCoords = tileCoords;
        viewTileDisplayCoords = screenCoords;

        setWhetherViewingTile(true);
    }
    */

    /*
    public void setWhetherViewingTile(bool view)
    {
        viewingTile = view;
    }
    */

    public Vector2i getTileCoordsFromScreenCoords()
    {
        return new Vector2i((int)(Camera.xOffset), (int)(Camera.yOffset));
    }

    public int getDisplayMode()
    {
        return Disp.settings.displayMode;
    }
    public void setDisplayMode(int mode)
    {
        Disp.settings.displayMode = mode;
    }

}
