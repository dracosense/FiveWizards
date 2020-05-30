using Godot;
using System;
using static Lib;

public class PlayerCamera : Camera
{

    private RayCast mRay;
    private Root root;
    private Vector3 mPos;
    private Vector2 mVPos;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        mRay = (RayCast)GetNode("../../CameraRay");
    }

    public override void _PhysicsProcess(float delta)
    {
        mRay.Translate(this.GlobalTransform.origin - mRay.GlobalTransform.origin);
        mVPos = GetViewport().GetMousePosition();
        mPos = ProjectRayNormal(mVPos);
        mRay.Enabled = true;
        mRay.CastTo = mPos * RAY_LENGTH;
        root.mousePos = mRay.GetCollisionPoint();
    }

}
