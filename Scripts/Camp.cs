using Godot;
using System;
using static Lib;

public class Camp : Area
{

    protected Root root;

    public void _on_body_entered(Spatial body)
    {
        PackedScene ps;
        Unit obj;
        uint x = wizardUnits[WAR_WIZARD][0];
        QueueFree();
        if (CAMP_UNITS_NUM.x <= 0 || CAMP_UNITS_NUM.y - CAMP_UNITS_NUM.x <= 0)
        {
            return;
        }
        uint y = (uint)(root.rand.Next() % (CAMP_UNITS_NUM.y - CAMP_UNITS_NUM.x) + CAMP_UNITS_NUM.x);
        for (int i = 0; i < y; i++)
        {
            if (root.playerWizard == WAR_WIZARD)
            {
                ps = friendUnitPS;
            }
            else
            {
                ps = enemyUnitPS;
            }
            obj = root.CreateObj(ps, 2 * GenRandMCellPos(root.rand) + this.GlobalTransform.origin) as Unit;
            if (obj == null)
            {
                GD.Print("Create unit from camp error.");
                obj.QueueFree(); 
                return;
            }
            if (root.playerWizard == WAR_WIZARD)
            {
                ((FriendUnit)obj).SetType(wizardUnits[WAR_WIZARD][0], WAR_WIZARD);
            }
            else
            {
                obj.SetType(wizardUnits[WAR_WIZARD][0], WAR_WIZARD);
            }
        }
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
    }

}
