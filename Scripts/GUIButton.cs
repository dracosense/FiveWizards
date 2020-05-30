using Godot;
using System;
using static Lib;

public class GUIButton : Button
{

    private Root root;
    private int pressed;

    public virtual void _on_button_down()
    {
        int x = 0;
        root.guiInput++;
        root.friendUnitsMode = x;
    }

    public virtual void _on_button_up()
    {
        pressed++;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        pressed = 0;
    }

    public override void _Process(float delta)
    {
        root.guiInput -= pressed;
        pressed = 0;        
    }    

}
