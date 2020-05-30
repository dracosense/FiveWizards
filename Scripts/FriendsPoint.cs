using Godot;
using System;
using static Lib;

public class FriendsPoint : Spatial
{

    private Root root;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
    }

    public override void _PhysicsProcess(float delta)
    {
        this.GlobalTransform = new Transform(this.GlobalTransform.basis, root.friendsTarget);
        this.Visible = (root.friendUnitsMode == DEFEND_POINT_M);
    }

}
