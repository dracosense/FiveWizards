using Godot;
using System;
using static Lib;

public class FriendUnit : Unit
{

    public Vector3 targetPos;

    protected Vector3 genPos;
    
    public override void SetType(uint t)
    {
        base.SetType(t);
        health *= FRIEND_H_CONST;
        damage *= FRIEND_D_CONST;
        speed = FRIEND_SPEED;
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
        SetType(wizardUnits[root.playerWizard, 0]);
        //this.GlobalTransform = new Transform(this.GlobalTransform.basis, Vector3.Inf);
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 v;
        EnemyUnit e = null;
        //
        if (effects[HEALTH_E].sTime + EFFECT_TIME < root.time && !root.playerInBattle && root.playerAtHome) // ??
        {
            effects[HEALTH_E].sTime = root.time;
            effects[HEALTH_E].param = FRIEND_HEALTH_E;
        }
        //
        if (type == wizardUnits[SPIRIT_WIZARD, 0] && root.friendUnitsNum >= BASE_FRIENDS_NUM) // spirit
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
        if (e != null)
        {
            targetPos = e.GlobalTransform.origin;
        }
        v = Vec3ToVec2(targetPos - this.GlobalTransform.origin);
        if (v.Length() < UNIT_TARGET_DIST)
        {
            SetAnim("idle");
        }
        else
        {
            if (root.friendUnitsMode == FOLLOW_PLAYER_M && v.Length() >= FRIEND_UNIT_TELEPORT_DIST) // || aRoom != unit room
            {
                genPos = 2.0f * FRIEND_UNIT_MAX_GEN_DIST * (new Vector3((float)(root.rand.NextDouble() - 0.5f), 0.0f, (float)(root.rand.NextDouble() - 0.5f)));
                this.GlobalTransform = new Transform(this.GlobalTransform.basis, root.playerPos + genPos);
            }
            MoveOn(v);
            SetAnim("move");
        }
    }

}
