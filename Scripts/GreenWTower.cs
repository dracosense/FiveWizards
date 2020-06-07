using Godot;
using System;
using static Lib;

public class GreenWTower : WTower
{

    protected WeakRef quardian = null;
    protected float genUnitTimeout = 30.0f;
    protected float timeFromGenUnit;

    public override void _Ready()
    {
        base._Ready();
        wizard = NATURE_WIZARD;
        weaponPS = healEArrowPS;
        weaponSpeed = ARROW_SPEED;
        timeFromGenUnit = genUnitTimeout;
    }

    public override void _PhysicsProcess(float delta)
    {
        Vec2I v;
        base._PhysicsProcess(delta);
        timeFromGenUnit += delta;
        if ((quardian == null || quardian.GetRef() == null) && timeFromGenUnit >= genUnitTimeout)
        {
            v = D_WAYS[root.rand.Next() % D_NUM];
            Unit unit = root.CreateObj(enemyUnitPS, this.GlobalTransform.origin + 
            MAP_CELL_SIZE * new Vector3(v.x, 0.0f, v.y) + GenRandMCellPos(root.rand)) as Unit;
            if (unit != null)
            {
                unit.SetType(wizardUnits[NATURE_WIZARD][0], NATURE_WIZARD);
                quardian = WeakRef(unit);
                timeFromGenUnit = 0.0f;
            }
            else
            {
                GD.Print("Green wizard tower unit gen error.");
                unit.QueueFree();
            }
        }
    }

}
