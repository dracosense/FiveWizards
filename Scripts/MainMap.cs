using Godot;
using System;
using System.Collections.Generic;
using static GameMap;
using static Lib;

public class MainMap : GridMap
{

    private List<WeakRef> aRoomMonsters;

    private Root root;
    private Minimap minimap;
    private GameMap map;
    private Vec2I pMapPos;
    private Vec2I lastPMapPos;
    private int aRoom;
    private int lastARoom;
    private bool redrawMap;

    public void SetCell(Vec2I pos, int tile)
    {
        int x = new Basis(new Vector3(0.0f, 0.5f * Mathf.Pi * (root.rand.Next() % 4), 0.0f)).GetOrthogonalIndex();
        if (tile == EMPTY_TILE)
        {
            DelCell(pos);
            return;
        }
        if (tile == BLOCK_TILE)
        {
            SetCellItem(pos.x, 0, pos.y, tile, x);
        }
        else
        {
            SetCellItem(pos.x, 0, pos.y, tile);
        }
    }

    public void DelCell(Vec2I pos)
    {
        if (GetCell(pos) == EMPTY_TILE)
        {
            SetCellItem(pos.x, 0, pos.y, -1);
        }
    }

    public int GetCell(Vec2I pos)
    {
        return GetCellItem(pos.x, 0, pos.y);
    }

    public MapCell GetMapCell(Vec2I pos)
    {
        return map.GetCell(pos);
    }

    public bool GetRoomGenerated(int room)
    {
        if (room < 0 || room >= map.rooms.Count)
        {
            return true;
        }
        return map.rooms[room].generated;
    }

    public int GetRoomOwner(int room)
    {
        if (room < 0)
        {
            return -1;
        }
        return map.rooms[room].owner;
    }

    public void AddARoomMonster(Unit monster)
    {
        if (root.playerInBattle)
        {
            aRoomMonsters.Add(WeakRef(monster));
        }
    }

    public void GenMonsters(PackedScene ps, int r, int num, uint type, int wizard)
    {
        Room room = map.rooms[r];
        Unit unit;
        Vec2I rPos = room.pos;
        room.SetRand();
        for (int i = 0; i < num; i++)
        {
            Vec2I rRandPos = map.GetRandRFreePos(r);
            Vector3 wPos = MapToWorld(rPos.x + rRandPos.x, 0, rPos.y + rRandPos.y) +
            new Vector3((float)root.rand.NextDouble() - 0.5f, 0.0f, (float)root.rand.NextDouble() - 0.5f);
            wPos.y = 0.1f;
            unit = root.CreateObj(ps, wPos) as Unit;
            if (unit != null)
            {
                unit.SetType(type, wizard);
                aRoomMonsters.Add(WeakRef(unit));
            }
            else
            {
                GD.Print("Gen map monster error.");
            }
        }
    }

    public void CreateStruct(Vec2I pos, int s, int owner, int room)
    {
        PackedScene ps = null;
        Spatial obj;
        switch(s)
        {
            case CAMP_STRUCT:
                ps = campPS;
                break;
            case TOWER_STRUCT:
                if (owner >= 0 && room >= 0 && !map.rooms[room].bossRoom)
                {
                    ps = wizardTowers[owner];
                } 
                break;
            case ALTAR_STRUCT:
                    if (owner >= 0)
                    {
                        ps = altarPS; 
                    }
                break;
            default:
                break;
        }
        if (ps != null)
        {
            obj = root.CreateObj(ps, MapToWorld(pos.x, 0, pos.y));
            if (s == ALTAR_STRUCT && owner >= 0)
            {
                if (obj is Altar)
                {
                    ((Altar)obj).wizard = (uint)owner;
                    if (room >= 0)
                    {
                        ((Altar)obj).room = map.rooms[room];
                    }
                    else
                    {
                        ((Altar)obj).room = null;
                    }
                }
                else
                {
                    GD.Print("Set altar owner error.");
                }
            }
        }
    }

