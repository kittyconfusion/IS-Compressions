using Map_Generator_CSharp.Source.external_util;
using Map_Generator_CSharp.Source.tiles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;
using Image = SFML.Graphics.Image;

namespace IS_Compressions.code;
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
            baseR = v7; baseG = v8; baseB = v9; outR = v10; outG = v11; outB = v12; displayMode = v13; pixelWidth = v14; pixelHeight = v15 ;
        }
    }

    private DisplaySettings displaySettings;
    private float xOffset, yOffset;

    private RenderWindow window;
    private View view;
    private Font font;

    private RenderTexture mapRenderTexture;
    private RenderTexture overlayRenderTexture;
    private RenderTexture menuRenderTexture;

    private bool activeMap = false;
    private bool mapNeedsUpdate = false;
    private bool activeMapUI = false;
    private bool activeMenu = false;

    public int fps;

    private bool viewingTile = false;
    private Vector2i viewTileCoords;
    private Vector2f viewTileDisplayCoords;

    private double cameraSpeed, effectiveCameraSpeed;

    private TileMap tileMap;

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

    private List<Vertex> rectPoints = new List<Vertex>(40000);

    public void renderMap(bool active)
    {
        activeMap = active;
    }
    public void renderMapUI(bool active)
    {
        activeMapUI = active;
    }
    public void renderMenu(bool active)
    {
        activeMenu = active;
    }
    public bool renderingMap()
    {
        return activeMap;
    }
    public bool renderingMapUI()
    {
        return activeMapUI;
    }
    public bool renderingMenu()
    {
        return activeMenu;
    }

    public void SetClock(Clock c)
    {
        clock = c;
    }
    private void draw()
    {
        if (activeMap)
        {
            // clear map rendertexture
            //mapRenderTexture.Clear(new Color(63, 63, 55));
            overlayRenderTexture.Clear(new Color(0, 0, 0, 0));
            menuRenderTexture.Clear(new Color(0, 0, 0, 0));

            // map drawing
            //drawTiles();

            if (activeMapUI)
            {
                if (viewingTile) { drawTileStats(); }
                drawCoords();
                //drawControls();
                drawDebug();
                //drawColorScheme();
            }


            // update map rendertexture
            mapRenderTexture.Display();

            // pass rendertexture to window

            var renderTextureSprite = new Sprite(mapRenderTexture.Texture);

            //renderTextureSprite.Scale = ;
            renderTextureSprite.Position = new Vector2f(xOffset * scale + getWindowWidth() / 2, yOffset * scale + getWindowHeight() / 2);
            renderTextureSprite.Scale = new Vector2f(scale, scale);
            window.Draw(renderTextureSprite);

            var overlayTextureSprite = new Sprite(overlayRenderTexture.Texture);
            overlayTextureSprite.TextureRect = new IntRect(0, (int)getWindowHeight(), (int)getWindowWidth(), (int)-getWindowHeight());
            //overlayTextureSprite.Scale = new Vector2f(1,1);
            window.Draw(overlayTextureSprite);
        }
        if (activeMenu)
        {
            menuRenderTexture.Display();

            var renderTextureSprite = new Sprite(menuRenderTexture.Texture);
            window.Draw(renderTextureSprite);
        }
        /*
        var rect = new RectangleShape(new Vector2f(5, 5));
        rect.Position = getCameraCenter();
        rect.FillColor = new Color(255, 0, 0);
        window.Draw(rect);
        */
    }
    public void drawTiles()
    {

        //var tileDisplayWidth = (int)(displaySettings.startScreenWidth / tileSize) + 2;
        //var tileDisplayHeight = (int)(displaySettings.startScreenHeight / tileSize) + 2;

        int width = tileMap.getWidth();
        int height = tileMap.getHeight();

        rectPoints.Clear();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {

                if (!tileMap.inBounds(x, y))
                {
                    Console.WriteLine("Not in bounds " + x + " " + y);
                    continue;
                }
                //if (x < 0 || x >= width || y < 0 || y >= height)
                //{
                //    continue;
                //}

                var t = tileMap.getTile(x, y);
                var screenPos = new Vector2f(x, y);
                //var screenPos = new Vector2f(0, 0);
                Color col;

                if (viewingTile == true && viewTileCoords.X == x && viewTileCoords.Y == y && activeMapUI)
                {
                    col = new Color(255, 100, 100);
                }
                else
                {

                    col = t.getColor(getDisplayMode());
                }

                var v = new Vertex(screenPos + new Vector2f(0, 0), col);
                rectPoints.Add(v);

                v = new Vertex(screenPos + new Vector2f(1, 0), col);
                rectPoints.Add(v);

                v = new Vertex(screenPos + new Vector2f(1, 1), col);
                rectPoints.Add(v);

                v = new Vertex(screenPos + new Vector2f(0, 1), col);
                rectPoints.Add(v);
            }
            if (rectPoints.Count >= 40000)
            {
                mapRenderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
                rectPoints.Clear();
                //rectPoints = new List<Vertex>(10000);
            }
        }
        //mapRenderTexture.Draw()
        mapRenderTexture.Draw(rectPoints.ToArray(), PrimitiveType.Quads);
    }
    /*
    private RectangleShape drawTile(Tile t, Vector2f screenPos)
    {
        return drawTile(t.getColor(getDisplayMode()), screenPos);
    }
    private RectangleShape drawTile(Color highlight, Vector2f screenPos)
    {
        RectangleShape rect = new RectangleShape();
        rect.Position = screenPos;
        rect.Size = new Vector2f((float)tileSize, (float)tileSize);
        rect.FillColor = highlight;
        return rect;
        //mapRenderTexture.Draw(rect);
    }
    */
    private void drawTileStats()
    {
        var viewTile = tileMap.getTile(viewTileCoords.X, viewTileCoords.Y);

        string featureText = "This is text";

        var offset = 5;
        var fontSize = 30; // Pixels

        var xSize = (int)(13.5 * fontSize);
        var ySize = 4 * fontSize + 6 * offset;

        var size = new Vector2f(xSize, ySize);
        var rect = new RoundedRectangleShape(size, 5, 5); // A class I found off of GitHub (make sure to add the files to your IDE in order to see them)
        rect.FillColor = new Color(100, 100, 100);
        rect.Position = viewTileDisplayCoords;

        menuRenderTexture.Draw(rect);

        var text = new Text();
        text.Font = font;
        text.CharacterSize = (uint)fontSize;
        text.FillColor = new Color((byte)displaySettings.baseR, (byte)displaySettings.baseG, (byte)displaySettings.baseB);
        text.OutlineThickness = 2;
        text.OutlineColor = new Color((byte)displaySettings.outR, (byte)displaySettings.outG, (byte)displaySettings.outB);

        text.Style = Text.Styles.Bold | Text.Styles.Underlined;
        text.Position = new Vector2f(viewTileDisplayCoords.X + offset, viewTileDisplayCoords.Y + offset);
        text.DisplayedString = "Tile (" + viewTileCoords.X + "," + viewTileCoords.Y + ")" + (featureText != "" ? " - " + featureText : "");
        menuRenderTexture.Draw(text);
        text.Style = Text.Styles.Regular;
        /*
        text.Position = new Vector2f(text.Position.X, text.Position.Y + fontSize + offset);
        text.DisplayedString = "Elevation: " + (int)(100 * viewTile.getAttribute("elevation"));
        mapRenderTexture.Draw(text);

        text.Position = new Vector2f(text.Position.X, text.Position.Y + fontSize + offset);
        text.DisplayedString = "Temperature: " + (int)(100 * viewTile.getAttribute("temperature"));
        mapRenderTexture.Draw(text);

        text.Position = new Vector2f(text.Position.X, text.Position.Y + fontSize + offset);
        text.DisplayedString = "Humidity: " + (int)(100 * viewTile.getAttribute("humidity"));
        mapRenderTexture.Draw(text);
        */

    }

    private void drawCoords()
    {
        var coordText = new Text();
        coordText.Font = font;

        //Coords are in the middle of the screen
        var coords = new Vector2f(-xOffset, -yOffset);

        //Vector2f mouseCoords = new Vector2f(Mouse.GetPosition().X - window.Position.X, Mouse.GetPosition().Y - window.Position.Y);
        //Vector2f coords = (mouseCoords + new Vector2f(xOffset, yOffset)) / scale;

        coordText.DisplayedString = "(" + (int)coords.X + ", " + (int)coords.Y + ")";


        coordText.CharacterSize = 50; // Pixels, not normal font size
        coordText.FillColor = new Color((byte)displaySettings.baseR, (byte)displaySettings.baseG, (byte)displaySettings.baseB); // Color

        coordText.OutlineThickness = 2;
        coordText.OutlineColor = new Color((byte)displaySettings.outR, (byte)displaySettings.outG, (byte)displaySettings.outB);

        coordText.Style = Text.Styles.Bold;


        coordText.Position = new Vector2f(10, 10);
        overlayRenderTexture.Draw(coordText);
    }
    private void drawControls()
    {
        var controlText = new Text();
        controlText.Font = font;
        if (Keyboard.IsKeyPressed(Keyboard.Key.H))
        {
            controlText.DisplayedString = "WASD/arrows/click-and-drag to move. Shift to go faster.\nSpace to regenerate terrain. F3 to enter seed in console.\nC/V to change display mode.\nClick on tile to view, ESC to stop viewing.\nF1 to toggle UI.";
        }
        else
        {
            controlText.DisplayedString = "H for controls.";
        }

        controlText.CharacterSize = 30; // Pixels, not normal font size
        controlText.FillColor = new Color((byte)displaySettings.baseR, (byte)displaySettings.baseG, (byte)displaySettings.baseB); // Color

        controlText.OutlineThickness = 2;
        controlText.OutlineColor = new Color((byte)displaySettings.outR, (byte)displaySettings.outG, (byte)displaySettings.outB);

        controlText.Style = Text.Styles.Bold;


        controlText.Position = new Vector2f(10, 110);
        overlayRenderTexture.Draw(controlText);
    }
    private void drawDebug()
    {
        var debugText = new Text();
        debugText.Font = font;

        debugText.DisplayedString = fps.ToString();


        debugText.CharacterSize = 40; // Pixels, not normal font size
        debugText.FillColor = new Color(0, 255, 0); // Color

        debugText.OutlineThickness = 2;
        debugText.OutlineColor = new Color(0, 0, 0);

        debugText.Style = Text.Styles.Bold;


        debugText.Position = new Vector2f(displaySettings.startScreenWidth - debugText.GetGlobalBounds().Width - 10, 10);
        overlayRenderTexture.Draw(debugText);

        //debugText.DisplayedString = tileMap.getSeed().ToString();

        debugText.FillColor = new Color(255, 255, 255);

        debugText.CharacterSize = 15;

        //debugText.Position = new Vector2f(displaySettings.startScreenWidth - debugText.GetGlobalBounds().Width - 10, 70);
        //overlayRenderTexture.Draw(debugText);

        debugText.DisplayedString = tileMap.GetSettings().width + " x " + tileMap.GetSettings().height + "\n";

        if (tileMap.inBounds(-(int)xOffset, -(int)yOffset))
        {
            debugText.DisplayedString += tileMap.getTile(-(int)xOffset, -(int)yOffset).getColor().ToString();
        }
        
        debugText.Position = new Vector2f(displaySettings.startScreenWidth - debugText.GetGlobalBounds().Width - 10, 100);
        overlayRenderTexture.Draw(debugText);
    }
    private void drawColorScheme()
    {
        var colorSchemeText = new Text();
        colorSchemeText.Font = font;

        switch (displaySettings.displayMode)
        {
            case 0:
                colorSchemeText.DisplayedString = "Elevation + Features";
                break;
            case 1:
                colorSchemeText.DisplayedString = "Elevation";
                break;
            case 2:
                colorSchemeText.DisplayedString = "Temperature";
                break;
            case 3:
                colorSchemeText.DisplayedString = "Humidity";
                break;
            default:
                colorSchemeText.DisplayedString = "Invalid display setting!";
                break;
        }


        colorSchemeText.CharacterSize = 40; // Pixels, not normal font size
        colorSchemeText.FillColor = new Color((byte)displaySettings.baseR, (byte)displaySettings.baseG, (byte)displaySettings.baseB); // Color

        colorSchemeText.OutlineThickness = 2;
        colorSchemeText.OutlineColor = new Color((byte)displaySettings.outR, (byte)displaySettings.outG, (byte)displaySettings.outB);

        colorSchemeText.Style = Text.Styles.Bold;


        colorSchemeText.Position = new Vector2f(10, displaySettings.startScreenHeight - colorSchemeText.GetGlobalBounds().Height - 20);
        overlayRenderTexture.Draw(colorSchemeText);
    }
    double getTilesShown()
    {
        return Math.Max(displaySettings.startScreenWidth, displaySettings.startScreenHeight);
    }

    public DisplayManager(DisplaySettings settings, TileMap tm, string rDir)
    {
        window = new RenderWindow(new VideoMode((uint)settings.startScreenWidth, (uint)settings.startScreenHeight), "Cat Viewer Deluxe Extreme Edition ++");

        mapRenderTexture = new RenderTexture((uint)tm.getWidth(), (uint)tm.getHeight());
        menuRenderTexture = new RenderTexture((uint)settings.startScreenWidth, (uint)settings.startScreenHeight);
        overlayRenderTexture = new RenderTexture((uint)settings.startScreenWidth, (uint)settings.startScreenHeight);

        view = new View();
        view.Center = new Vector2f(settings.startScreenWidth / 2, settings.startScreenHeight / 2);
        view.Size = new Vector2f(settings.startScreenWidth, settings.startScreenHeight);

        window.SetView(view);

        if (settings.mapStartEnabled)
        {
            activeMap = true;
            activeMapUI = true;
        }
        else
            activeMenu = true;

        displaySettings = settings;

        tileMap = tm;

        resourceDir = rDir;
        /*
        if (resourceDir == "")
        {
            resourceDir = ExePath::mergePaths(ExePath::getExecutableDir(), "Resources");
        }
        */
        //std::cout << "Using resource directory: " << resourceDir << "\n";

        xOffset = settings.initialXOffset;
        yOffset = settings.initialYOffset;
        //tileSize = (float)(Math.Max(settings.screenWidth, settings.screenHeight) / settings.initialTilesShown);

        loadFont();
        loadIcon();

        // How many seconds does it take to move across one screen with the camera?
        double cameraSecondsPerScreen = 2;
        // Camera speed in tiles per second
        cameraSpeed = displaySettings.initialTilesShown / cameraSecondsPerScreen;
        effectiveCameraSpeed = cameraSpeed;

        changeTileSize(3);
    }
    public RenderWindow GetWindow()
    {
        return window;
    }

    public void setTileMap(TileMap tm)
    {
        tileMap = tm;
        setWhetherViewingTile(false);
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
        changeTileSize(0); // Make sure tilesize is within bounds

        view.Center = new Vector2f(displaySettings.startScreenWidth / 2, displaySettings.startScreenHeight / 2);
        view.Size = new Vector2f(displaySettings.startScreenWidth, displaySettings.startScreenHeight);

        window.SetView(view);

        //mapRenderTexture = new RenderTexture((uint)displaySettings.startScreenWidth, (uint)displaySettings.startScreenHeight);
        menuRenderTexture = new RenderTexture((uint)displaySettings.startScreenWidth, (uint)displaySettings.startScreenHeight);
    }
    public void display()
    {
        window.Clear();
        draw();
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
        var centerX = xOffset * scale + (getWindowWidth() / 2);
        var centerY = yOffset * scale + (getWindowHeight() / 2);

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

    public void changeTileSize(double delta)
    {
        scale = (float)Math.Max(Math.Min(scale + delta, getMaxTileSize()), getMinTileSize());

        //xOffset = center.X - displaySettings.startScreenWidth / (2 * tileSize);
        //yOffset = center.Y - displaySettings.startScreenHeight / (2 * tileSize);

        setWhetherViewingTile(false); // Leads to general problems if this isn't here
    }
    public void moveCamera(float x, float y)
    {
        xOffset += x / scale;
        yOffset += y / scale;

        viewTileDisplayCoords.X -= x * (displaySettings.startScreenWidth / getWindowWidth());
        viewTileDisplayCoords.Y -= y * (displaySettings.startScreenHeight / getWindowHeight());
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

        if (viewingTile)
        {
            setWhetherViewingTile(false);
        }
        else
        {
            setViewTile(new Vector2i(100, 100), new Vector2f(200, 200));
        }
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
            tileMap.rerenderTiles(getDisplayMode());
        }
        else if (e.Code == Keyboard.Key.V)
        {
            setDisplayMode((getDisplayMode() + 1) % 4);
            tileMap.rerenderTiles(getDisplayMode());
        }

        // exits tile view
        else if (e.Code == Keyboard.Key.Escape)
        {
            setWhetherViewingTile(false);
        }

        else if (e.Code == Keyboard.Key.F1)
        {
            renderMapUI(!renderingMapUI());
        }

    }
    public void OnMouseMove(object sender, MouseMoveEventArgs e)
    {
        if (drag)
        {
            int newX = e.X;
            int newY = e.Y;

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
        changeTileSize(e.Delta);
        //moveCamera(1 / e.Delta, 1 / e.Delta);
        //scale = e.Delta;
        double cameraSecondsPerScreen = 2;

        cameraSpeed = getTilesShown() / cameraSecondsPerScreen; // Tiles per second
    }
}
