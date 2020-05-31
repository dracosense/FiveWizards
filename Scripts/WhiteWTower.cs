using Godot;
using System;
using static Lib;

public class WhiteWTower : StaticBody
{

    protected Root root;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        this.Rotation = new Vector3(0.0f, (root.rand.Next() % 4) * (Mathf.Pi / 2.0f), 0.0f);
    }

}
