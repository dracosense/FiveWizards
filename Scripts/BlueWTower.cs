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
        float a = 0.0f;
        root = (Root)GetNode("/root/root");
        attackArea = (Area)GetNode("AttackArea");
        a = (float)(2.0f * Mathf.Pi * root.rand.NextDouble());
        move = new Vector3(Mathf.Cos(a), 0.0f, Mathf.Sin(a)) * BLUE_W_TOWER_SPEED;
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
                unit.Damage(BLUE_W_TOWER_DAMAGE, -1, -1);
            }
            else
            {
                GD.Print("Blue wizard tower attack unit error.");
            }
            timeFromAttack = 0.0f;
        }
        MoveAndSlide(new Vector3(move.x, 0.0f, move.y));
        if (GetSlideCount() > 0)
        {
            KinematicCollision c = GetSlideCollision(0);
            move = -move.Rotated(c.Normal, Mathf.Pi);
            move.y = 0.0f;
        }
    }

}
