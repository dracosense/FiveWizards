using Godot;
using System;
using static Lib;

public class WTower : Area
{

    protected Root root;
    protected Spatial archPos;
    protected PackedScene weaponPS = eArrowPS;
    protected float weaponSpeed = ARROW_SPEED;
    protected float timeFromAttack;
    protected float attackTimeout = 2.0f;
    protected int wizard = -1;

    public virtual void Attack(Unit unit)
    {
        Arrow arrow = root.CreateObj(weaponPS, archPos.GlobalTransform.origin) as Arrow;
        if (arrow != null)
        {
            arrow.speed = weaponSpeed * ((unit.GlobalTransform.origin + SHOOT_BASE_TRANSLATION - archPos.GlobalTransform.origin).Normalized());
            if (wizard >= 0) 
            {
                arrow.damage *= root.wizardClockConst[wizard];
            }
        }
        else
        {
            GD.Print("Create weapon error.");
        }
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        archPos = (Spatial)GetNode("ArchPos");
        timeFromAttack = 0.0f;
    }

    public override void _PhysicsProcess(float delta)
    {
        Unit unit = null;
        var enemies = GetOverlappingBodies();
        timeFromAttack += delta;
        if (timeFromAttack >= attackTimeout && enemies != null && enemies.Count > 0)
        {
            unit = enemies[root.rand.Next() % enemies.Count] as Unit;
            if (unit != null)
            {
                Attack(unit);
                timeFromAttack = 0;
            }
            else
            {
                GD.Print("Wizard tower attack unit error.");
            }
        }
    }

}
