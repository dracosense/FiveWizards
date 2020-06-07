using Godot;
using System;
using static Lib;

public class EnemyUnit : Unit
{

    public Player target;

    public bool CanAttackPlayer()
    {
        return ((Vec3ToVec2((root.playerPos - this.GlobalTransform.origin))).Length() <= attackDist);
    }

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
                if (!CanAttackPlayer()) 
                {
                    if (v.Length() > effects[VISIBILITY_R_E].GetPower() * archDist || !IsArcher())
                    {
                        MoveOn(v);
                        SetAnim("move");
                    }
                    else
                    {
                        TryArch(target.GlobalTransform.origin + SHOOT_BASE_TRANSLATION);
                    }
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
        if (timeFromAttack >= attackTimeout && CanAttackPlayer())
        {
            Attack(root.player);
            timeFromAttack = 0.0f;
        }
        base._Process(delta);
    }

}
