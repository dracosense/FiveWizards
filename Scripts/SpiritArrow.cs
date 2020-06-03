using Godot;
using System;
using static Lib;

public class SpiritArrow : Arrow
{

    public override void _Ready()
    {
        base._Ready();
        damage = SPIRIT_ARROW_DAMAGE;
    }

}
