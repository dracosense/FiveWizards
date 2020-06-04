using Godot;
using System;
using static Lib;

public class Unit : KinematicBody
{

    public float speed = 4.0f;
    public float scale;
    public bool canAttack = true;

    protected Effect[] effects;
    protected AnimationPlayer aPlayer;
    protected MeshInstance model;
    protected MeshInstance healthBar;
    protected MeshInstance[] eIndicators;
    protected Spatial indicators;
    protected Spatial archPos;
    protected Area vRange;
    protected Area aRange;
    protected Root root;
    protected Vector3 spawnPos;
    protected float health;
    protected float maxHealth = 4.0f;
    protected float shield = 0.3f;
    protected float damage = 0.6f;
    protected float timeFromAttack;
    protected float attackTimeout = 1.6f;
    protected float timeFromDamage;
    protected float damagedTimeout = 0.1f;
    protected int type = -1;
    protected int wizard = -1;
    protected int  lastDFromW = -1; 
    protected bool damaged;
    protected bool randScaled = true;
    protected bool randRotated = true;
    protected bool attackUnitsInRange;

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void SetAnim(string s)
    {
        if (type >= 0)
        {
            aPlayer.Play(unitName[type] + '_' + s);
        }
    }

    public virtual void SetWCMask(KinematicBody body)
    {
        body.CollisionMask = 0;
        body.SetCollisionMaskBit(MAP_M_BIT, true);
    }

    public bool TryArch(Vector3 pos)
    {
        Arrow a = null;
        Vector2 v = Vec3ToVec2(pos - this.GlobalTransform.origin);
        if (timeFromAttack >= attackTimeout && type >= 0 && unitArrow[type] != null && archPos != null)
        {
            this.Rotation = new Vector3(0.0f, GetRealAngle(v.Angle()), 0.0f);
            a = root.CreateObj(unitArrow[type], archPos.GlobalTransform.origin) as Arrow;
            if (a  != null)
            {
                SetWCMask(a);
                a.speed = UNIT_ARROW_SPEED * ((pos - archPos.GlobalTransform.origin).Normalized());
                if (wizard >= 0)
                {
                    a.damage *= root.wizardClockConst[wizard];
                }
                timeFromAttack = 0.0f;
                return true;
            }
            else
            {
                GD.Print("Unit create arrow error.");
                a.QueueFree();
            }
        }
        return false;
    }

    public virtual void SetType(uint t, int _wizard)
    {
        type = (int)t;
        health = maxHealth = unitHealth[t];
        speed = unitSpeed[t];
        shield = unitShield[t];
        damage = unitDamage[t];
        wizard = _wizard;
    }

    public virtual void Attack(Unit unit)
    {
        unit.Damage(((wizard < 0)?1.0f:root.wizardClockConst[wizard]) * effects[ATTACK_E].GetPower() * damage, -1, -1);
    }

    public void Damage(float d, int e, int fromW)
    {
        float x = effects[DAMAGE_E].GetPower() * d;
        health = Mathf.Max(health - x + Mathf.Max(Mathf.Min(shield + effects[SHIELD_E].GetPower(), -x), 0.0f), 0.0f); //
        timeFromDamage = 0;
        model.MaterialOverride = damagedUnitM;
        damaged = true;
        if (d > 0.0f)
        {
            lastDFromW = fromW;
        }
        if (e >= 0)
        {
            effects[e] = new Effect(e, root.time, d);
        }
    }

    public void SetSpawnPos(Vector3 pos)
    {
        spawnPos = pos;
    }

    public void MoveOn(Vector2 v)
    {
        this.Rotation = new Vector3(0.0f, GetRealAngle(v.Angle()), 0.0f);
        MoveAndSlide(effects[SPEED_E].GetPower() * speed * ((new Vector3(v.x, 0.0f, v.y)).Normalized())); //
    }

    public virtual void Destroy()
    {
        PackedScene ps;
        Unit obj;
        uint x = wizardUnits[NECROMANCER_WIZARD, 0];
        bool b = (lastDFromW == root.playerWizard);
        if (b)
        {
            root.magicE = Mathf.Min(root.magicE + root.pBoosts[1] * UNIT_M_E_ADD, MAX_MAGIC_E); // add magic energy 
        }
        if (lastDFromW == NECROMANCER_WIZARD && type != x && root.rand.NextDouble() < SKELETON_GEN_CONST)
        {
            if (b)
            {
                ps = friendUnitPS;
            }
            else
            {
                ps = enemyUnitPS;
            }
            obj = root.CreateObj(ps, this.GlobalTransform.origin + MAP_CELL_SIZE * (new Vector3((float)(root.rand.NextDouble() - 0.5f), 0.0f, (float)(root.rand.NextDouble() - 0.5f)))) as Unit; // ?? change position ??
            if (obj != null)
            {
                if (b)
                {
                    ((FriendUnit)obj).SetType(x, NECROMANCER_WIZARD);
                }
                else
                {
                    obj.SetType(x, NECROMANCER_WIZARD);
                }
            }
            else
            {
                GD.Print("Error create unit");
            }
        }
        QueueFree();
    }

