using Godot;
using System;
using static Lib;

public class FriendUnit : Unit
{

    public Vector3 targetPos;

    protected Vector3 genPos;
    
    public override void SetType(uint t, int _wizard)
    {
        base.SetType(t, _wizard);
        health *= FRIEND_H_CONST;
        damage *= FRIEND_D_CONST;
        speed = FRIEND_SPEED;
    }

    public override void SetWCMask(KinematicBody body)
    {
        base.SetWCMask(body);
        body.SetCollisionMaskBit(ENEMY_M_BIT, true);
    }

    public override void Destroy()
    {
        root.friendUnitsNum--;
        base.Destroy();
    }

    public override void _Ready()
    {
        base._Ready();
        root.friendUnitsNum++;
        this.Scale = 1.2f * (new Vector3(1.0f, 1.0f, 1.0f));
        SetType(wizardUnits[root.playerWizard][0], (int)root.playerWizard);
        //this.GlobalTransform = new Transform(this.GlobalTransform.basis, Vector3.Inf);
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 v;
        EnemyUnit e = null;
        float x = 0.0f;
        if (type >= 0)
        {
            damage = root.pBoosts[4] * unitDamage[type];
            shield = root.pBoosts[5] * unitShield[type]; 
        }
        speed = (root.playerInBattle?1.0f:PLAYER_RUN_SPEED_C) * FRIEND_SPEED;
        //
        if (effects[HEALTH_E].sTime + EFFECT_TIME < root.time && !root.playerInBattle && root.playerAtHome) // ??
        {
            effects[HEALTH_E].sTime = root.time;
            effects[HEALTH_E].param = FRIEND_HEALTH_E;
        }
        //
        if (type == wizardUnits[SPIRIT_WIZARD][0] && root.friendUnitsNum > BASE_FRIENDS_NUM) // spirit
        {
            root.magicE = Mathf.Max(root.magicE - SPIRIT_TIME_COST * delta, 0.0f);
            if (Input.IsActionJustPressed("destroy_units") || root.magicE <= 0.0f)
            {
                lastDFromW = -1;
                Destroy();
            }
        }
        if (vRange != null)
        {
            var enemies = vRange.GetOverlappingBodies();
            if (enemies.Count > 0)
            {
                e = enemies[0] as EnemyUnit;
            }
        }
        switch (root.friendUnitsMode)
        {
            case FOLLOW_PLAYER_M:
                targetPos = root.playerPos + genPos;
                break;
            case DEFEND_POINT_M:
                targetPos = root.friendsTarget + genPos; 
                break;
            case ATTACK_M:
                targetPos = this.GlobalTransform.origin;
                break;
            default:
                break;
        }
        if (root.clockSector == FOG_CLOCK_SECTOR) // orange fog
        {
            targetPos = root.playerPos;
        }
        else
        {
            if (e != null)
            {
                x = (e.GlobalTransform.origin - this.GlobalTransform.origin).Length();
                if (x < UNIT_TARGET_DIST || x > archDist || !IsArcher())
                {
                    targetPos = e.GlobalTransform.origin;
                }
                else
                {
                    TryArch(e.GlobalTransform.origin + SHOOT_BASE_TRANSLATION);
                }
            }
        }
        v = Vec3ToVec2(targetPos - this.GlobalTransform.origin);
        if (root.clockSector == FOG_CLOCK_SECTOR)
        {
            if (timeFromAttack >= attackTimeout && v.Length() < attackDist)
            {
                Attack(root.player);
                timeFromAttack = 0.0f;
            }
            /*if (v.Length() < archDist) // ?? arch to player ?? (change arrow paramss)
            {
                TryArch(targetPos + SHOOT_BASE_TRANSLATION);
            }*/
        }
        if (v.Length() < UNIT_TARGET_DIST)
        {
            SetAnim("idle");
        }
        else
        {
            if ((root.friendUnitsMode == FOLLOW_PLAYER_M || root.clockSector == FOG_CLOCK_SECTOR) && v.Length() >= FRIEND_UNIT_TELEPORT_DIST) // || aRoom != unit room
            {
                genPos = 2.0f * FRIEND_UNIT_MAX_GEN_DIST * GenRandMCellPos(root.rand) / MAP_CELL_SIZE;
                this.GlobalTransform = new Transform(this.GlobalTransform.basis, root.playerPos + genPos);
            }
            MoveOn(v);
            SetAnim("move");
        }
    }

}
