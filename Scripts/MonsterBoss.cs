using Godot;
using System;
using static Lib;

public class MonsterBoss : Boss
{

    public override void SetAnim(string s)
    {
        aPlayer.Play("monster_boss_idle");
    }

    public override void MoveOn(Vector2 v)
    {
        
    }

    public override void _Ready()
    {
        maxHealth = M_BOSS_MAX_HEALTH;
        shield = M_BOSS_SHIELD;
        speed = 0.0f;
        base._Ready();
    }

}