    public override void _Ready()
    {
        aPlayer = (AnimationPlayer)GetNode("APlayer");
        model = (MeshInstance)GetNode("Model");
        healthBar = (MeshInstance)GetNodeOrNull("Indicators/HealthBar"); 
        vRange = (Area)GetNodeOrNull("VisibilityRange");
        aRange = (Area)GetNodeOrNull("AttackRange");
        indicators = (Spatial)GetNodeOrNull("Indicators");
        archPos = (Spatial)GetNodeOrNull("ArchPos");
        root = (Root)GetNode("/root/root");
        spawnPos = new Vector3(this.GlobalTransform.origin);
        effects = new Effect[EFFECTS_NUM];
        eIndicators = new MeshInstance[EFFECTS_NUM];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new Effect(i, root.time - EFFECT_TIME, 0.0f);
            eIndicators[i] = (MeshInstance)GetNodeOrNull("Indicators/E" + i.ToString());
        }
        if (randScaled)
        {
            scale = 1.0f + (randScaled?(float)(root.rand.NextDouble() * UNIT_SCALE_CONST):0.0f);
            this.Scale = scale * new Vector3(1.0f, 1.0f, 1.0f);
            maxHealth *= scale; //
            damage *= scale; //
        }
        if (randRotated)
        {
            this.Rotation = ((float)(2.0f * Mathf.Pi * root.rand.NextDouble())) * new Vector3(0.0f, 1.0f, 0.0f);
        }
        timeFromAttack = 0;
        timeFromDamage = 0;
        health = maxHealth;
        damaged = false;
        attackUnitsInRange = true;
    }

    public override void _Process(float delta)
    {
        Unit unit;
        Vector3 v;
        int x = 0;
        v = this.GlobalTransform.origin;
        v.y = 0.0f; 
        attackUnitsInRange = (root.clockSector != -1);
        model.MaterialOverride = ((root.clockSector == -1)?unitOrangeFogM:unitM);
        this.GlobalTransform = new Transform(this.GlobalTransform.basis, v);
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].sTime + EFFECT_TIME <= root.time)
            {
                effects[i].param = 0.0f;
                if (eIndicators[i] != null)
                {
                    eIndicators[i].Visible = false;
                }
            }
            else
            {
                if (eIndicators[i] != null)
                {
                    eIndicators[i].Visible = true;
                }
            }
        }
        if (vRange != null)
        {
            vRange.Scale = effects[VISIBILITY_R_E].GetPower() * (new Vector3(1.0f, 1.0f, 1.0f)); //
        }
        health = Mathf.Max(Mathf.Min(health + delta * effects[HEALTH_E].GetPower(), maxHealth), 0.0f); //
        //
        if (canAttack && aRange != null)
        {
            var aRangeE = aRange.GetOverlappingBodies();
            x = root.rand.Next();
            if (timeFromAttack >= attackTimeout && aRangeE.Count > 0 && attackUnitsInRange) 
            {
                x %= aRangeE.Count;
                if (aRangeE[x] is Unit)
                {
                    unit = (Unit)aRangeE[x]; 
                    Attack(unit); //
                    if (unit.GetHealth() <= 0.0f && type == ENT_TYPE)
                    {
                        maxHealth += ENT_ADD_HEALTH;
                        health += ENT_ADD_HEALTH;
                    }
                    timeFromAttack = 0.0f;
                }
                else
                {
                    GD.Print("Unit attack error.");
                }
            }
        }
        // Vector3 v = this.GlobalTransform.origin;
        // this.GlobalTransform = new Transform(this.GlobalTransform.basis, new Vector3(v.x, 0.0f, v.z));
        if (timeFromDamage > damagedTimeout && damaged)
        {
            model.MaterialOverride = unitM;
            damaged = false;
        }
        if (healthBar != null)
        {
            healthBar.Scale = new Vector3(((maxHealth == 0.0f || health <= 0.0f)?0.0f:(health / maxHealth)), 1.0f, 1.0f);
        }
        if (indicators != null)
        {
            indicators.Rotation = -this.Rotation;
        }
        timeFromAttack += delta;
        timeFromDamage += delta;
        if (health <= 0)
        {
            Destroy();
        }
    }

}