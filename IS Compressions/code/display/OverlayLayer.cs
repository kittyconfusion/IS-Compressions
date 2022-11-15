using IS_Compressions.code.core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace IS_Compressions.code.display;
internal class OverlayLayer
{
    internal RenderTexture overlayRenderTexture;
    private RenderWindow window;
    private LayerHolder layers;
    private ColorCache colors;
    Font font;

    public OverlayLayer(ref RenderWindow window, ref Font font, ref LayerHolder layers, ref ColorCache colors)
    {
        this.window = window;
        this.font = font;
        this.overlayRenderTexture = new RenderTexture((uint)Display.settings.startScreenWidth, (uint)Display.settings.startScreenHeight);
        this.layers = layers;
        this.colors = colors;
    }
    public void Resize(uint x, uint y)
    {
        overlayRenderTexture.Dispose(); //Object is never freed from memory if not called
        overlayRenderTexture = new RenderTexture(x, y);
    }
    public void Draw()
    {
        overlayRenderTexture.Clear(new Color(0, 0, 0, 0));
        DrawCoords();
        DrawControls();
        DrawTool();
        DrawDebug();
    }
    private void DrawCoords()
    {
        var coordText = new Text();
        coordText.Font = font;

        //Coords are in the middle of the screen

        coordText.DisplayedString = "(" + (int)-Camera.xOffset + ", " + (int)-Camera.yOffset + ")";
        coordText.CharacterSize = 50; // Pixels, not normal font size
        coordText.FillColor = new Color((byte)Display.settings.baseR, (byte)Display.settings.baseG, (byte)Display.settings.baseB); // Color

        coordText.OutlineThickness = 2;
        coordText.OutlineColor = new Color((byte)Display.settings.outR, (byte)Display.settings.outG, (byte)Display.settings.outB);
        coordText.Style = Text.Styles.Bold;


        coordText.Position = new Vector2f(10, 10);
        overlayRenderTexture.Draw(coordText);
    }
    private void DrawControls()
    {
        var controlText = new Text();
        controlText.Font = font;
        if (Keyboard.IsKeyPressed(Keyboard.Key.H))
        {
            controlText.DisplayedString = "WASD/arrows/click-and-drag to move. \nShift/Control to go faster/slower. \nC/V to change display mode.\nF1 to toggle UI.";
        }
        else
        {
            //controlText.DisplayedString = "H for controls.";
        }
        controlText.CharacterSize = 30; // Pixels, not normal font size
        controlText.FillColor = new Color((byte)Display.settings.baseR, (byte)Display.settings.baseG, (byte)Display.settings.baseB); // Color

        controlText.OutlineThickness = 2;
        controlText.OutlineColor = new Color((byte)Display.settings.outR, (byte)Display.settings.outG, (byte)Display.settings.outB);
        controlText.Style = Text.Styles.Bold;


        controlText.Position = new Vector2f(10, 110);
        overlayRenderTexture.Draw(controlText);
    }
    
