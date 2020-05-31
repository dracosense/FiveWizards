using Godot;
using System;
using static Lib;

public class NecromancerArrow : Arrow
{

    public override void _Ready()
    {
        base._Ready();
        damage = NECROMANCER_ARROW_DAMAGE;
        wizard = NECROMANCER_WIZARD;
    }

}
