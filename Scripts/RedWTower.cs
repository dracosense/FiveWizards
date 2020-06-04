using Godot;
using System;
using static Lib;

public class RedWTower : Area
{

    protected Root root;
    protected float createUTimeout = 10.0f;
    protected float timeFromCreateUnit;
    protected int active;

    public void _on_body_entered(Spatial body)
    {
        active++;
    }

    public void _on_body_exited(Spatial body)
    {
        active--;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        timeFromCreateUnit = 0.0f;
        active = 0;
    }

    public override void _PhysicsProcess(float delta)
    {
        Unit unit;
        timeFromCreateUnit += delta;
        if (active > 0 && timeFromCreateUnit > createUTimeout)
        {    
            unit = root.CreateObj(enemyUnitPS, this.GlobalTransform.origin + MAP_CELL_SIZE * new Vector3((float)(root.rand.NextDouble() - 0.5f), 0.0f, (float)(root.rand.NextDouble() - 0.5f))) as Unit;
            if (unit != null)
            {
                unit.SetType(wizardUnits[MONSTER_WIZARD, 0], MONSTER_WIZARD);
            }
            else
            {
                GD.Print("Red tower create unit error.");
            }
            timeFromCreateUnit = 0.0f;
        }
    }

}