    private void DrawDebug()
    {
        var debugText = new Text();
        debugText.Font = font;

        debugText.DisplayedString = TimeKeeper.fps.ToString();


        debugText.CharacterSize = 26; // Pixels, not normal font size
        debugText.FillColor = new Color(255, 255, 255); // Color

        debugText.OutlineThickness = 2;
        debugText.OutlineColor = new Color(0, 0, 0);

        debugText.Style = Text.Styles.Bold;

        
        debugText.Position = new Vector2f(window.Size.X - debugText.GetGlobalBounds().Width - 10, 10);
        overlayRenderTexture.Draw(debugText);
        
        debugText.FillColor = new Color(255, 255, 255);

        debugText.CharacterSize = 24;
        
        debugText.DisplayedString = "(\t" + "WIP"; //"tileMap.GetSettings().width + " x " + tileMap.GetSettings().height;

        
        //Get the current position of the mouse relative to the window
        Vector2f mouseCoords = new Vector2f(Mouse.GetPosition().X - window.Position.X, Mouse.GetPosition().Y - window.Position.Y - 32);
        //Find the tile the mouse is currently hovering over
        Vector2f attemptTilePos = new Vector2f((mouseCoords.X / Camera.scale) - Camera.xOffset, ((mouseCoords.Y / Camera.scale) - Camera.yOffset));
        //Account for map centering in the middle of the screen instead of the top right corner
        attemptTilePos -= new Vector2f((float)window.Size.X / 2 / Camera.scale, (float)window.Size.Y / 2 / Camera.scale);

        Vector2i attemptTilePosInt = new Vector2i((int)attemptTilePos.X, (int)attemptTilePos.Y);

        debugText.DisplayedString += "\t\tX(" + attemptTilePosInt.X + ") Y(" + attemptTilePosInt.Y + ")\t";

        //debugText.DisplayedString += layers.GetTopLayerAtPos(attemptTilePosInt.X, attemptTilePosInt.Y);

        /*
        var c = 0;
        foreach(Pixel? p in layers.GetPixelsAtPos(attemptTilePosInt.X, attemptTilePosInt.Y))
        {
            if (p == null) { continue; }
            Color colStr = p.GetColor();
            debugText.DisplayedString += "R(" + colStr.R + ") G(" + colStr.G + ") B(" + colStr.B + ") A(" + colStr.A + ")  ";

            c += 1;
        }
        */
        if (colors.InBounds(attemptTilePosInt.X, attemptTilePosInt.Y)) 
        {
            Color colStr = colors.GetCachedColor(attemptTilePosInt.X, attemptTilePosInt.Y);
            debugText.DisplayedString += "R(" + colStr.R + ") G(" + colStr.G + ") B(" + colStr.B + ") A(" + colStr.A + ")  ";
        }

        debugText.Position = new Vector2f(-15, window.Size.Y - debugText.GetGlobalBounds().Height - 10);
        overlayRenderTexture.Draw(debugText);
    }
    private void DrawTool()
    {
        var colorSchemeText = new Text();
        colorSchemeText.Font = font;

        switch (Display.settings.displayMode)
        {
            case 0:
                colorSchemeText.DisplayedString = "...";
                break;
            case 1:
                colorSchemeText.DisplayedString = "Red";
                break;
            case 2:
                colorSchemeText.DisplayedString = "Green";
                break;
            case 3:
                colorSchemeText.DisplayedString = "Blue";
                break;
            default:
                colorSchemeText.DisplayedString = "Invalid display setting!";
                break;
        }
        colorSchemeText.CharacterSize = 40; // Pixels, not normal font size
        colorSchemeText.FillColor = new Color((byte)Display.settings.baseR, (byte)Display.settings.baseG, (byte)Display.settings.baseB); // Color

        colorSchemeText.OutlineThickness = 2;
        colorSchemeText.OutlineColor = new Color((byte)Display.settings.outR, (byte)Display.settings.outG, (byte)Display.settings.outB);
        colorSchemeText.Style = Text.Styles.Bold;


        colorSchemeText.Position = new Vector2f(window.Size.X - colorSchemeText.GetGlobalBounds().Width - 10, window.Size.Y - colorSchemeText.GetGlobalBounds().Height - 20);
        overlayRenderTexture.Draw(colorSchemeText);
    }
    /*
    private void drawTileStats()
    {
        
        var viewTile = tileMap.getTile(viewTileCoords.X, viewTileCoords.Y);

        var featureText = "This is text";

        


        var xSize = (int)(13.5 * fontSize);
        var ySize = 4 * fontSize + 6 * offset;

        var size = new Vector2f(xSize, ySize);
        var rect = new RoundedRectangleShape(size, 5, 5);
        rect.FillColor = new Color(100, 100, 100);
        rect.Position = viewTileDisplayCoords;

        menuRenderTexture.Draw(rect);
        var fontSize = 30; // Pixels
        var offset = 5;

        var text = new Text();
        text.Font = font;
        text.CharacterSize = (uint)fontSize;
        text.FillColor = new Color((byte)Display.settings.baseR, (byte)Display.settings.baseG, (byte)Display.settings.baseB);
        text.OutlineThickness = 2;
        text.OutlineColor = new Color((byte)Display.settings.outR, (byte)Display.settings.outG, (byte)Display.settings.outB);

        text.Style = Text.Styles.Bold | Text.Styles.Underlined;
        text.Position = new Vector2f(viewTileDisplayCoords.X + offset, viewTileDisplayCoords.Y + offset);
        text.DisplayedString = "Tile (" + viewTileCoords.X + "," + viewTileCoords.Y + ")" + (featureText != "" ? " - " + featureText : "");
        overlayRenderTexture.Draw(text);
        text.Style = Text.Styles.Regular;

        text.Position = new Vector2f(text.Position.X, text.Position.Y + fontSize + offset);
        text.DisplayedString = "R " + viewTile.getColor().R +
                             "\nG " + viewTile.getColor().G +
                             "\nG " + viewTile.getColor().B +
                             "\nA " + viewTile.getColor().A;

        overlayRenderTexture.Draw(text);
    }
    */
}

