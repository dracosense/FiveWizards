using Godot;
using System;
using static Lib;

public class BlueWTower : KinematicBody
{

    protected Root root;
    protected Area attackArea;
    protected Vector3 move;
    protected float timeFromAttack;
    protected float attackTimeout = 1.0f;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        attackArea = (Area)GetNode("AttackArea");
        move = BLUE_W_TOWER_SPEED * RandAngleV(root.rand);
        timeFromAttack = 0.0f;
    }

    public override void _Process(float delta)
    {
        Unit unit = null;
        var enemies = attackArea.GetOverlappingBodies();
        timeFromAttack += delta;
        if (timeFromAttack >= attackTimeout && enemies != null && enemies.Count > 0)
        {
            unit = enemies[root.rand.Next() % enemies.Count] as Unit;
            if (unit != null)
            {
                unit.Damage(root.wizardClockConst[ELEMENTAL_WIZARD] * BLUE_W_TOWER_DAMAGE, -1, -1);
            }
            else
            {
                GD.Print("Blue wizard tower attack unit error.");
            }
            timeFromAttack = 0.0f;
        }
        MoveAndSlide(new Vector3(move.x, 0.0f, move.z));
        if (GetSlideCount() > 0)
        {
            KinematicCollision c = GetSlideCollision(0);
            move = -move.Rotated(c.Normal, Mathf.Pi);
            move.y = 0.0f;
        }
    }

}
