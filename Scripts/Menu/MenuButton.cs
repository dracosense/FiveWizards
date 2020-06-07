      using Godot;
using System;
using static Lib;

public class MenuButton : Button
{

    protected Root root;
    protected int num;

    public void _on_button_up()
    {
        root.menuPanel = num;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        if (this.Name.Length > 0 && this.Name[0] >= '0' && this.Name[0] <= '9')
        {
            num = ((int)this.Name[0]) - '0';
        }
        else
        {
            num = M_MAIN_PANEL;
        }
    }

}