    public void DrawMap()
    {
        MapCell c;
        int x = 0;
        bool b = (pMapPos.x != lastPMapPos.x || pMapPos.y != lastPMapPos.y); 
        if (b || redrawMap)
        {
            if (b) // ??
            {
                for (int i = lastPMapPos.x - DRAW_MAP_DIST; i <= lastPMapPos.x + DRAW_MAP_DIST; i++)
                {
                    for (int j = lastPMapPos.y - DRAW_MAP_DIST; j <= lastPMapPos.y + DRAW_MAP_DIST; j++)
                    {
                        if (Mathf.Abs(i - pMapPos.x) > DRAW_MAP_DIST || Mathf.Abs(j - pMapPos.y) > DRAW_MAP_DIST)
                        {
                            DelCell(new Vec2I(i, j));
                            c = map.GetCell(new Vec2I(i, j));
                            if (c != null && c.tower != null)
                            {
                                c.tower.QueueFree();
                                c.tower = null;
                            }
                        }
                    }
                }
            }
            for (int i = pMapPos.x - DRAW_MAP_DIST; i <= pMapPos.x + DRAW_MAP_DIST; i++)
            {
                for (int j = pMapPos.y - DRAW_MAP_DIST; j <= pMapPos.y + DRAW_MAP_DIST; j++)
                {
                    c = map.GetCell(new Vec2I(i, j));
                    if (c != null && (c.tile != GetCell(new Vec2I(i, j)) || Mathf.Abs(i - lastPMapPos.x) > DRAW_MAP_DIST ||
                     Mathf.Abs(j - lastPMapPos.y) > DRAW_MAP_DIST || redrawMap))
                    {
                        if (c.room >= 0)
                        {
                            x = map.rooms[c.room].owner;
                        }
                        else
                        {
                            x = -1;
                        }
                        SetCell(new Vec2I(i, j), c.tile);
                        if (c.structure != NULL_STRUCT)
                        {
                            if (x != root.playerWizard)
                            {
                                CreateStruct(new Vec2I(i, j), c.structure, x, c.room);
                            }
                            c.structure = NULL_STRUCT;
                        }
                        if (c.tower_t >= 0 && c.tower == null && (c.room == aRoom || c.room == -1 || aRoom == -1))
                        {
                            map.map[i, j].tower = root.CreateTower(MapToWorld(i, 0, j) + this.GlobalTransform.origin, c.tower_t, c.tower_e, c.tower_p);
                        }
                    }
                }
            }
        }
        redrawMap = false;
    }

    public void SetRoomGenerated(int room)
    {
        root.playerInBattle = false;
        if (room >= 0)
        {
            map.rooms[room].generated = true;
            root.AddPBoost(true);
            /*if (room.bossRoom && room.owner != root.playerWizard)
            {
                root.winCrystals++;
            }*/
        }
    }

    public void InitMap()
    {
        map = new GameMap(new Vec2I((int)(mapSizeConst[root.GetDifficult()] * MAIN_MAP_SIZE.x), 
        (int)(mapSizeConst[root.GetDifficult()] * MAIN_MAP_SIZE.y)),
         new Vec2I(MAP_T_FLOOR_SIZE.x, MAP_T_FLOOR_SIZE.y), 
         WIZARDS_NUM, mapSizeTowerFloorsNum[root.GetDifficult()], 0);
        if (map.mainRooms != null && map.mainRooms.Count > 1) // ?? portal in main room ??
        {
            root.SetPPos(MapToWorld(map.mainRooms[0].pos.x, 0, map.mainRooms[0].pos.y));
            map.mainRooms[0].generated = true;
            if (map.mainRooms[0].owner >= 0)
            {
                root.playerWizard = (uint)map.mainRooms[0].owner;
            }
            else
            {
                GD.Print("Start room owner error.");
            }
        }
        else
        {
            GD.Print("Start room error.");
        }
        aRoom = -1;
        lastARoom = -1;
        redrawMap = true;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        minimap = (Minimap)GetNode("/root/Game/Minimap/Map");
        aRoomMonsters = new List<WeakRef>();
        pMapPos = new Vec2I();
        lastPMapPos = new Vec2I();
    }

