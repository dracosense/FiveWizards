using Godot;
using System;
using static Lib;

public class GUI : Control
{

    protected Root root;
    protected ProgressBar healthBar;
    protected ProgressBar magicEBar; 
    protected ProgressBar unitMagicEBar;
    protected Label unitsMode;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        healthBar = (ProgressBar)GetNode("HealthBar");
        magicEBar = (ProgressBar)GetNode("MagicEBar");
        unitMagicEBar = (ProgressBar)GetNode("UnitMagicEBar");
        unitsMode = (Label)GetNode("UnitsMode");
    }

    public override void _Process(float delta)
    {
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
        s = "UNITS MODE: ";
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
        unitsMode.Text = s;
    }

}
