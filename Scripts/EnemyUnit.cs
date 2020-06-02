using Godot;
using System;
using static Lib;

public class EnemyUnit : Unit
{

    public Player target;

    public override void SetWCMask(KinematicBody body)
    {
        base.SetWCMask(body);
        body.SetCollisionMaskBit(PLAYER_M_BIT, true);
        body.SetCollisionMaskBit(FRIEND_M_BIT, true);
    }

    public override void _Ready()
    {
        base._Ready();
        target = root.player; 
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 v;
        if (target != null)
        {
            v = Vec3ToVec2(target.GlobalTransform.origin - this.GlobalTransform.origin);
            if (v.Length() <= effects[VISIBILITY_R_E].GetPower() * ENEMY_V_RANGE)
            {
                if (v.Length() > effects[VISIBILITY_R_E].GetPower() * ENEMY_ARCH_DIST || !TryArch(target.GlobalTransform.origin + SHOOT_BASE_TRANSLATION))
                {
                    MoveOn(v);
                    SetAnim("move");
                }
                return;
            }
        }
        v = Vec3ToVec2(spawnPos - this.GlobalTransform.origin);
        if (v.Length() >= UNIT_RETURN_POINT_DIST)
        {
            MoveOn(v);
            SetAnim("move");
        }
        else
        {
            SetAnim("idle");
        }
    }

    public override void _Process(float delta)
    {
        if (timeFromAttack >= attackTimeout && (Vec3ToVec2((root.playerPos - this.GlobalTransform.origin))).Length() <= ENEMY_ATTACK_PLAYER_DIST)
        {
            root.player.Damage(damage, -1, -1);
            timeFromAttack = 0.0f;
        }
        base._Process(delta);
    }

}
