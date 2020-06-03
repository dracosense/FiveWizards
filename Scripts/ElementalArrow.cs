using Godot;
using System;
using static Lib;

public class ElementalArrow : Arrow
{

    public override void _Ready()
    {
        base._Ready();
        damage = ELEMENTAL_ARROW_DAMAGE;
    }

}
