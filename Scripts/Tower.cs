using Godot;
using System;
using static Lib;

public class Tower : StaticBody
{

    public float power; 
    public int type;
    public int effect;

    protected AnimationPlayer crystalAPlayer;
    protected MeshInstance model;
    protected MeshInstance crystal;
    protected Root root;

    public void RespawnSpell(Spell s, Vector3 normal, float angle, bool last = false)
    {
        if (s == null)
        {
            GD.Print("Respawn spell error.");
            return;
        }
        Spell a;
        if (last)
        {
            a = s;
        }
        else
        {
            if (s.copy >= MAX_SPELL_COPIES_NUM)
            {
                return;
            }
            a = (Spell)s.Duplicate();
            /* a = root.CreateSpell(s.GlobalTransform.origin);
            if (a == null)
            {
                return;
            }*/
            root.AddToGame(a);
            a.startTime = s.startTime;
        }
        a.copy = s.copy + 1;
        a.wizard = s.wizard;
        a.effect = effect;
        Vector3 v = normal.Rotated(new Vector3(0.0f, 1.0f, 0.0f), angle).Normalized();
        Vector3 arrowPos = TOWER_GEN_SPELL_DIST * v + this.GlobalTransform.origin;
        arrowPos.y = s.GlobalTransform.origin.y;
        float arrowSpeed = s.speed.Length();
        a.GlobalTransform = new Transform(s.GlobalTransform.basis, arrowPos);
        a.speed = arrowSpeed * v;
        a.damage *= power;
    }

    public void SpellCollision(Spell s, Vector3 normal)
    {
        if (type == SIDE_T_TYPE || type == ALL_T_TYPE)
        {
            RespawnSpell(s, normal, Mathf.Pi / 2.0f);
            RespawnSpell(s, normal, -Mathf.Pi / 2.0f, (type == SIDE_T_TYPE));
        }
        if (type == FRONT_T_TYPE || type == ALL_T_TYPE)
        {
            if (s.copy <= 0)
            {
                RespawnSpell(s, normal, 0.0f);
            }
            RespawnSpell(s, normal, Mathf.Pi, true);
        }
    }

    public void Init()
    {
        if (type >= 0 && model != null)
        {
            model.Mesh = towerTModels[type]; 
        }
        if (effect >= 0 && crystal != null)
        {
            crystal.MaterialOverride = eMaterials[effect];
        }
    }

    public override void _Ready()
    {
        crystalAPlayer = (AnimationPlayer)GetNode("CrystalAPlayer");
        model = (MeshInstance)GetNode("Model");
        crystal = (MeshInstance)GetNode("Crystal");
        root = (Root)GetNode("/root/root");
        crystalAPlayer.Play("crystal");
        type = -1;
        effect = -1;
        power = BASE_TOWER_POWER;
    }

}
