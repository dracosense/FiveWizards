using Godot;
using System;
using static Lib;

public class PurpureWTower : WTower
{
    
    public override void _Ready()
    {
        base._Ready();
        weaponPS = necromancerEArrowPS;
        weaponSpeed = NECROMANCER_ARROW_SPEED;
    }

}