    public override void _PhysicsProcess(float delta)
    {
        Room room;
        MapCell c;
        Vec2I v = new Vec2I();
        int x = 0;
        lastPMapPos = pMapPos;
        lastARoom = aRoom;
        pMapPos = Vec3ToI(WorldToMap(root.playerPos));
        root.pMapPos = pMapPos;
        c = map.GetCell(pMapPos);
        for (int i = 0; i < aRoomMonsters.Count;)
        {
            if (aRoomMonsters[i].GetRef() == null) 
            {
                if (i != aRoomMonsters.Count)
                {
                    aRoomMonsters[i] = aRoomMonsters[aRoomMonsters.Count - 1];
                }
                aRoomMonsters.RemoveAt(aRoomMonsters.Count - 1);
            }
            else
            {
                i++;
            }
        }
        if (aRoom != -1 && aRoomMonsters.Count == 0 && !map.rooms[aRoom].generated)
        {
            map.SetREntrances(aRoom, FLOOR_TILE);
            SetRoomGenerated(aRoom);
            redrawMap = true;
        }
        if (c != null)
        {
            aRoom = c.room;
            if (aRoom != -1 && map.rooms[aRoom].generated)
            {
                if (map.rooms[aRoom].owner == root.playerWizard)
                {
                    root.playerAtHome = true;
                }
                else
                {
                    root.playerAtHome = false;
                }
                if (c.teleport >= 0 && Input.IsActionJustPressed("action"))
                {
                    v = map.teleportTo[c.teleport];
                    root.pFloor = map.tToFloor[c.teleport];
                    root.SetPPos(MapToWorld(v.x, 0, v.y));
                }
            }
            else
            {
                root.playerAtHome = false;
            }
            if (aRoom != lastARoom)
            {
                if (lastARoom != -1 && !map.rooms[lastARoom].generated)
                {
                    map.SetREntrances(lastARoom, FLOOR_TILE);
                    SetRoomGenerated(lastARoom);
                    redrawMap = true;
                    for (int i = 0; i < aRoomMonsters.Count; i++)
                    {
                        if (aRoomMonsters[i] != null && !aRoomMonsters[i].IsQueuedForDeletion())
                        {
                            ((Unit)aRoomMonsters[i].GetRef()).QueueFree();
                        }
                    }
                    aRoomMonsters.Clear();
                }
                if (aRoom != -1 && !map.rooms[aRoom].generated)
                {
                    room = map.rooms[aRoom];
                    if (room.owner == root.playerWizard || room.owner < 0)
                    {
                        SetRoomGenerated(aRoom);
                        // ?? redraw map ?? 
                    }
                    else
                    {
                        map.SetREntrances(aRoom, BLOCK_TILE);
                        x = RoomGenMonstersNum((room.sizeDL.x + room.sizeUR.x) * (room.sizeDL.y + room.sizeUR.y), (uint)room.owner, root.GetDifficult(), root.rand); 
                        GenMonsters(enemyUnitPS, aRoom, x, wizardUnits[room.owner][0], room.owner);
                        if (room.bossRoom)
                        {
                            GenMonsters(((room.owner == MONSTER_WIZARD)?mBossPS:bossPS), aRoom, 1, 0, room.owner);
                            /*if (aRoomMonsters.Count == x + 1)
                            {
                                root.aBoss = WeakRef(aRoomMonsters[aRoomMonsters.Count - 1]);
                            }
                            else
                            {
                                GD.Print("Room monsters gen error.");
                            }*/
                        }
                        redrawMap = true;
                        root.playerInBattle = true;
                    }
                }
            }
        }
        if (redrawMap)
        {
            minimap.redrawMap = true;
        }
        DrawMap();
    }

}
