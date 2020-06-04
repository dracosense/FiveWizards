using Godot;
using System;
using static Lib;
using static GameMap;

public class Altar : Area
{

    public Room room;
    public uint wizard = 0;
    public bool crystal = true;

    protected Root root;
    protected MeshInstance model;
    protected MeshInstance crystalObj;
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
        model = (MeshInstance)GetNode("Model");
        crystalObj = (MeshInstance)GetNode("Crystal");
        active = 0;
    }

    public override void _Process(float delta)
    {
        model.MaterialOverride = crystalObj.MaterialOverride = wizardPMaterials[wizard];
        crystalObj.Visible = crystal;
        if (active > 0 && room != null && room.generated && Input.IsActionJustPressed("Action"))
        {
            if (crystal)
            {
                root.winCrystals++;
                crystal = false;
            }
            else
            {
                if (root.winCrystals > 0)
                {
                    root.winCrystals--;
                    crystal = true;
                }
            }
        }
    }

}
