using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class Root : Spatial
{

    public Player player;
    public Random rand = new Random();
    public Vector3 mPosS;
    public Vector3 mPosE;
    public Vector3 mousePos;
    public Vector3 playerPos;
    public Vector3 friendsTarget;
    public Vec2I pMapPos;
    public float time;
    public float magicE;
    public int guiInput;
    public int friendUnitsMode;
    public int friendUnitsNum;
    public uint playerWizard;
    public uint winCrystals;
    public bool setFriendsTarget;
    public bool playerInBattle;
    public bool playerAtHome;
    public bool followCursor;
    // public Queue<Spell> spellQueue;

    private MeshInstance mCursor;
    public Spatial game;

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
            FriendUnit unit = CreateObj(friendUnitPS, playerPos + MAP_CELL_SIZE *
             (new Vector3((float)(rand.NextDouble() - 0.5f), 0.0f, (float)(rand.NextDouble() - 0.5f)))) as FriendUnit;
            if (a)
            {
                unit.SetType(wizardUnits[ELEMENTAL_WIZARD, 0]);
                magicE -= ELEMENTAL_MAGIC_E;
            }
            else
            {
                unit.SetType(wizardUnits[SPIRIT_WIZARD, 0]);
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
        GD.Print("You win!");
        GetTree().Quit();
    }

    public override void _Ready()
    {
        // Spell s;
        mCursor = (MeshInstance)GetNode("/root/Game/MCursor");
        game = (Spatial)GetNode("/root/Game/");
        player = (Player)GetNode("/root/Game/Player");
        time = 0;
        guiInput = 0;
        friendUnitsMode = FOLLOW_PLAYER_M;
        playerInBattle = false;
        playerAtHome = false;
        followCursor = false;
        magicE = 0.0f;
        playerWizard = 0; // 
        friendUnitsNum = 0;
        winCrystals = 1;
        /*spellQueue = new Queue<Spell>();
        try
        {
            for (int i = 0; i < MAX_SPELL_NUM; i++)
            {
                s = (Spell)CreateObj(spellPS, INF * (new Vector3(1.0f, 1.0f, 1.0f)));
                s.active = false;
                spellQueue.Enqueue(s);
            }
        }
        catch
        {
            spellQueue.Clear();
            GD.Print("Create spells error.");
        }*/
    }

    public override void _PhysicsProcess(float delta)
    {
        time = ((float)OS.GetTicksMsec()) / 1000.0f;
        mCursor.Translation = mousePos;
    }

    public override void _Process(float delta)
    {
        CreatePUnit();
        RegeneratePlayer();
        if (winCrystals >= WIZARDS_NUM)
        {
            WinGame();
        }
        if (Input.IsActionJustPressed("exit"))
        {
            GetTree().Quit();
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
