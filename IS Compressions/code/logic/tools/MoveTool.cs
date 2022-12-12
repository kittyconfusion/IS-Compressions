using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IS_Compressions.code.core;
using IS_Compressions.code.display;
using SFML.System;
using SFML.Window;

namespace IS_Compressions.code.logic.tools;
internal class MoveTool : Tool
{
    readonly double maxClickLength = 0.2;
    readonly double moveSpeed = 50;

    int clickX, clickY;
    int recentDragX, recentDragY;
    bool up, down, left, right;
    bool shift, control;

    float clickTime;
    bool drag;
    Clock clock = new Clock();

    public void Update()
    {
        Move();
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

    }
    public void OnMouseMove(object sender, MouseMoveEventArgs e)
    {
        if (drag)
        {
            var newX = e.X;
            var newY = e.Y;

            Camera.MoveCamera(-(recentDragX - newX), -(recentDragY - newY));

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
                //OnClick((int)(clickX * ((double)Display.settings.startScreenWidth / getWindowWidth())), (int)(clickY * ((double)Display.settings.startScreenHeight / getWindowHeight())));
            }
        }
    }

    public void OnMouseScroll(object sender, MouseWheelScrollEventArgs e)
    {

    }
    public void Move()
    {
        if (!drag)
        {
            var effectiveMoveSpeed = moveSpeed * TimeKeeper.deltaTime;

            if (shift) { effectiveMoveSpeed *= 3; }
            if (control) { effectiveMoveSpeed /= 3; }

            float x = 0, y = 0;

            if (up && !down) { y = (float)effectiveMoveSpeed; }
            else if (down && !up) { y = (float)-effectiveMoveSpeed; }
            if (left && !right) { x = (float)effectiveMoveSpeed; }
            else if (right && !left) { x = (float)-effectiveMoveSpeed; }

            DisplayManager.layers.MoveAmount(Display.currentSelectedLayer,(int)(x + 0.5), (int)(y + 0.5));
        }
    }
}
