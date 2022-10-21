using Map_Generator_CSharp.Source.external_util;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;
using Image = SFML.Graphics.Image;

namespace IS_Compressions.code.display;
class DisplayManager
{
    public struct DisplaySettings
    {
        public bool mapStartEnabled;
        public int startScreenWidth, startScreenHeight;
        public float initialXOffset, initialYOffset, initialTilesShown;
        public double minTilesShown, maxTilesShown;
        public int baseR, baseG, baseB, outR, outG, outB;
        public int displayMode, pixelWidth, pixelHeight;

        public DisplaySettings(bool v1, int x, int y, float v2, float v3, float v4, double v5, double v6, int v7, int v8,
            int v9, int v10, int v11, int v12, int v13, int v14, int v15) : this()
        {
            mapStartEnabled = v1; startScreenWidth = x; startScreenHeight = y; initialXOffset = v2; initialYOffset = v3; initialTilesShown = v4;
            minTilesShown = v5; maxTilesShown = v6;
            baseR = v7; baseG = v8; baseB = v9; outR = v10; outG = v11; outB = v12; displayMode = v13; pixelWidth = v14; pixelHeight = v15;
        }
    }

    private DisplaySettings displaySettings;
    private float xOffset, yOffset;

    private RenderWindow window;
    private View view;
    private Font font;

    
    private OverlayLayer overlay;
    private LayerHolder layers;

    private bool activeMap = false;
    private bool mapNeedsUpdate = false;
    private bool activeMapUI = false;
    private bool activeMenu = false;

    public int fps;

    private bool viewingTile = false;
    private Vector2i viewTileCoords;
    private Vector2f viewTileDisplayCoords;

    private double cameraSpeed, effectiveCameraSpeed;

    private string @resourceDir;

    Clock clock;

    //Movement Stuffs
    int recentDragX, recentDragY;

    bool drag = false;

    bool left = false, right = false, up = false, down = false, shift = false, control = false;

    double clickTime;
    int clickX, clickY;

    double maxClickLength = 0.2; // How long a click can be to qualify (in seconds)

    public double deltaTime;

    float scale;


    public void SetClock(Clock c)
    {
        clock = c;
    }
    private void Draw()
    {
        // clear map rendertexture
        window.Clear(new Color(150, 150, 150));


        layers.Draw();
        overlay.Draw(xOffset,yOffset,fps);

        // pass rendertexture to window

        var renderTextureSprite = new Sprite(layers.screenRenderTexture.Texture);

        //renderTextureSprite.Scale = ;
        renderTextureSprite.Position = new Vector2f(xOffset * scale + getWindowWidth() / 2, yOffset * scale + getWindowHeight() / 2);
        renderTextureSprite.Scale = new Vector2f(scale, scale);
        window.Draw(renderTextureSprite);
        
        var overlayTextureSprite = new Sprite(overlay.overlayRenderTexture.Texture);
        overlayTextureSprite.TextureRect = new IntRect(0, (int)getWindowHeight(), (int)getWindowWidth(), (int)-getWindowHeight());
        //overlayTextureSprite.Scale = new Vector2f(1,1);
        window.Draw(overlayTextureSprite);
    }

    double getTilesShown()
    {
        return Math.Max(displaySettings.startScreenWidth, displaySettings.startScreenHeight);
    }

