using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class Root : Spatial
{

    public float[] wizardClockConst;
    public float[] pBoosts;

    public Player player;
    public Random rand = new Random();
    public WeakRef aBoss;
    public Vector3 mPosS;
    public Vector3 mPosE;
    public Vector3 mousePos;
    public Vector3 playerPos;
    public Vector3 friendsTarget;
    public Vec2I pMapPos;
    public Vec2I pFloor;
    public float time;
    public float magicE;
    public float clockP;
    public int guiInput;
    public int friendUnitsMode;
    public int friendUnitsNum;
    public int clockSector;
    public int menuPanel;
    public uint playerWizard;
    public uint winCrystals;
    public bool setFriendsTarget;
    public bool playerInBattle;
    public bool playerAtHome;
    public bool followCursor;

    // public Queue<Spell> spellQueue;

    private MeshInstance mCursor;
    private Spatial game;
    private MainMap mainMap;
    private GUI gui;
    private uint difficult = 1;
    private uint mapSize = 1;

    public void AddToGame(Spatial a)
    {
        game.AddChild(a);
    }

    public Spatial CreateObj(PackedScene ps, Vector3 pos)
    {
        try
        {
            Spatial obj = (Spatial)ps.Instance();
            obj.Translation = pos;
            AddToGame(obj);
            return obj;
        }
        catch
        {
            GD.Print("Can't create object.");
            return null;
        }
    }

    public Tower CreateTower(Vector3 pos, int type, int effect, float power)
    {
        Tower tower = (Tower)CreateObj(towerPS, pos);
        tower.type = type;
        tower.effect = effect;
        tower.power = power;
        tower.Init();
        return tower;
    }

    /*public Spell CreateSpell(Vector3 pos)
    {
        Spell s;
        if (spellQueue.Count <= 0)
        {
            return null;
        }
        s = spellQueue.Dequeue();
        s.Init();
        s.startTime = time;
        s.GlobalTransform = new Transform(new Basis(Vector3.Zero), pos);
        return s;
    }

    public void ReleaseSpell(Spell s)
    {
        GD.Print(s.ToString());
        s.active = false;
        s.Visible = false;
        s.GlobalTransform = new Transform(new Basis(Vector3.Zero), INF * (new Vector3(1.0f, .0f, 1.0f)));
        spellQueue.Enqueue(s);
    }*/

    public void AddPBoost(bool positive)
    {
        int x = 0;
        if (PLAYER_BOOST_NUM > 0)
        {
            x = rand.Next() % PLAYER_BOOST_NUM;
            pBoosts[x] = Mathf.Max(pBoosts[x] + (positive?P_POSITIVE_BOOST_C:P_NEGATIVE_BOOST_C), 0.1f);
            gui.SetMessage(pBoostName[x], (positive?Colors.Green:Colors.Red));
        }
    }

    public void SetPPos(Vector3 pos)
    {
        player.GlobalTransform = new Transform(player.GlobalTransform.basis, pos);
        playerPos = pos;
    }

    public void CreatePUnit()
    {
        bool a = ((playerWizard == ELEMENTAL_WIZARD) && (magicE >= ELEMENTAL_MAGIC_E));
        bool b = ((playerWizard == SPIRIT_WIZARD) && (magicE >= SPIRIT_MAGIC_E));
        if (Input.IsActionJustPressed("create_unit") && (a || b))
        {
            FriendUnit unit = CreateObj(friendUnitPS, playerPos + GenRandMCellPos(rand)) as FriendUnit;
            if (a)
            {
                unit.SetType(wizardUnits[ELEMENTAL_WIZARD][0], ELEMENTAL_WIZARD);
                magicE -= ELEMENTAL_MAGIC_E;
            }
            else
            {
                unit.SetType(wizardUnits[SPIRIT_WIZARD][0], SPIRIT_WIZARD);
                magicE -= SPIRIT_MAGIC_E;
            }
        }
    }

    public void RegeneratePlayer()
    {
        if (Input.IsActionJustPressed("regenerate_player") && magicE >= R_PLAYER_M_E)
        {
            magicE -= R_PLAYER_M_E;
            player.Damage(-R_PLAYER_HEALTH, -1, -1);
        }
    }

    public void WinGame()
    {
        menuPanel = M_WIN_PANEL; 
        EndGame();
        //GD.Print("You win!");
        //GetTree().Quit();
    }

    public void LoseGame()
    {
        menuPanel = M_LOSE_PANEL;
        EndGame();
    }

    public void AddARoomMonster(Unit monster)
    {
        mainMap.AddARoomMonster(monster);
    }

    public void MoveClockP(float delta)
    {
        int x = 0;
        float y = 0.0f;
        float z = 0.0f;
        clockP += delta * CLOCK_P_SPEED;
        x = (int)(clockP / CLOCK_PERIOD);
        for (int i = 0; i < x; i++)
        {
            AddPBoost(false);    
        }
        clockP -= x * CLOCK_PERIOD;
        clockSector = clockSectors[(int)(clockP / C_SECTOR_SIZE)];
        for (int i = 0; i < WIZARDS_NUM; i++)
        {
            y = clockP - wizardClockPos[i];
            if (y < 0.0f)
            {
                y += CLOCK_PERIOD;
            }
            else
            {
                z += CLOCK_PERIOD;
            }
            wizardClockConst[i] = Mathf.Max(Mathf.Min(y, CLOCK_PERIOD - y) - C_SECTOR_SIZE, 0.0f);
            if (CLOCK_PERIOD > 2.0f * C_SECTOR_SIZE)
            {
                wizardClockConst[i] /= (CLOCK_PERIOD / 2.0f - C_SECTOR_SIZE);
            }
            wizardClockConst[i] = 1.0f + WIZARD_C_C_LEN / 2.0f - WIZARD_C_C_LEN * wizardClockConst[i];
        }
    }

    public void NextDifficult()
    {
        difficult++;
        difficult %= DIFFICULTS_NUM;
    }

    public void NextMapSize()
    {
        mapSize++;
        mapSize %= MAP_SIZES_NUM;
    }

    public uint GetDifficult()
    {
        return difficult;
    }

    public uint GetMapSize()
    {
        return mapSize;
    }

    public void EndGame()
    {
        Node node = GetNodeOrNull("/root/Game");
        if (node != null)
        {
            node.Free();
        }
        if (menuPanel == M_GAME)
        {
            menuPanel = M_MAIN_PANEL;
        }
        /*node = GetNodeOrNull("/root/Menu");
        if (node != null)
        {
            return;
        }
        GetTree().Root.AddChild(menuPS.Instance());*/
    }

    public void StartGame()
    {
        FriendUnit friend;
        /*Node node = GetNodeOrNull("/root/Menu");
        if (node != null)
        {
            node.Free();
        }*/
        Node node = GetNodeOrNull("/root/Game");
        if (node != null)
        {
            node.Free();
        }
        GetTree().Root.AddChild(mainPS.Instance());
        menuPanel = M_GAME;
        mCursor = (MeshInstance)GetNode("/root/Game/MCursor");
        game = (Spatial)GetNode("/root/Game/");
        gui = (GUI)GetNode("/root/Game/GUILayer/GUI");
        mainMap = (MainMap)GetNode("/root/Game/MainMap");
        player = (Player)GetNode("/root/Game/Player");
        guiInput = 0;
        friendUnitsMode = FOLLOW_PLAYER_M;
        playerInBattle = false;
        playerAtHome = false;
        followCursor = false;
        magicE = 0.0f;
        playerWizard = 0; // 
        friendUnitsNum = 0;
        winCrystals = 1;
        clockP = 0.0f;
        clockSector = clockSectors[0];
        pFloor = new Vec2I(0, 0);
        mainMap.InitMap();
        for (int i = 0; i < BASE_FRIENDS_NUM; i++)
        {
            friend = CreateObj(friendUnitPS, playerPos + GenRandMCellPos(rand)) as FriendUnit;
            if (friend != null)
            {
                friend.SetType(wizardUnits[playerWizard][0], (int)playerWizard);
            }
        }
        for (int i = 0; i < WIZARDS_NUM; i++)
        {
            wizardClockConst[i] = 1.0f;
        }
        for (int i = 0; i < PLAYER_BOOST_NUM; i++)
        {
            pBoosts[i] = 1.0f;
        }
    }

    public override void _Ready()
    {
        wizardClockConst = new float[WIZARDS_NUM];
        pBoosts = new float[PLAYER_BOOST_NUM];
        time = 0;
        menuPanel = M_MAIN_PANEL;   
    }

    public override void _PhysicsProcess(float delta)
    {
        time = ((float)OS.GetTicksMsec()) / 1000.0f;
        if (Input.IsActionJustPressed("exit"))
        {
            switch (menuPanel)
            {
                case M_GAME:
                    EndGame();
                    break;
                case M_MAIN_PANEL:
                    GetTree().Quit();
                    break;
                case M_WIN_PANEL:
                    menuPanel = M_MAIN_PANEL;
                    break;
                case M_LOSE_PANEL:
                    menuPanel = M_MAIN_PANEL;
                    break;
                default:
                    break;
            }
        }
        if (menuPanel == M_GAME)
        {
            MoveClockP(delta);
            CreatePUnit();
            RegeneratePlayer();
            mCursor.Translation = mousePos;
            if (winCrystals >= WIZARDS_NUM)
            {
                WinGame();
            }
            if (Input.IsActionJustPressed("follow_cursor"))
            {
                followCursor = !followCursor;
                if (!followCursor)
                {
                    Input.ActionRelease("game_up");
                }
            }
            if (followCursor)
            {
                Input.ActionPress("game_up");
            }
            if (Input.IsActionJustPressed("friends_follow_p_mode"))
            {
                friendUnitsMode = FOLLOW_PLAYER_M;
            }
            if (Input.IsActionJustPressed("friends_defend_point_mode"))
            {
                setFriendsTarget = true;
            }
            if (Input.IsActionJustPressed("friends_attack_mode"))
            {
                friendUnitsMode = ATTACK_M;
            }
        }
    }

}
