using SFML.Window;

namespace IS_Compressions.code.logic.tools;
internal interface Tool
{
    void Update()
    {
    }
    void OnKeyRelease(object sender, KeyEventArgs e)
    {
    }
    void OnKeyPress(object sender, KeyEventArgs e)
    {
    }
    void OnMouseMove(object sender, MouseMoveEventArgs e)
    {
    }
    void OnMouseScroll(object sender, MouseWheelScrollEventArgs e)
    {
    }
    void OnMousePress(object sender, MouseButtonEventArgs e)
    {
    }
    void OnMouseRelease(object sender, MouseButtonEventArgs e)
    {
    }
}
