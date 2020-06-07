using Godot;
using System;
using static Lib;

public class GUI : Control
{

    protected Root root;
    protected ProgressBar healthBar;
    protected ProgressBar magicEBar; 
    protected ProgressBar unitMagicEBar;
    protected ProgressBar bossHealthBar;
    protected TextureProgress crystalsBar;
    protected TextureProgress clock;
    protected TextureRect orangeFog;
    protected Button createUnitB;
    protected Button destroyUnitsB;
    protected Label unitsMode;
    protected Label message;
    protected Label timeLabel;

    public void SetMessage(string name, Color c)
    {
        message.Text = name;
        message.Modulate = c;
    }

    public override void _Ready()
    {
        Color c;
        root = (Root)GetNode("/root/root");
        healthBar = (ProgressBar)GetNode("HealthBar");
        magicEBar = (ProgressBar)GetNode("MagicEBar");
        unitMagicEBar = (ProgressBar)GetNode("UnitMagicEBar");
        bossHealthBar = (ProgressBar)GetNode("BossHealthBar");
        crystalsBar = (TextureProgress)GetNode("CrystalsBar");
        clock = (TextureProgress)GetNode("Clock");
        orangeFog = (TextureRect)GetNode("OrangeFog");
        createUnitB = (Button)GetNode("create_unit");
        destroyUnitsB = (Button)GetNode("destroy_units");
        unitsMode = (Label)GetNode("UnitsMode");
        message = (Label)GetNode("Message");
        timeLabel = (Label)GetNode("Time");
        c = message.Modulate;
        c.a = 0.0f;
        message.Modulate = c;
    }

    public override void _Process(float delta)
    {
        Boss boss;
        Color c;
        string s;
        float x = 0.0f;
        if (root.player != null)
        {
            healthBar.Value = healthBar.MaxValue * (root.player.GetHealth() / root.player.GetMaxHealth());
        }
        else
        {
            healthBar.Value = 0.0f;
        }
        if (MAX_MAGIC_E > 0.0f)
        {
            magicEBar.Value = magicEBar.MaxValue * (root.magicE / MAX_MAGIC_E);
            switch (root.playerWizard)
            {
                case ELEMENTAL_WIZARD:
                    x = ELEMENTAL_MAGIC_E;
                    break;
                case SPIRIT_WIZARD:
                    x = SPIRIT_MAGIC_E;
                    break;
                default:
                    x = 0.0f;
                    break;
            }
            unitMagicEBar.Value = unitMagicEBar.MaxValue * (x / MAX_MAGIC_E);
        }

        if (root.aBoss != null && root.aBoss.GetRef() != null)
        {
            bossHealthBar.Visible = true;
            boss = root.aBoss.GetRef() as Boss;
            bossHealthBar.Value = bossHealthBar.MaxValue * ((boss == null)?1.0f:(boss.GetHealth() / boss.GetMaxHealth()));
        }
        else
        {
            bossHealthBar.Visible = false;
        }

        crystalsBar.Value = Mathf.RoundToInt((float)(crystalsBar.MaxValue * root.winCrystals) / (float)WIZARDS_NUM);

        clock.Value = clock.MaxValue * (root.clockP / CLOCK_PERIOD);

        orangeFog.Visible = (root.clockSector == FOG_CLOCK_SECTOR);

        createUnitB.Visible = (root.playerWizard == SPIRIT_WIZARD || root.playerWizard == ELEMENTAL_WIZARD);

        destroyUnitsB.Visible = (root.playerWizard == SPIRIT_WIZARD);

        s = "UNITS MODE: ";
        if (root.setFriendsTarget)
        {
            s += "SET DEFEND POINT";
        }
        else
        {
            switch (root.friendUnitsMode)
            {
                case FOLLOW_PLAYER_M:
                    s += "FOLLOW WIZARD";
                    break;
                case DEFEND_POINT_M:
                    s += "DEFEND POINT"; 
                    break;
                case ATTACK_M:
                    s += "ATTACK";
                    break;
                default:
                    break;
            }
        }
        unitsMode.Text = s;

        c = message.Modulate;
        c.a = Mathf.Max(c.a - delta * POPUP_CHANGE_A_SPEED, 0.0f);
        message.Modulate = c;

        timeLabel.Text = "TIME: " + ((int)root.time).ToString();
    }

}
