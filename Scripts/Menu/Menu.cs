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
    protected AudioStreamPlayer menuMusic;
    protected AudioStreamPlayer gameMusic;
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
        menuMusic = (AudioStreamPlayer)GetNode("MenuMusic");
        gameMusic = (AudioStreamPlayer)GetNode("GameMusic");
        difficultButton = (Button)GetNode("MainPanel/Difficult/Button");
        mapSizeButton = (Button)GetNode("MainPanel/MapSize/Button");
        menuMusic.Play();
    }

    public override void _Process(float delta)
    {
        float x = 0.0f;
        mainPanel.Visible = (root.menuPanel == M_MAIN_PANEL);
        winPanel.Visible = (root.menuPanel == M_WIN_PANEL);
        losePanel.Visible = (root.menuPanel == M_LOSE_PANEL);
        loadingPanel.Visible = (root.menuPanel == M_LOADING_PANEL);
        background.Visible = (root.menuPanel != M_GAME);
        difficultButton.Text = difficultName[root.GetDifficult()];
        mapSizeButton.Text = mapSizeName[root.GetMapSize()];
        if (root.menuPanel == M_LOADING_PANEL)
        {
            menuMusic.Play();
            menuMusic.StreamPaused = true;
            gameMusic.Play();
            root.StartGame();
        }
        gameMusic.StreamPaused = !(menuMusic.StreamPaused = (root.menuPanel == M_GAME));
        x = CHANGE_VOLUME_C * delta;
        if (Input.IsActionPressed("increase_music_volume"))
        {
            menuMusic.VolumeDb += x;
            gameMusic.VolumeDb += x;
        }   
        if (Input.IsActionPressed("decrease_music_volume"))
        {
            menuMusic.VolumeDb -= x;
            gameMusic.VolumeDb -= x;
        }
    }

}
