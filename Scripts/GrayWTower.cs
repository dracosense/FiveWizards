using Godot;
using System;
using static Lib;

public class GrayWTower : WTower
{

    public override void _Ready()
    {
        base._Ready();
        wizard = WAR_WIZARD;
    }
    
}
