using Godot;
using System;

public class FPS : Label
{

    public override void _Ready()
    {
        
    }


    public override void _Process(float delta)
    {
        Text = "FPS: " + Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
    }

}
