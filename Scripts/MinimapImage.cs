using Godot;
using System;

public class MinimapImage : Control
{

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("hide_map"))
        {
            this.Visible = !this.Visible;
        }
    }

}
