using Godot;
using System;

public class Joystick : GUIButton
{

    protected TextureRect point;
    protected string actionUp = "game_up";
    protected string actionDown = "game_down";
    protected string actionLeft = "game_left";
    protected string actionRight = "game_right";
    protected bool active;

    public override void _on_button_down()
    {
        active = true;
        base._on_button_down();
    }

    public override void _on_button_up()
    {
        active = false;
        Input.ActionRelease(actionUp);
        Input.ActionRelease(actionDown);
        Input.ActionRelease(actionLeft);
        Input.ActionRelease(actionRight);
        base._on_button_up();
    }

    public override void _Ready()
    {
        base._Ready();
        point = (TextureRect)GetNode("Point");
        active = false;
    }

    public override void _Process(float delta)
    {
        Vector2 mPos = GetLocalMousePosition();
        Vector2 pPos;
        mPos.x /= this.RectSize.x;
        mPos.y /= this.RectSize.y;
        mPos -= 0.5f * new Vector2(1.0f, 1.0f);
        mPos *= 2.0f;
        pPos = ((mPos.Length() > 1.0f)?(mPos.Normalized()):mPos) / 2.0f;
        pPos.x *= this.RectSize.x;
        pPos.y *= this.RectSize.y;
        point.RectPosition = (this.RectSize - point.RectSize) / 2.0f;
        if (active)
        {
            point.RectPosition += pPos;
        }
        if (active)
        {
            Input.ActionPress(actionUp, -mPos.y);
            Input.ActionPress(actionDown, mPos.y);
            Input.ActionPress(actionLeft, -mPos.x);
            Input.ActionPress(actionRight, mPos.x);
        }
        base._Process(delta);
    }

}
