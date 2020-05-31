using Godot;
using System;
using static Lib;

public class HealArrow : Arrow
{

    public override void _Ready()
    {
        base._Ready();    
        damage = H_ARROW_DAMAGE;
    }

}