    public DisplayManager(DisplaySettings settings, Layer backLayer, string rDir)
    {
        window = new RenderWindow(new VideoMode((uint)settings.startScreenWidth, (uint)settings.startScreenHeight), "Cat Viewer Deluxe Extreme Edition ++#");


        view = new View();
        view.Center = new Vector2f(settings.startScreenWidth / 2, settings.startScreenHeight / 2);
        view.Size = new Vector2f(settings.startScreenWidth, settings.startScreenHeight);

        window.SetView(view);

        displaySettings = settings;

        resourceDir = rDir;

        Console.WriteLine("Using resource directory " + resourceDir);

        xOffset = settings.initialXOffset;
        yOffset = settings.initialYOffset;
        //tileSize = (float)(Math.Max(settings.screenWidth, settings.screenHeight) / settings.initialTilesShown);

        loadFont();
        loadIcon();

        layers = new LayerHolder(backLayer);
        overlay = new OverlayLayer(settings, ref window, ref font);

        // How many seconds does it take to move across one screen with the camera?
        double cameraSecondsPerScreen = 2;
        // Camera speed in tiles per second
        cameraSpeed = displaySettings.initialTilesShown / cameraSecondsPerScreen;
        effectiveCameraSpeed = cameraSpeed;

        changeScale(3);
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
    //Called after all movement keys have been processed
    public void Move()
    {
        effectiveCameraSpeed = cameraSpeed;
        effectiveCameraSpeed *= deltaTime; // Make speed ignore FPS
        if (shift) { effectiveCameraSpeed *= 3; }

        if (control) { effectiveCameraSpeed /= 3; }

        if (!drag)
        {
            if (up && !down) { moveCamera(0, (float)effectiveCameraSpeed); }
            else if (down && !up) { moveCamera(0, (float)-effectiveCameraSpeed); }
            if (left && !right) { moveCamera((float)effectiveCameraSpeed, 0); }
            else if (right && !left) { moveCamera((float)-effectiveCameraSpeed, 0); }
        }
    }

    public void resize(object sender, SizeEventArgs e)
    {
        displaySettings.startScreenWidth = (int)e.Width;
        displaySettings.startScreenHeight = (int)e.Height;
        changeScale(0); // Make sure tilesize is within bounds

        view.Center = new Vector2f(displaySettings.startScreenWidth / 2, displaySettings.startScreenHeight / 2);
        view.Size = new Vector2f(displaySettings.startScreenWidth, displaySettings.startScreenHeight);

        window.SetView(view);

        //mapRenderTexture = new RenderTexture((uint)displaySettings.startScreenWidth, (uint)displaySettings.startScreenHeight);
        overlay.Resize((uint)displaySettings.startScreenWidth, (uint)displaySettings.startScreenHeight);
        layers.Resize((uint)displaySettings.startScreenWidth, (uint)displaySettings.startScreenHeight);
    }
    public void Display()
    {
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
        var centerX = xOffset * scale + getWindowWidth() / 2;
        var centerY = yOffset * scale + getWindowHeight() / 2;

        //var centerX = xOffset / scale;
        //var centerY = yOffset / sacle;// + getWindowHeight() / 2;

        return new Vector2f((float)centerX, (float)centerY);
    }

    public double getMaxTileSize()
    {
        return Math.Max(displaySettings.startScreenWidth, displaySettings.startScreenHeight) / displaySettings.minTilesShown;
    }

    public double getMinTileSize()
    {
        return Math.Max(displaySettings.startScreenWidth, displaySettings.startScreenHeight) / displaySettings.maxTilesShown;
    }

    public void changeScale(double delta)
    {
        scale = (float)Math.Max(Math.Min(scale + delta, getMaxTileSize()), getMinTileSize());

        //xOffset = center.X - displaySettings.startScreenWidth / (2 * tileSize);
        //yOffset = center.Y - displaySettings.startScreenHeight / (2 * tileSize);

        //setWhetherViewingTile(false); // Leads to general problems if this isn't here
    }
    public void moveCamera(float x, float y)
    {
        xOffset += x / scale;
        yOffset += y / scale;

        viewTileDisplayCoords.X += x;// * (displaySettings.startScreenWidth / getWindowWidth());
        viewTileDisplayCoords.Y += y;// * (displaySettings.startScreenHeight / getWindowHeight());
    }
    public void setViewTile(Vector2i tileCoords, Vector2f screenCoords)
    {
        viewTileCoords = tileCoords;
        viewTileDisplayCoords = screenCoords;

        setWhetherViewingTile(true);
    }

    public void setWhetherViewingTile(bool view)
    {
        viewingTile = view;
    }

    public Vector2i getTileCoordsFromScreenCoords(int screenX, int screenY)
    {
        return new Vector2i((int)(xOffset + (double)screenX), (int)(yOffset + (double)screenY));
    }

    public void onClick(int clickX, int clickY)
    {
        /*
        if (viewingTile)
        {
            setWhetherViewingTile(false);
        }
        else
        {
            setViewTile(new Vector2i(100, 100), new Vector2f(100, 100));
        }
        */
        //var tileCoords = getTileCoordsFromScreenCoords(clickX, clickY);

        /*
        if ((!viewingTile || tileCoords != viewTileCoords) && tileCoords.X >= 0 && tileCoords.X < tileMap.getWidth() && tileCoords.Y >= 0 && tileCoords.Y < tileMap.getHeight())
        {
            setViewTile(tileCoords, new Vector2f(clickX + displaySettings.startScreenWidth / 20, clickY + displaySettings.startScreenHeight / 20));
        }
        else
        {
            setWhetherViewingTile(false);
        }
        */
    }

    public int getDisplayMode()
    {
        return displaySettings.displayMode;
    }
    public void setDisplayMode(int mode)
    {
        displaySettings.displayMode = mode;
    }
    public void OnKeyRelease(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.W || e.Code == Keyboard.Key.Up) { up = false; }

        else if (e.Code == Keyboard.Key.S || e.Code == Keyboard.Key.Down) { down = false; }

        else if (e.Code == Keyboard.Key.A || e.Code == Keyboard.Key.Left) { left = false; }

        else if (e.Code == Keyboard.Key.D || e.Code == Keyboard.Key.Right) { right = false; }

        else if (e.Code == Keyboard.Key.LShift || e.Code == Keyboard.Key.RShift) { shift = false; }

        else if (e.Code == Keyboard.Key.LControl || e.Code == Keyboard.Key.RControl) { control = false; }
    }
    public void OnKeyPress(object sender, KeyEventArgs e)
    {

        // Move via WASD. Mouse take priority over WASD.
        if (e.Code == Keyboard.Key.W || e.Code == Keyboard.Key.Up) { up = true; }

        else if (e.Code == Keyboard.Key.S || e.Code == Keyboard.Key.Down) { down = true; }

        else if (e.Code == Keyboard.Key.A || e.Code == Keyboard.Key.Left) { left = true; }

        else if (e.Code == Keyboard.Key.D || e.Code == Keyboard.Key.Right) { right = true; }

        else if (e.Code == Keyboard.Key.LShift || e.Code == Keyboard.Key.RShift) { shift = true; }

        else if (e.Code == Keyboard.Key.LControl || e.Code == Keyboard.Key.RControl) { control = true; }

        // changes color mode
        else if (e.Code == Keyboard.Key.C)
        {
            setDisplayMode((getDisplayMode() + 3) % 4);
            //tileMap.rerenderTiles(getDisplayMode());
            //drawTiles();
        }
        else if (e.Code == Keyboard.Key.V)
        {
            setDisplayMode((getDisplayMode() + 1) % 4);
            //tileMap.rerenderTiles(getDisplayMode());
            //drawTiles();
        }

        // exits tile view
        else if (e.Code == Keyboard.Key.Escape)
        {
            setWhetherViewingTile(false);
        }

        else if (e.Code == Keyboard.Key.F1)
        {
            activeMapUI = !activeMapUI;
        }

    }
    public void OnMouseMove(object sender, MouseMoveEventArgs e)
    {
        if (drag)
        {
            var newX = e.X;
            var newY = e.Y;

            moveCamera(-(recentDragX - newX), -(recentDragY - newY));

            recentDragX = newX;
            recentDragY = newY;
        }
    }
    public void OnMousePress(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            recentDragX = e.X;
            recentDragY = e.Y;
            drag = true;

            clickX = e.X;
            clickY = e.Y;
            clickTime = clock.ElapsedTime.AsSeconds();
        }
    }
    public void OnMouseRelease(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            drag = false;

            if (e.X - clickX == 0 && e.Y - clickY == 0 && clock.ElapsedTime.AsSeconds() - clickTime <= maxClickLength)
            {
                onClick((int)(clickX * ((double)displaySettings.startScreenWidth / getWindowWidth())), (int)(clickY * ((double)displaySettings.startScreenHeight / getWindowHeight())));
            }
        }
    }
    public void OnMouseScroll(object sender, MouseWheelScrollEventArgs e)
    {
        changeScale(e.Delta);
        //moveCamera(1 / e.Delta, 1 / e.Delta);
        //scale = e.Delta;
        double cameraSecondsPerScreen = 2;

        cameraSpeed = getTilesShown() / cameraSecondsPerScreen; // Tiles per second
    }
}
