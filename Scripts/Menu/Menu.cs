using Godot;
using System;
using static Lib;

public class Menu : Node
{

    protected Root root;
    protected Control mainPanel;
    protected Control winPanel;
    protected Control losePanel;
    protected Control loadingPanel;
    protected Control background;
    protected Button difficultButton;
    protected Button mapSizeButton;

    public void _on_play_button_up()
    {
        root.menuPanel = M_LOADING_PANEL;
    }

    public void _on_difficult_button_up()
    {
        root.NextDifficult();
    }

    public void _on_map_size_button_up()
    {
        root.NextMapSize();
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        mainPanel = (Control)GetNode("MainPanel");
        winPanel = (Control)GetNode("WinPanel");
        losePanel = (Control)GetNode("LosePanel");
        loadingPanel = (Control)GetNode("LoadingPanel");
        background = (Control)GetNode("Background");
        difficultButton = (Button)GetNode("MainPanel/Difficult/Button");
        mapSizeButton = (Button)GetNode("MainPanel/MapSize/Button");
    }

    public override void _Process(float delta)
    {
        mainPanel.Visible = (root.menuPanel == M_MAIN_PANEL);
        winPanel.Visible = (root.menuPanel == M_WIN_PANEL);
        losePanel.Visible = (root.menuPanel == M_LOSE_PANEL);
        loadingPanel.Visible = (root.menuPanel == M_LOADING_PANEL);
        background.Visible = (root.menuPanel != M_GAME);
        difficultButton.Text = difficultName[root.GetDifficult()];
        mapSizeButton.Text = mapSizeName[root.GetMapSize()];
        if (root.menuPanel == M_LOADING_PANEL)
        {
            root.StartGame();
        }
    }

}
