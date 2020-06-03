using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class GameMap
{

    public class MapCell
    {

        public Tower tower; 
        public float tower_p;
        public int tower_t;
        public int tower_e;
        public int tile;
        public int room;
        public int teleport;
        public int structure;

        public void ClearTower()
        {
            tower_t = -1;
            tower_e = -1;
            tower_p = BASE_TOWER_POWER;
        }

        public MapCell()
        {
            tower = null;
            room = -1;
            tile = EMPTY_TILE;
            teleport = -1;
            structure = NULL_STRUCT;
            ClearTower();
        }

    }

    public class Room
    {

        public static int GLOBAL_ID = 0;

        public List<Vec2I> entrances;
        public Random rand;
        public Vec2I sizeUR;
        public Vec2I sizeDL;
        public Vec2I pos;
        public int id;
        public int owner;
        public int randSeed;
        public bool generated;
        public bool bossRoom;

        public Room()
        {
            id = GLOBAL_ID;
            GLOBAL_ID++;
            rand = new Random();
            randSeed = rand.Next();
            rand = new Random(randSeed);
            generated = false;
            bossRoom = false;
            owner = -1;
            sizeUR = new Vec2I();
            sizeDL = new Vec2I();
            entrances = new List<Vec2I>();
        }

        public void SetRand()
        {
            rand = new Random(randSeed);
        }

        public Vec2I GetRandPos()
        {
            Vec2I ans = new Vec2I();
            if (sizeUR.x + sizeDL.x + 1 <= 0)
            {
                ans.x = 0;
            }
            else
            {
                ans.x = rand.Next() % (sizeUR.x + sizeDL.x + 1) - sizeDL.x;
            }
            if (sizeUR.y + sizeDL.y + 1 <= 0)
            {
                ans.y = 0;
            }
            else
            {
                ans.y = rand.Next() % (sizeUR.y + sizeDL.y + 1) - sizeDL.y;
            }
            return ans;
        }

    }

    public MapCell[,] map;
    public List<Room> rooms;
    public List<Room> mainRooms;
    public Vec2I[] teleports;
    public Vec2I[] teleportTo;
    public int[]  towerOwners;
    public Vec2I size;
    public int type;

    private Random rand;

    public GameMap(Vec2I mainSize, Vec2I towerSize, int towersNum, int towerHeight, int playerTower) // main floor + towers for each teleport with towerHeight floors
    {
        int[] owners = null;
        Vec2I floorsNum = new Vec2I();
        Vec2I genPos = new Vec2I();
        int x = 0;
        int y = 0;
        bool b = false;
        rand = new Random();
        size = new Vec2I();
        teleports = new Vec2I[(2 * towerHeight + 1) * towersNum];
        teleportTo = new Vec2I[(2 * towerHeight + 1) * towersNum]; // 
        rooms = new List<Room>();
        mainRooms = new List<Room>();
        if (mainSize.x <= 0 || mainSize.x <= 0 || towersNum <= 0 || towerHeight < 0 ||
         (towersNum != 0 && (towerHeight == 0 || towerSize.x <= 0 || towerSize.y <= 0 ||
          towerSize.x > mainSize.x || towerSize.y > mainSize.y)))
        {
            GD.Print("Init map error.");
            return;
        }
        size = mainSize;
        //
        floorsNum.y = (int)((float)(mainSize.y - towerSize.y) / (float)(towerSize.y + MAP_FLOOR_DIST)) + 1;
        floorsNum.x = (towersNum * towerHeight) / floorsNum.y + (((towersNum * towerHeight) % floorsNum.y == 0)?0:1); 
        size.x += floorsNum.x * (towerSize.x + MAP_FLOOR_DIST);
        //
        map = new MapCell[size.x, size.y];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                map[i, j] = new MapCell();
            }
        }
        towerOwners = new int[towersNum];
        for (int i = 0; i < towerOwners.Length; i++)
        {
            towerOwners[i] = i;
        }
        for (int i = 0; i < RANDOM_SHUFFLE_CONST * towerOwners.Length; i++) // random shuffle
        {
            for (int j = 1; j < towerOwners.Length; j++)
            {
                if (rand.Next() % 2  == 0)
                {
                    x = towerOwners[j];
                    towerOwners[j] = towerOwners[j - 1];
                    towerOwners[j - 1] = x;
                }
            }
        }
        if (playerTower >= 0 && playerTower < towersNum && towersNum > 1) // ?? more random ??
        {
            if (towerOwners[playerTower] == MONSTER_WIZARD)
            {
                y = ((playerTower > 0)?(playerTower - 1):(playerTower + 1));
                towerOwners[playerTower] = towerOwners[y];
                towerOwners[y] = MONSTER_WIZARD;
            }
        }
        GenMap(new Vec2I(0, 0), mainSize, new Vec2I(11, 11), new Vec2I(15, 15), 
        new Vec2I(2, 2), new Vec2I(5, 5), new Vec2I(0, towersNum), towerOwners, towerOwners[playerTower]); // main floor
        //
        x = 0;
        for (int j = 0; j < floorsNum.y; j++)
        {
            for (int i = 0; i < floorsNum.x && j * floorsNum.x + i < towersNum * towerHeight; i++)
            {
                y = towerOwners[(j * floorsNum.x + i) % towersNum];
                owners = new int[]{y, y};
                genPos.x = mainSize.x + MAP_FLOOR_DIST + (i * towerSize.x + MAP_FLOOR_DIST);
                genPos.y = j * (towerSize.y + MAP_FLOOR_DIST);
                b = (j * floorsNum.x + i < towersNum * (towerHeight - 1));
                GenMap(genPos, genPos + towerSize, new Vec2I(11, 11), new Vec2I(15, 15), 
                new Vec2I(2, 2), new Vec2I(5, 5), new Vec2I(towersNum + x, towersNum + x + 2), owners); // up and down, for end - only down
                x += 2;
                if (!b)
                {
                    if (mainRooms.Count == x + towersNum)
                    {
                        mainRooms[towersNum + x - 1].bossRoom = true;
                        map[teleports[towersNum + x - 1].x, teleports[towersNum + x - 1].y].teleport = -1;
                    }
                    else
                    {
                        GD.Print("Boss room generation error");
                    }
                }
            }
        }
        if (towersNum > 0)
        {
            for (int i = 0; i < towersNum; i++) // ?? optimize ??
            {
                teleportTo[i] = teleports[towersNum + (towerHeight == 1?i:(2 * i))];
            }
            for (int i = 0; i < towerHeight; i++)
            {
                for (int j = 0; j < towersNum; j++)
                {
                    x = (2 * i + 1) * towersNum;
                    teleportTo[x + 2 * j] = teleports[(i == 0)?j:((x - 2 * towersNum) + 2 * j + 1)];
                    if (i < towerHeight - 1)
                    {
                        teleportTo[x + 2 * j + 1] = teleports[x + 2 * (towersNum + j)];// + ((i == towerHeight - 2)?1:2) * j];
                    }
                }
            }
            /*for (int i = 0; i < towersNum; i++)
            {
                teleportTo[i + teleports.Length - towersNum] = teleports[2 * i + teleports.Length - 3 * towersNum + 1]; 
            }*/
        }
        //
    }
    
    public Vec2I GetRandRFreePos(int r)
    {
        Room room;
        Vec2I ans = new Vec2I();
        if (r < 0 || r >= rooms.Count)
        {
            return ans;
        }
        room = rooms[r];
        for (int i = 0; i < MAX_GEN_RAND_R_POS_NUM; i++)
        {
            ans = room.GetRandPos();
            if (map[room.pos.x + ans.x, room.pos.y + ans.y].tower_t == -1)
            {
                return ans;
            }
        }
        return ans;
    }

    public Vec2I GetSNMP(Vec2I v, Vec2I[,] m)
    {
        if (v == m[v.x, v.y])
        {
            return v;
        }
        return m[v.x, v.y] = GetSNMP(m[v.x, v.y], m);
    }

    public Vec2I SNMMerge(Vec2I v, Vec2I u, Vec2I[,] m)
    {
        v = GetSNMP(v, m);
        u = GetSNMP(u, m);
        if (u == v)
        {
            return v;
        }
        if (rand.Next() % 2 == 0)
        {
            m[u.x, u.y] = v;
            return v;
        }
        m[v.x, v.y] = u;
        return u;
    }

    public void AddRWay(Vec2I v, int way, int[,] r, int[,,] ways)
    {
        ways[v.x, v.y, way] = 1;
        ways[v.x + D_WAYS[way].x, v.y + D_WAYS[way].y, (way + D_NUM / 2) % D_NUM] = 1;
        r[v.x, v.y]++;
        r[v.x + D_WAYS[way].x, v.y + D_WAYS[way].y]++;
    } 

    public void GenRoom(Vec2I pos, Vec2I sizeUR, Vec2I sizeDL)
    {
        Room room = new Room();
        MapCell cell;
        Vec2I rBegin = new Vec2I();
        Vec2I t;
        room.pos = pos;
        room.sizeUR = sizeUR;
        room.sizeDL = sizeDL;
        rBegin.x = room.pos.x - room.sizeDL.x;
        rBegin.y = room.pos.y - room.sizeDL.y;
        for (int xi = rBegin.x; xi <= room.pos.x + room.sizeUR.x; xi++)
        {
            for (int yi = rBegin.y; yi <= room.pos.y + room.sizeUR.y; yi++)
            {
                if (TOWER_TYPES_NUM > 0 && (xi - rBegin.x) % TOWER_GEN_CONST == TOWER_GEN_CONST - 1 &&
                 (yi - rBegin.y) % TOWER_GEN_CONST == TOWER_GEN_CONST - 1 && rand.NextDouble() < T_RAND_GEN_CONST)
                {
                    map[xi, yi].tower_t = rand.Next() % TOWER_TYPES_NUM;
                    if (EFFECTS_NUM > 0)
                    {
                        map[xi, yi].tower_e = rand.Next() % EFFECTS_NUM;
                    }
                    else
                    {
                        map[xi, yi].tower_e = -1;
                    }
                    map[xi, yi].tower_p = BASE_TOWER_POWER;
                }
                map[xi, yi].tile = FLOOR_TILE;
                map[xi, yi].room = rooms.Count;
            }
        }
        for (int p = 0; p < D_NUM; p++)
        {
            t = new Vec2I(room.pos.x, room.pos.y);
            if (D_WAYS[p].x != 0)
            {
                if (D_WAYS[p].x > 0)
                {
                    t.x += room.sizeUR.x + 1;
                }
                else
                {
                    t.x -= room.sizeDL.x + 1;
                }
            }
            else
            {
                if (D_WAYS[p].y > 0)
                {
                    t.y += room.sizeUR.y + 1;
                }
                else
                {
                    t.y -= room.sizeDL.y + 1;
                }
            }
            for (int xi = -1; xi <= 1; xi++)
            {
                cell = GetCell(new Vec2I(t.x + xi * D_WAYS[p].y, t.y + xi * D_WAYS[p].x));
                if (cell != null && cell.tile == FLOOR_TILE)
                {
                    room.entrances.Add(t + new Vec2I(xi * D_WAYS[p].y, xi * D_WAYS[p].x));
                }
            }
        }
        rooms.Add(room);
    }

    public void GenMap(Vec2I begin, Vec2I end, Vec2I minRDist, 
    Vec2I maxRDist, Vec2I minRSize, Vec2I maxRSize, Vec2I mainR, int[] mROwners, int player = -1)
    {
        List<Vec2I> rList = new List<Vec2I>();
        List<int> rcx = new List<int>();
        List<int> rcy = new List<int>();
        List<int> mRooms;
        Queue<Vec2I> setOwners = new Queue<Vec2I>();
        Room[,] rPositions;
        Vec2I[,] rParent;
        int[,] rOwners;
        int[,] r;
        int[,,] rWays; // -2 - go outside of build zone, -1 - blocked, 0 - free, 1 - tunnel
        MapCell cell;
        Vec2I rNum = new Vec2I();
        Vec2I s = new Vec2I(end.x - begin.x, end.y - begin.y);
        Vec2I v;
        Vec2I u;
        int vNum = 0;
        int k = 0;
        int x = 0;
        bool b = false;
        if (begin.x >= end.x || begin.y >= end.y || begin.x < 0 || begin.y < 0 || end.x > size.x || end.y > size.y ||
         maxRDist.x <= minRDist.x || maxRDist.y <= minRDist.y || maxRSize.x <= minRSize.x || 
         maxRSize.y <= minRSize.y || mROwners == null || mROwners.Length < mainR.y - mainR.x)
        {
            GD.Print("Gen floor error.");
            return;
        }

        // set blocks on all area
        for (int i = begin.x; i < end.x; i++)
        {
            for (int j = begin.y; j < end.y; j++)
            {
                map[i, j].tile = BLOCK_TILE;
            }
        }

        // choose room positions
        for (int i = begin.x + maxRSize.x; i < end.x - maxRSize.x;)
        {
            rcx.Add(i);
            i += minRDist.x + rand.Next() % (maxRDist.x - minRDist.x); //
        }
        for (int i = begin.y + maxRSize.y; i < end.y - maxRSize.y;)
        {
            rcy.Add(i);
            i += minRDist.y + rand.Next() % (maxRDist.y - minRDist.y); //
        }
        rNum.x = rcx.Count;
        rNum.y = rcy.Count;
        r = new int[rNum.x, rNum.y];
        rParent = new Vec2I[rNum.x, rNum.y];
        rWays = new int[rNum.x, rNum.y, D_NUM];
        rOwners = new int[rNum.x, rNum.y];
        rPositions = new Room[rNum.x, rNum.y];

        // create room centers
        for (int i = 0; i < rNum.x; i++)
        {
            for (int j = 0; j < rNum.y; j++)
            {
                r[i, j] = 0;
                rParent[i, j] = new Vec2I(i, j);
                rList.Add(new Vec2I(i, j));
                rOwners[i, j] = -1;
                for (int p = 0; p < D_NUM; p++)
                {
                    if (i + D_WAYS[p].x >= rNum.x || j + D_WAYS[p].y >= rNum.y || i + D_WAYS[p].x < 0 || j + D_WAYS[p].y < 0)
                    {
                        rWays[i, j, p] = -2;
                    }
                    else
                    {
                        rWays[i, j, p] = -1;
                    }
                }
                for (int xi = -1; xi <= 1; xi++)
                {
                    for (int yi = -1; yi <= 1; yi++)
                    {
                        map[rcx[i] + xi, rcy[j] + yi].tile = FLOOR_TILE;
                    }
                }
            }
        }

        // create graph
        for (; rList.Count > 0;)
        {
            vNum = rand.Next() % rList.Count;
            v = rList[vNum];
            k = 0;
            for (int i = 0; i < D_NUM; i++)
            {
                if (rWays[v.x, v.y, i] == -1)
                {
                    if (GetSNMP(v, rParent) == GetSNMP(v + D_WAYS[i], rParent))
                    {
                        rWays[v.x, v.y, i] = 0;
                        r[v.x, v.y]++;
                        continue;
                    }
                    k++;
                }
            }
            if (k > 0)
            {
                x = rand.Next() % k;
                for (int i = 0; i < D_NUM; i++)
                {
                    if (rWays[v.x, v.y, i] == -1)
                    {
                        if (x <= 0)
                        {
                            SNMMerge(v, v + D_WAYS[i], rParent);
                            AddRWay(v, i, r, rWays);
                            break;
                        }
                        x--;
                    }
                }
            }
            if (r[v.x, v.y] == D_NUM || k == 0)
            {
                u = rList[rList.Count - 1];
                rList[rList.Count - 1] = v;
                rList[vNum] = u;
                rList.RemoveAt(rList.Count - 1);
                continue;
            }
        }

        for (int i = 0; i < rNum.x; i++)
        {
            for (int j = 0; j < rNum.y; j++)
            {
                k = 0;
                x = 0;
                for (int p = 0; p < D_NUM; p++)
                {
                    if (rWays[i, j, p] == 0)
                    {
                        k++;
                    }
                    if (rWays[i, j, p] == 1)
                    {
                        x++;
                    }
                }
                if (x <= 1 && k > 0)
                {
                    x = rand.Next() % k;
                    for (int p = 0; p < D_NUM; p++)
                    {
                        if (rWays[i, j, p] == 0)
                        {
                            if (x <= 0)
                            {
                                AddRWay(new Vec2I(i, j), p, r, rWays);
                                break;
                            }
                            x--;
                        }
                    }
                }
            }
        }

        // create tunnels
        for (int i = 0; i < rNum.x; i++)
        {
            for (int j = 0; j < rNum.y; j++)
            {
                for (int p = 0; p < D_NUM; p++)
                {
                    if (D_WAYS[p].x >= 0 && D_WAYS[p].y >= 0 && rWays[i, j, p] == 1)
                    {
                        if (D_WAYS[p].x != 0 && D_WAYS[p].y == 0 && i + 1 < rNum.x)
                        {
                            for (int xi = rcx[i]; xi <= rcx[i + 1]; xi++)
                            {
                                for (int yi = -1; yi <= 1; yi++)
                                {
                                    map[xi, rcy[j] + yi].tile = FLOOR_TILE;
                                }
                            }
                        }
                        if (D_WAYS[p].x == 0 && D_WAYS[p].y != 0 && j + 1 < rNum.y)
                        {
                            for (int yi = rcy[j]; yi <= rcy[j + 1]; yi++)
                            {
                                for (int xi = -1; xi <= 1; xi++)
                                {
                                    map[rcx[i] + xi, yi].tile = FLOOR_TILE;
                                }
                            }
                        }
                    }
                }
            }
        }

        // gen main rooms with teleports
        mRooms = GenRandInRange(new Vec2I(0, rNum.x * rNum.y), mainR.y - mainR.x, rand);
        x = 0;

        // create rooms
        for (int i = 0; i < rNum.x; i++)
        {
            for (int j = 0; j < rNum.y; j++)
            {
                if (mRooms !=  null && x < mRooms.Count && i * rNum.y + j == mRooms[x])
                {
                    GenRoom(new Vec2I(rcx[i], rcy[j]),
                    new Vec2I(maxRSize.x - 1, maxRSize.y - 1),
                    new Vec2I(maxRSize.x - 1, maxRSize.y - 1));
                    rooms[rooms.Count - 1].owner = mROwners[x]; // + mainR.x;
                    cell = map[rcx[i], rcy[j]];
                    cell.ClearTower();
                    cell.teleport = x + mainR.x;
                    cell.tile = TELEPORT_TILE;
                    for (int p = 0; p < D_NUM; p++)
                    {
                        if (rand.NextDouble() < WIZARD_T_GEN_CONST)
                        {
                            cell = map[rcx[i] + MAIN_R_T_GEN_DIST * D_WAYS[p].x, rcy[j] + MAIN_R_T_GEN_DIST * D_WAYS[p].y];
                            cell.ClearTower();
                            cell.structure = TOWER_STRUCT;
                        }
                    }
                    mainRooms.Add(rooms[rooms.Count - 1]);
                    teleports[x + mainR.x] = new Vec2I(rcx[i], rcy[j]);
                    rOwners[i, j] = mROwners[x]; // + mainR.x;
                    rPositions[i, j] = rooms[rooms.Count - 1];
                    if (mROwners[x] != player) // ??
                    {
                        setOwners.Enqueue(new Vec2I(i, j));
                    }
                    x++;
                }
                else
                {
                    if (rand.NextDouble() < ROOM_GEN_CONST)
                    {
                        GenRoom(new Vec2I(rcx[i], rcy[j]), 
                        new Vec2I(minRSize.x + rand.Next() % (maxRSize.x - minRSize.x), 
                        minRSize.y + rand.Next() % (maxRSize.y - minRSize.y)),
                        new Vec2I(minRSize.x + rand.Next() % (maxRSize.x - minRSize.x), 
                        minRSize.y + rand.Next() % (maxRSize.y - minRSize.y)));
                        rPositions[i, j] = rooms[rooms.Count - 1];
                        if (rand.NextDouble() < WIZARD_T_GEN_CONST)
                        {
                            map[rcx[i], rcy[j]].ClearTower();
                            map[rcx[i], rcy[j]].structure = TOWER_STRUCT;
                        }
                    }
                    else
                    {
                        if (rand.NextDouble() < CAMP_GEN_CONST)
                        {
                            map[rcx[i], rcy[j]].structure  = CAMP_STRUCT;
                        }
                        rPositions [i, j] = null;
                    }
                }
            }
        }

        // set rooms owners
        for (; setOwners.Count > 0;)
        {
            u = setOwners.Dequeue();
            for (int i = 0; i < D_NUM; i++)
            {
                v = u + D_WAYS[i];
                if (rWays[u.x, u.y, i] == 1 && rOwners[v.x, v.y] == -1)
                {
                    rOwners[v.x, v.y] = rOwners[u.x, u.y];
                    if (rPositions[v.x, v.y] != null)
                    {
                        rPositions[v.x, v.y].owner = rOwners[v.x, v.y];
                    }
                    setOwners.Enqueue(v);
                }
            }
        }

        // free space around dungeon to optimize drawing
        for (int i = begin.x; i < end.x; i++)
        {
            for (int j = begin.y; j < end.y; j++)
            {
                b = true;
                for (int xi = -1; xi <= 1; xi++) // optimize
                {
                    for (int yi = -1; yi <= 1; yi++)
                    {
                        cell = GetCell(new Vec2I(i + xi, j + yi));
                        if (cell != null && cell.tile != EMPTY_TILE && cell.tile != BLOCK_TILE)
                        {
                            b = false;
                            break;
                        }
                    }
                }
                if (b)
                {
                    map[i, j].tile = EMPTY_TILE;
                }
            }
        }
    }

    public MapCell GetCell(Vec2I pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y)
        {
            return null;
        }
        return map[pos.x, pos.y];
    }

    public void SetREntrances(int rNum, int tile)
    {
        if (rNum < 0 || rNum >= rooms.Count)
        {
            return;
        }
        Room room = rooms[rNum];
        for (int i = 0; i < room.entrances.Count; i++)
        {
            map[room.entrances[i].x, room.entrances[i].y].tile = tile;
        }
    }

}
