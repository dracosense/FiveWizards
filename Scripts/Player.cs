using Godot;
using System;
using static Lib;

public class Player : Unit
{

    protected CollisionShape collider;
    protected OmniLight mainLight;
    protected Spatial cameraObj;
    protected Spatial aPosObj; // attack position object
    protected Vector2 move;
    protected float angle;
    protected float lastAngle;
    protected bool absMove;

    public void SetRot(Vector3 rotation)
    {
        //angle = rotation.y;
        collider.Rotation = model.Rotation = rotation;
    }

    public void SetRot(float angle)
    {
        SetRot(new Vector3(0.0f, GetRealAngle(angle), 0.0f));
    }

    public void AttackOnPos(Vector3 pos)
    {
        if (timeFromAttack >= attackTimeout)
        {
            Vector3 v = (pos - this.GlobalTransform.origin);
            SetRot(AngleByVec3(v));
            Spell s = (Spell)root.CreateObj(spellPS, aPosObj.GlobalTransform.origin);
            // Spell s = root.CreateSpell(aPosObj.GlobalTransform.origin);
            if (s != null)
            {
                s.speed = SPELL_SPEED * (new Vector3(v.x, 0.0f, v.z).Normalized());
                s.wizard = (int)root.playerWizard;
            }
            else
            {
                GD.Print("Can't cast spell.");
            }
            timeFromAttack = 0.0f;
        }
    }

    public override void Destroy()
    {
        GetTree().Quit();
    }

    public override void _Ready()
    {
        maxHealth = 30.0f;
        speed = 8.0f;
        shield = 0.2f;
        randScaled = false;
        randRotated = false;
        base._Ready();
        collider = (CollisionShape)GetNode("Collider");
        mainLight = (OmniLight)GetNode("Model/MainLight");
        cameraObj = (Spatial)GetNode("CameraObj");
        aPosObj = (Spatial)GetNode("Model/APosObj");
        angle = 0.0f;
        lastAngle = 0.0f;
        absMove = true;
        attackTimeout = 0.5f;
    }

    public override void _PhysicsProcess(float delta) // light scale
    {
        float x = 0.0f, y = 0.0f;
        root.playerPos = this.GlobalTransform.origin;
        move = Vector2.Zero;
        //
        mainLight.OmniRange = effects[VISIBILITY_R_E].GetPower() * MAIN_P_LIGHT_BASE_R;
        //
        model.MaterialOverride = wizardPMaterials[root.playerWizard];
        if (absMove)
        {
            move = new Vector2(Input.GetActionStrength("game_left") - Input.GetActionStrength("game_right"),
            Input.GetActionStrength("game_up") - Input.GetActionStrength("game_down"));
        }
        else
        {
            if ((root.mousePos - root.playerPos).Length() >= MIN_GO_TO_POINT_DIST)
            {
                x = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
                y = Input.GetActionStrength("game_up") - Input.GetActionStrength("game_down");
                move = new Vector2(Mathf.Cos(angle) * y - Mathf.Sin(angle) * x, Mathf.Sin(angle) * y + Mathf.Cos(angle) * x);
            }
        }
        move = move.Normalized();
        if (Input.IsActionJustPressed("abs_move"))
        {
            absMove = !absMove;
        }
        if (Input.IsActionJustReleased("arrow_attack") && root.guiInput <= 0)
        {
            if (root.setFriendsTarget)
            {
                root.friendsTarget = new Vector3(root.mousePos.x, 0.0f, root.mousePos.z);
                root.friendUnitsMode = DEFEND_POINT_M;
                root.setFriendsTarget = false;
            }
            else
            {
                AttackOnPos(root.mousePos);
            }
        }
        if (Input.IsActionJustPressed("attack_up"))
        {
            AttackOnPos(this.GlobalTransform.origin + new Vector3(0.0f, 0.0f, 1.0f));
        }
        if (Input.IsActionJustPressed("attack_down"))
        {
            AttackOnPos(this.GlobalTransform.origin + new Vector3(0.0f, 0.0f, -1.0f));
        }
        if (Input.IsActionJustPressed("attack_left"))
        {
            AttackOnPos(this.GlobalTransform.origin + new Vector3(1.0f, 0.0f, 0.0f));
        }
        if (Input.IsActionJustPressed("attack_right"))
        {
            AttackOnPos(this.GlobalTransform.origin + new Vector3(-1.0f, 0.0f, 0.0f));
        }
        angle = AngleByVec3(root.mousePos - this.GlobalTransform.origin);
        if (angle != lastAngle)
        {
            SetRot(angle);
            lastAngle = angle;
            //cameraObj.Rotation = new Vector3(0.0f, GetRealAngle(angle), 0.0f);
        }
        if (move != Vector2.Zero)
        {
            aPlayer.Play("wizard_move");
            SetRot(move.Angle());
            MoveAndSlide(new Vector3(speed * move.x, 0.0f, speed * move.y));
        }
        else
        {
            aPlayer.Play("wizard_idle");
        }
    }

}
