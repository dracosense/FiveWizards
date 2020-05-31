using Godot;
using System;
using static Lib;

public class NecromancerEArrow : Arrow
{

    public override void _Ready()
    {
        base._Ready();
        wizard = NECROMANCER_WIZARD;
    }

}
