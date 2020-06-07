using Godot;
using System;
using static Lib;

public class Boss : EnemyUnit
{

    public float abilityTimeout = 6.0f;
    public float timeFromAbility;

    public override void SetAnim(string s)
    {
        aPlayer.Play("wizard_" + s);
    }

    public override void SetType(uint t, int _wizard) 
    {
        wizard = _wizard;
        arrowWizard = wizard;
        arrowEffect = wizard;
        speed = BOSS_SPEED;
        if (wizard == ELEMENTAL_WIZARD)
        {
            speed *= 1.5f;
        }
    }

    public override void Destroy()
    {
        Unit unit;
        if (wizard == WAR_WIZARD)
        {
            unit = root.CreateObj(enemyUnitPS, this.GlobalTransform.origin + GenRandMCellPos(root.rand)) as Unit;
            if (unit != null)
            {
                unit.SetType(wizardUnits[WAR_WIZARD][0], WAR_WIZARD); //
                root.AddARoomMonster(unit);
            }
            else
            {
                GD.Print("Boss create unit on destroy error.");
            }
        }
        base.Destroy();
    }

    public override void SetMaterial()
    {
        if (wizard >= 0)
        {
            model.MaterialOverride = wizardPMaterials[wizard];
        }
        else
        {
            model.MaterialOverride = unitM;
        }
    }

    public override void _Ready()
    {
        maxHealth = BOSS_MAX_HEALTH;
        shield = BOSS_SHIELD;
        attackDist = 0.0f;
        archDist = BOSS_ARCH_DIST;
        weaponPS = eSpellPS;
        base._Ready();
        orangeFogChangeM = false;
        timeFromAbility = abilityTimeout;
        root.aBoss = WeakRef(this);
    }

    public override void _PhysicsProcess(float delta)
    {
        Unit unit;
        timeFromAbility += delta;
        base._PhysicsProcess(delta);
        if (wizard == ELEMENTAL_WIZARD && WIZARDS_NUM > 0)
        {
            arrowEffect = root.rand.Next() % WIZARDS_NUM;
        }
        if (timeFromAbility >= abilityTimeout)
        {
            switch(wizard)
            {
                case MONSTER_WIZARD:
                    for (int i = 0; i < M_BOSS_RAND_SPELLS_NUM; i++)
                    {
                        TryArch(this.GlobalTransform.origin + RandAngleV(root.rand), false);
                    }
                    break;
                case NATURE_WIZARD:
                    health += 2.0f;
                    break;
                case ELEMENTAL_WIZARD: // random effects + speed
                    break;
                case WAR_WIZARD: // on destroy
                    break;
                case NECROMANCER_WIZARD:
                    unit = root.CreateObj(enemyUnitPS, this.GlobalTransform.origin + GenRandMCellPos(root.rand)) as Unit;
                    if (unit != null)
                    {
                        unit.SetType(wizardUnits[NECROMANCER_WIZARD][0], NECROMANCER_WIZARD);
                    }
                    else
                    {
                        GD.Print("Boss create unit error.");
                    }
                    break;
                case SPIRIT_WIZARD:
                    this.Visible = !this.Visible;
                    break;                    
                default:
                    break;
            }
            timeFromAbility = 0.0f;
        }
    }

}
