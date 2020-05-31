using Godot;
using System;
using static Lib;

public class GreenWTower : WTower
{

    public override void _Ready()
    {
        base._Ready();
        weaponPS = healEArrowPS;
        weaponSpeed = ARROW_SPEED;
    }

}
