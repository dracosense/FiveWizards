using Godot;
using System;

public class ActionButton : GUIButton
{
    
    public override void _on_button_down()
    {
        Input.ActionPress(this.Name);
        base._on_button_down();
    }

    public override void _on_button_up()
    {
        Input.ActionRelease(this.Name);
        base._on_button_up();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        this.SelfModulate = (Input.IsActionPressed(this.Name)?Colors.DarkGray:Colors.White);
    }

}
