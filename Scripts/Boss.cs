using Godot;
using System;
using static Lib;

public class Boss : Unit
{

    public override void SetAnim(string s)
    {
        aPlayer.Play("wizard_" + s);
    }

    public override void SetType(uint t, int _wizard) 
    {
        wizard = _wizard;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if (wizard >= 0)
        {
            model.MaterialOverride = wizardPMaterials[wizard];
        }
    }

}
