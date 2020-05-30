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

    public void GenMonsters(int r, int num, uint type)
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
            unit = (Unit)root.CreateObj(enemyUnitPS, wPos);
            unit.SetType(type);
            aRoomMonsters.Add(WeakRef(unit));
        }
    }

    public void CreateStruct(Vec2I pos, int s)
    {
        PackedScene ps = null;
        switch(s)
        {
            case CAMP_STRUCT:
                ps = campPS;
                break;
            case TOWER_STRUCT:
                //ps = wizardTowers[]; 
                break;
            default:
                break;
        }
        if (ps != null)
        {
            root.CreateObj(ps, MapToWorld(pos.x, 0, pos.y));
        }
    }

    public void DrawMap()
    {
        MapCell c;
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
                        SetCell(new Vec2I(i, j), c.tile);
                        if (c.structure != CREATED_STRUCT)
                        {
                            CreateStruct(new Vec2I(i, j), c.structure);
                            c.structure = CREATED_STRUCT;
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

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        minimap = (Minimap)GetNode("/root/Game/Minimap/Map");
        map = new GameMap(new Vec2I(MAIN_MAP_SIZE.x, MAIN_MAP_SIZE.y),
         new Vec2I(MAP_T_FLOOR_SIZE.x, MAP_T_FLOOR_SIZE.y), WIZARDS_NUM, MAP_TOWER_FLOORS_NUM, 0);
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
        pMapPos = new Vec2I();
        lastPMapPos = new Vec2I();
        aRoom = -1;
        lastARoom = -1;
        redrawMap = true;
        aRoomMonsters = new List<WeakRef>();
    }

    public override void _PhysicsProcess(float delta)
    {
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
            if (aRoomMonsters[i].GetRef() == null) // not work
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
            map.rooms[aRoom].generated = true;
            redrawMap = true;
            root.playerInBattle = false;
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
                if (c.teleport >= 0 && Input.IsActionJustPressed("teleport"))
                {
                    v = map.teleportTo[c.teleport];
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
                    map.rooms[lastARoom].generated = true;
                    redrawMap = true;
                    root.playerInBattle = false;
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
                    if (map.rooms[aRoom].owner == root.playerWizard || map.rooms[aRoom].owner < 0)
                    {
                        map.rooms[aRoom].generated = true;
                        // redrawMap = true // ??
                    }
                    else
                    {
                        map.SetREntrances(aRoom, BLOCK_TILE);
                        x = root.rand.Next()  % (ROOM_MONSTERS_NUM.y - ROOM_MONSTERS_NUM.x) + ROOM_MONSTERS_NUM.x; 
                        GenMonsters(aRoom, x, wizardUnits[map.rooms[aRoom].owner, 0]);
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
